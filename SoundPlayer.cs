using System;
using System.Collections.Generic;

using Sandbox.ModAPI;
using Sandbox.Game.Entities;

using VRage.ModAPI;
using VRage.Game.Entity;
using VRageMath;
using VRage.Game.ModAPI;

namespace SETextToSpeechMod
{
    public class SoundPlayer
    {
        const int CHANCE_OF_CLANG = 10000; //cannot be zero
        const string BONK = "BONK";
        const string SPACE = "SPACE";       

        MyEntity3DSoundEmitter TTSEmitter;
        Random numberGenerator = new Random();

        private float volume = 0.6f;
            
        private bool debugging;

        public SoundPlayer (bool isDebugging)
        {
            debugging = isDebugging;
            Initialise();
        }

        private void Initialise() //i tested with a constructor instead but it doesnt seem to work with the game?
        {            
            if (debugging == false)
            {
                IMyEntity emitterEntity = new MyEntity() as IMyEntity; //couldnt instantiate MyEntity so had to use its cast.
                TTSEmitter = new MyEntity3DSoundEmitter (emitterEntity as MyEntity);
                TTSEmitter.CustomMaxDistance = 500.0f; //since emitter position updates every interval, the distance should be large.
                TTSEmitter.SourceChannels = 1;
                TTSEmitter.Force3D = true;
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
            if (newVolume >= 0.0f &&
                newVolume <= 1.0f)
            {
                volume = newVolume;
                MyAPIGateway.Utilities.ShowMessage ("New speech volume: ", volume.ToString());
            }
            MyAPIGateway.Utilities.ShowMessage ("", "Cannot apply that new volume. It must be between 0.0 and 1.0");
        }

        public bool PlaySentence (List <TimelineClip> inputTimeline, int currentTick)
        {
            bool hasFinished = false;
            int rngBonk = numberGenerator.Next (CHANCE_OF_CLANG);

            if (rngBonk == CHANCE_OF_CLANG - 1) //repent you fucking sinner. CLANG
            {
                PlayClip (BONK);
            } 

            if (inputTimeline.Count > 0)
            {
                while (inputTimeline.Count > 0 &&
                       inputTimeline[0].StartPoint <= currentTick)
                {
                    if (inputTimeline[0].ClipsSound != SPACE)
                    {
                        PlayClip (inputTimeline[0].ClipsSound);
                    }
                    inputTimeline.RemoveAt (0);
                } 
            }                           
            
            else
            {
                hasFinished = true;
            }
            return hasFinished;
        }

        private void PlayClip (string clip)
        {
            if (debugging == false)
            {
                MySoundPair sound = new MySoundPair (clip);
                TTSEmitter.PlaySound (sound, false, false, false, true, false); //this overload enables sound in realistic mode.
            }
        }
    }
}
