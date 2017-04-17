using SETextToSpeechMod.Output;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SETextToSpeechMod
{
    public class OutputManager
    {
        public const int MAX_LETTERS = 100;
        const int UPDATES_INTERVAL = 60;
        const int TOTAL_SIMULTANEOUS_SPEECHES = 5;
        const int DOESNT_EXIST = -1;             
                
        public Type LocalPlayersVoice
        {
            get
            {
                return localVoiceField;  
            }

            set
            {
                localVoiceField = value;
            }
        }
        Type localVoiceField = PossibleOutputs.HawkingType;
        
        /// <summary>
        /// Same sized groups of sentence types are ordered by their position in struct PossibleOutputs.Collection
        /// </summary>                
        internal IList <SpeechTask> Speeches
        {
            get
            {
                return speechesField.AsReadOnly();
            }
        }
        private readonly List <SpeechTask> speechesField = new List <SpeechTask>();
        private SoundPlayer soundPlayerRef;

        TaskFactory taskFactory = new TaskFactory();

        public static bool IsDebugging {get; private set;}
        public bool RunSpeechPlayback { get; private set;}        

        int timer;
        int[] typeIndexes;  

        public OutputManager (SoundPlayer inputEmitter, bool isDebugging)
        {
            if (IsDebugging == false)
            {
                IsDebugging = isDebugging;
            }                  
            soundPlayerRef = inputEmitter;
            FactoryReset();
        }

        public void FactoryReset()
        {
            RunSpeechPlayback = false;
            typeIndexes = new int[PossibleOutputs.Collection.Count];
            speechesField.Clear();

            for (int i = 0; i < PossibleOutputs.Collection.Count; i++)
            {
                for (int k = 0; k < TOTAL_SIMULTANEOUS_SPEECHES; k++)
                {
                    if (PossibleOutputs.Collection[i] == PossibleOutputs.MarekType)
                    {
                        speechesField.Add (new SpeechTask (new MarekVoice (soundPlayerRef)));                                       
                    }

                    else if (PossibleOutputs.Collection[i] == PossibleOutputs.HawkingType)
                    {
                        speechesField.Add (new SpeechTask (new MarekVoice (soundPlayerRef)));                                       
                    }

                    else if (PossibleOutputs.Collection[i] == PossibleOutputs.GLADOSType)
                    {
                        //speechesField.Add new SpeechTask (new MarekVoice (soundPlayerRef)));                                       
                    }                      
                }                
            }   
        }

        public void Run()
        {         
            if (RunSpeechPlayback == true)
            {
                RunSpeechPlayback = false; 

                for (int i = 0; i < speechesField.Count; i++) 
                {                    
                    if (speechesField[i].Speech.IsFresh ||
                        speechesField[i].Speech.IsBusy)
                    {
                        RunSpeechPlayback = true;

                        if (speechesField[i].ReturnInfo.IsCompleted == true) //assuming asyncCalls has matching length to speechesField
                        {
                            taskFactory.StartNew (() => {
                                speechesField[i].RunAsync(); //fixed bug where there was a single returned task from all speeches.
                                }, 
                                speechesField[i].TaskCanceller.Token
                            );                            
                        }
                    }
                }

                if (timer <= 0) //a little timer will prevent the emitter from oscillating between your eyes.
                {
                    timer = UPDATES_INTERVAL;
                    soundPlayerRef.UpdatePosition (AttendanceManager.LocalPlayer);                    
                }

                else
                {
                    timer--;
                }                               
            }
        }

        /// <summary>
        /// Returns integer indicating the type is indexed in struct PossibleOutputs.
        /// Can be -1 if no match found.
        /// </summary>
        /// <param name="scrutinizedType"></param>
        /// <returns></returns>
        private int GetOutputTypesIndex (string scrutinizedTypeName)
        {
            int outcome = DOESNT_EXIST;

            for (int i = 0; i < PossibleOutputs.Collection.Count; i++)
            {
                if (PossibleOutputs.Collection[i].FullName == scrutinizedTypeName)
                {
                    outcome = i;
                }
            }
            return outcome;
        }

        /// <summary>
        /// Resets the next speech within the group of the same voice type, instead of incurring performance costs but instantiating every time.
        /// The only accepted types are contained in public struct PossibleOutputs.
        /// </summary>
        /// <param name="validVoiceType"></param>
        /// <param name="sentence"></param>
        public void CreateNewSpeech (string validVoiceTypeName, string inputSentence)
        {
            int currentTypeIndex = GetOutputTypesIndex (validVoiceTypeName);

            if (currentTypeIndex != DOESNT_EXIST)
            {
                RunSpeechPlayback = true;                
                int firstTypeInstance = currentTypeIndex * TOTAL_SIMULTANEOUS_SPEECHES;

                if (typeIndexes[currentTypeIndex] >= TOTAL_SIMULTANEOUS_SPEECHES)
                {
                    typeIndexes[currentTypeIndex] = 0;                    
                }      
                int newSpeechIndex = firstTypeInstance + typeIndexes[currentTypeIndex];

                
while (asyncCalls[newSpeechIndex].IsCompleted == false) { }
                speechesField[newSpeechIndex].Speech.FactoryReset (inputSentence); //reuses instances of sentencefactory instead of instantiating every new sentence.
                typeIndexes[currentTypeIndex]++;                         
            }
        }

        public void DisposeOfUnsafe()
        {
            for (int i = 0; i < speechesField.Count; i++)
            {
                speechesField[i].TaskCanceller.Dispose();
            }
        }
    }
}
