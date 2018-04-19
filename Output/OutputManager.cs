using SETextToSpeechMod.Output;
using SETextToSpeechMod.Processing;
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
        
        public List <SpeechTask> Speeches
        {
            get
            {
                return speechesField;
            }
        }
        

        private readonly List <SpeechTask> speechesField = new List <SpeechTask>();
        private SoundPlayer soundPlayerRef;
        TaskFactory taskFactory = new TaskFactory(); 
        private AttendanceManager attendanceManager = AttendanceManager.GetSingleton();

        public static bool IsDebugging { get; private set;}
        public bool IsProcessingOutputs { get; private set;}   
        public bool WasShutDown {get; private set;}

        int timer;
        int[] typeIndexes;  

        public OutputManager (SoundPlayer inputEmitter, bool isDebugging)
        {                             
            if (IsDebugging == false) //Global state which cannot be switched off once set high.
            {
                IsDebugging = isDebugging;
            }  
            soundPlayerRef = inputEmitter;
            FactoryReset();
        }

        private void FactoryReset()
        {
            IsProcessingOutputs = false;
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
            if (WasShutDown == false)
            {
                int lastIndex = Speeches.Count - 1;
                int tasksCompleted = 0;

                Parallel.For(0, lastIndex, (index) => {
                    if (Speeches[index].Worker.HasAnOrder)
                    {
                        IsProcessingOutputs = true;

                        if (Speeches[index].ReturnInfo.Status == TaskStatus.Created) //assuming async calls have matching length to Speeches
                        {                                                
                            taskFactory.StartNew (() => {                             
                                    Speeches[index].Run(); //fixed bug where there was a single returned task from all speeches.
                                }, 
                                Speeches[index].TaskCanceller.Token
                            );                            
                        }

                        else if(Speeches[index].ReturnInfo.Status == TaskStatus.RanToCompletion)
                        {
                            tasksCompleted++;
                        }
                        SetSoundTimer();
                    }

                    if(index == lastIndex &&
                        tasksCompleted == Speeches.Count)
                    {
                        IsProcessingOutputs = false;
                    }
                });
            }
        }

        /// <summary>
        /// a little timer will prevent the emitter from oscillating between your eyes.
        /// </summary>
        private void SetSoundTimer()
        {
            if (timer <= 0) 
            {
                timer = UPDATES_INTERVAL;
                soundPlayerRef.UpdatePosition (attendanceManager.LocalPlayer);                    
            }

            else
            {
                timer--;
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
        /// Returns whether creation was successful.
        /// This can fail if you have called DisposeOfUnsafe().
        /// </summary>
        /// <param name="validVoiceType"></param>
        /// <param name="sentence"></param>
        public bool CreateNewSpeech (string validVoiceTypeName, Sentence inputSentence)
        {
            bool outcome = default (bool);

            if (WasShutDown == false)
            {
                int currentTypeIndex = GetOutputTypesIndex (validVoiceTypeName);

                if (currentTypeIndex != DOESNT_EXIST)
                {              
                    int firstTypeInstance = currentTypeIndex * TOTAL_SIMULTANEOUS_SPEECHES;

                    if (typeIndexes[currentTypeIndex] >= TOTAL_SIMULTANEOUS_SPEECHES)
                    {
                        typeIndexes[currentTypeIndex] = 0;                    
                    }      
                    int newSpeechIndex = firstTypeInstance + typeIndexes[currentTypeIndex];   
                    typeIndexes[currentTypeIndex]++;       
                    Speeches[newSpeechIndex].FactoryReset (inputSentence);                                          
                    outcome = true;
                    IsProcessingOutputs = true;  
                }
            }
            return outcome;
        }

        public void DisposeOfUnsafe()
        {
            WasShutDown = true;
            IsProcessingOutputs = false;

            for (int i = 0; i < Speeches.Count; i++)
            {
                Speeches[i].Dispose();
            }            
        }
    }
}
