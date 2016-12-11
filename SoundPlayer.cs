using System;
using System.Collections.Generic;

using Sandbox.ModAPI;
using Sandbox.Game.Entities;

using VRage.ModAPI;
using VRage.Game.Entity;
using VRageMath;

namespace SETextToSpeechMod
{
    static class SoundPlayer //A stateless class equip to handle sound emitting requests and anything associated.
    {
        const int CHANCE_OF_CLANG = 10000; //cannot be zero
        const string BONK = "BONK";
        const string SPACE = "SPACE";       

        static MyEntity3DSoundEmitter TTSEmitter;
        static Random numberGenerator = new Random();

        public static float TTSVolume
        {
            get { return volume; }

            set
            {
                if (value >= 0.0f &&
                    value <= 1.0f)
                {
                    volume = value;
                }
            }
        }
        static float volume = 0.6f;

        public static void InitialiseEmitter() //i tested with a constructor instead but it doesnt seem to work with the game?
        {
            IMyEntity emitterEntity = new MyEntity() as IMyEntity; //couldnt instantiate MyEntity so had to use its cast.
            TTSEmitter = new MyEntity3DSoundEmitter (emitterEntity as MyEntity);
            TTSEmitter.CustomMaxDistance = 500.0f; //since emitter position updates every interval, the distance should be large.
            TTSEmitter.SourceChannels = 1;
            TTSEmitter.Force3D = true;
            TTSEmitter.CustomVolume = volume;
        }

        public static void UpdatePosition()
        {
            for (int i = 0; i < AttendanceManager.Players.Count; i++)
            {
                if (AttendanceManager.Players[i].SteamUserId == MyAPIGateway.Multiplayer.MyId) //finds the client in the pool of players.
                {
                    Vector3D pos3D = AttendanceManager.Players[i].GetPosition(); //some formatting is required
                    Vector3I pos3I = new Vector3I (pos3D);
                    Vector3 pos3 = new Vector3 (pos3I);
                    TTSEmitter.SetPosition (pos3);
                }
            }
        }

        public static bool PlaySentence (List <TimelineClip> inputTimeline, int currentTick)
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

        private static void PlayClip (string clip)
        {
            if (OutputManager.Debugging == false)
            {
                MySoundPair sound = new MySoundPair (clip);
                TTSEmitter.PlaySound (sound, false, false, false, true, false); //this overload enables sound in realistic mode.
            }
        }
    }
}
