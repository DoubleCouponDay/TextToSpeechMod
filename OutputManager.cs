using System;
using System.Collections.Generic;

namespace SETextToSpeechMod
{
    static class OutputManager
    {
        const int UPDATES_INTERVAL = 60;
        const int TOTAL_SPEECHES = 5;

        static bool speechDemanded;
        static int timer;
        static Type localPlayersField = POSSIBLE_OUTPUTS.HawkingType;      

        /// <summary>
        /// The only accepted inputs are contained in public POSSIBLE_OUTPUTS 
        /// </summary>
        public static Type LocalPlayersVoice
        {
            get
            {
                return localPlayersField;  
            }

            set
            {
                if (CheckRequestIsValid (value))
                {
                    localPlayersField = value;
                }
            }
        }
        public static bool Debugging { get; set; } //globabl state

        public static List <SentenceFactory> Speeches { get; private set; }

        static OutputManager()
        {
            FactoryReset();
            Speeches = new List <SentenceFactory>();

            for (int i = 0; i < POSSIBLE_OUTPUTS.AllOptions.Count; i++)
            {
                for (int k = 0; k < TOTAL_SPEECHES; k++)
                {     
                    Speeches.Add (Activator.CreateInstance (POSSIBLE_OUTPUTS.AllOptions[i]) as SentenceFactory);
                    Console.WriteLine (Speeches[Speeches.Count - 1].Name);
                }                
            }
        }

        public static void FactoryReset()
        {
            speechDemanded = false;
        }

        public static void Run()
        {
            if (speechDemanded == true)
            {
                speechDemanded = false; 

                if (timer <= 0) //im hoping that a little distance will prevent the oscillating position whch annoys ear drums.
                {
                    timer = UPDATES_INTERVAL;
                    SoundPlayer.UpdatePosition();                    
                }

                else
                {
                    timer--;
                }               

                for (int i = 0; i < TOTAL_SPEECHES; i++) 
                {                    

                }
            }
        }

        private static bool CheckRequestIsValid (Type typeDemanded)
        {
            bool outcome = false;

            for (int i = 0; i < POSSIBLE_OUTPUTS.AllOptions.Count; i++)
            {
                if (POSSIBLE_OUTPUTS.AllOptions[i] == typeDemanded)
                {
                    outcome = true;
                }
            }
            return outcome;
        }

        /// <summary>
        /// The only accepted inputs are contained in public POSSIBLE_OUTPUTS
        /// </summary>
        /// <param name="validVoiceType"></param>
        /// <param name="sentence"></param>
        public static void CreateNewSpeech (Type validVoiceType, string sentence)
        {
            if (CheckRequestIsValid (validVoiceType))
            {
                speechDemanded = true;

                for (int i = Speeches.Count - 2; i >= 1; i--)
                {
                    Speeches[i + 1] = Speeches[i];
                }                
                Speeches[0].FactoryReset (sentence);
            }
        }
    }

    public struct POSSIBLE_OUTPUTS
    {        
        public static Type MarekType { get; private set; }
        public static Type HawkingType { get; private set; }
        public static Type GLADOSType { get; private set; }
        public static List <Type> AllOptions { get; private set; }

        public static int AutoSignatureSize { get; private set; }

        static POSSIBLE_OUTPUTS()
        {
            MarekType = typeof (MarekVoice);
            HawkingType = typeof (HawkingVoice);
            GLADOSType = typeof (GLADOSVoice);
            AllOptions = new List <Type>();
            AllOptions.Add (MarekType);
            AllOptions.Add (HawkingType);
            AllOptions.Add (GLADOSType);

            for (int i = 0; i < AllOptions.Count; i++)
            {
                if (AllOptions[i].ToString().Length > AutoSignatureSize)
                {
                    AutoSignatureSize = AllOptions[i].ToString().Length;
                }
            }
        }
    } 
}
