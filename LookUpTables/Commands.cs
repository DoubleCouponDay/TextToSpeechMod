using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.LookUpTables
{
    class Commands
    {        
        public readonly string CHANGE_VOICE_TO_MAREK = "[ MAREK";
        //public readonly string CHANGE_VOICE_TO_HAWKING {get; private set;} = "[ JOHN MADDEN";
        public readonly string CHANGE_VOICE_TO_GLADOS = "[ GLADOS";

        public readonly string[] VoiceCollection;

        public readonly string MUTE_PLAYER = "[ MUTE";
        public readonly string UNMUTE_PLAYER = "[ UNMUTE";
        public readonly int MUTING_MIN_SIZE = 3;

        public readonly string CHANGE_VOLUME = "[ VOLUME";

        private static Commands instance = new Commands();

        private Commands()
        {
            VoiceCollection = new string[]
            {
                CHANGE_VOICE_TO_MAREK,
                //CHANGE_VOICE_TO_HAWKING,
                CHANGE_VOICE_TO_GLADOS,
            };
        }

        public static Commands GetSingleton()
        {
            return instance;
        }
    }
}
