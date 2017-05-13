using System;
using System.Collections.Generic;

using Sandbox.ModAPI;
using Sandbox.Game.Entities;

using VRage.ModAPI;
using VRage.Game.Entity;
using VRageMath;
using VRage.Game.ModAPI;
using System.Threading.Tasks;
using SETextToSpeechMod.Processing;

namespace SETextToSpeechMod
{
    public class SoundPlayer
    {    
        const int CHANCE_OF_CLANG = 10000; //cannot be zero
        const string BONK = "BONK";     

        MyEntity3DSoundEmitter TTSEmitter;
        Random numberGenerator = new Random();

        private float volume = 0.6f;
        private const float DISTANCE = 500.0f;
            
        private readonly bool debugging;
        private readonly bool playInRealisticMode;

        public SoundPlayer (bool isDebugging, bool inputRealisticModeOption)
        {
            this.debugging = isDebugging;
            this.playInRealisticMode = inputRealisticModeOption;                 
            Initialise();
        }

        private void Initialise() //i tested with a constructor instead but it doesnt seem to work with the game?
        {            
            if (debugging == false)
            {
                IMyEntity emitterEntity = new MyEntity() as IMyEntity; //couldnt instantiate MyEntity so had to use its cast.
                TTSEmitter = new MyEntity3DSoundEmitter (emitterEntity as MyEntity);
                TTSEmitter.CustomMaxDistance = DISTANCE; //since emitter position updates every interval, the distance should be large.
                TTSEmitter.SourceChannels = 1;
                TTSEmitter.Force2D = true;
                TTSEmitter.CustomVolume = volume;
            }
        }

        /// <summary>
        /// Checks localPlayer for null reference.
        /// </summary>
        /// <param name="localPlayer"></param>
        public void UpdatePosition (IMyPlayer localPlayer)
        {
            if (localPlayer != null)
            {
                Vector3D pos3D = localPlayer.GetPosition(); 
                Vector3I pos3I = new Vector3I (pos3D); //some formatting is required
                Vector3 pos3 = new Vector3 (pos3I);
                TTSEmitter.SetPosition (pos3);
            }
        }

        /// <summary>
        /// Input volume must be between 0.0f and 1.0f
        /// </summary>
        /// <param name="newVolume"></param>
        public void UpdateVolume (float newVolume)
        {
            if (newVolume >= default (float) &&
                newVolume <= 1.0f)
            {
                volume = newVolume;
                MyAPIGateway.Utilities.ShowMessage ("New speech volume: ", volume.ToString());
            }

            else
            {
                MyAPIGateway.Utilities.ShowMessage ("", "Cannot apply that new volume. It must be between 0.0 and 1.0");
            }            
        }

        public async Task PlaySentence (List <TimelineClip> timeline)
        {            
            if (timeline.Count > 0)
            {
                int currentClip = default (int);
                var timelineClone = timeline.ToArray(); //assume that timeline is kill during Task.Run

                await Task.Run (() => {
                    for (int i = 0; i <= timelineClone[timelineClone.Length - 1].StartPoint; i++)
                    {
                        int rngBonk = numberGenerator.Next (CHANCE_OF_CLANG);

                        if (rngBonk == CHANCE_OF_CLANG - 1) 
                        {
                            PlayClip (BONK); //repent you fucking sinner. CLANG
                        } 

                        if (i == timelineClone[currentClip].StartPoint)
                        {
                            PlayClip (timelineClone[currentClip].ClipsSound);
                        }
                    }                 
                });
            }       
        }

        private void PlayClip (string clip)
        {
            if (debugging == false)
            {
                MySoundPair sound = new MySoundPair (clip);                
                TTSEmitter.PlaySound (sound, false, false, false, playInRealisticMode, false); //this overload enables sound in realistic mode.
            }
        }
    }
}
    