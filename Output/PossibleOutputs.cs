using SETextToSpeechMod.VoiceCode.HawkingVoice;
using System;
using System.Collections.Generic;

namespace SETextToSpeechMod
{
    static class PossibleOutputs
    {        
        public static Type MarekType { get { return typeof (MarekVoice); } }
        public static Type HawkingType { get { return typeof (HawkingVoice); } }
        public static Type GLADOSType { get { return typeof (GladosVoice); } }
        
        public static IList <Type> Collection
        {
            get
            {
                return allOptionsField.AsReadOnly();
            }
        }

        private static List <Type> allOptionsField = new List<Type>()
        {
            MarekType, //marek must be first for optionaldebugger to work
            HawkingType, 
            //GLADOSType,
        };
        public static int AutoSignatureSize { get; }

        static PossibleOutputs()
        {
            for (int i = 0; i < allOptionsField.Count; i++)
            {
                int typeNamesSize = allOptionsField[i].ToString().Length;

                if (typeNamesSize > AutoSignatureSize)
                {
                    AutoSignatureSize = typeNamesSize;
                }
            }
        }
    } 
}
