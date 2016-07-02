using System.Collections.Generic;

using Sandbox.ModAPI;
using Sandbox.Game.Entities;
using VRage.ModAPI;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRageMath;

namespace SETextToSpeechMod
{
    static class SoundPlayer //i only need one emitter per player.
    {
        public static MyEntity3DSoundEmitter TTSEmitter { get; set; }
        public static MyEntity3DSoundEmitter bonkEmitter { get; set; }
        public const float DEFAULT_VOLUME = 0.6f;

        public static void InitialiseEmitter()
        {
            IMyEntity emitter_entity = new MyEntity() as IMyEntity; //couldnt instantiate MyEntity so had to use its cast.
            TTSEmitter = new MyEntity3DSoundEmitter (emitter_entity as MyEntity);
            TTSEmitter.CustomMaxDistance = 500.0f; //since emitter position updates every interval, the distance should be large.
            TTSEmitter.SourceChannels = 1;
            TTSEmitter.Force2D = true;
            TTSEmitter.CustomVolume = DEFAULT_VOLUME; //my sounds are already clipping; people dont want it that loud.
            bonkEmitter = TTSEmitter;
            bonkEmitter.CustomVolume = 0.4f;
        }

        public static void UpdatePosition (List <IMyPlayer> players)
        {
            for (int i = 0; i < players.Count; i++) //performance danger
            {
                if (players[i].SteamUserId == MyAPIGateway.Multiplayer.MyId) //finds the client in the pool of players.
                {
                    Vector3D pos3D = players[i].GetPosition(); //some formatting is required
                    Vector3I pos3I = new Vector3I (pos3D);
                    Vector3 pos3 = new Vector3 (pos3I);
                    TTSEmitter.SetPosition (pos3);
                    bonkEmitter.SetPosition (pos3);
                }
            }
        }

        public static void PlayClip (string clip, bool shouldIBonk)
        {
            MyEntity3DSoundEmitter chosenEmitter = shouldIBonk ? bonkEmitter : TTSEmitter;
            MySoundPair sound = new MySoundPair (clip);
            chosenEmitter.PlaySound (sound);
        }
    }
}
