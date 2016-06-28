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
        public static MyEntity3DSoundEmitter tts_emitter { get; set; }
        public static MyEntity3DSoundEmitter bonkEmitter { get; set; }
        public const float DEFAULT_VOLUME = 0.6f;

        public static void InitialiseEmitter()
        {
            IMyEntity emitter_entity = new MyEntity() as IMyEntity; //couldnt instantiate MyEntity so had to use its cast.
            tts_emitter = new MyEntity3DSoundEmitter (emitter_entity as MyEntity);
            tts_emitter.CustomMaxDistance = 10.0f;
            tts_emitter.SourceChannels = 1;
            tts_emitter.Force2D = true;
            tts_emitter.CustomVolume = DEFAULT_VOLUME; //my sounds are already clipping; people dont want it that loud.
            bonkEmitter = tts_emitter;
        }

        public static void UpdatePosition (List <IMyPlayer> players)
        {
            for (int i = 0; i < players.Count; i++) //performance danger
            {
                if (players[i].SteamUserId == MyAPIGateway.Multiplayer.MyId) //finds the client in the pool of players.
                {
                    Vector3D pos_3D = players[i].GetPosition(); //some formatting is required
                    Vector3I pos_3I = new Vector3I (pos_3D);
                    Vector3 pos_3 = new Vector3 (pos_3I);
                    tts_emitter.SetPosition (pos_3);
                }
            }
        }

        public static void PlayClip (string clip, bool shouldIBonk)
        {
            MyEntity3DSoundEmitter chosenEmitter = shouldIBonk ? bonkEmitter : tts_emitter;
            MySoundPair sound = new MySoundPair (clip);
            chosenEmitter.PlaySound (sound);
        }
    }
}
