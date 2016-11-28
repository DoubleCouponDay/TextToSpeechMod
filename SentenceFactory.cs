using System;
using System.Collections.Generic;

namespace SETextToSpeechMod
{   
    public abstract class SentenceFactory : StateResetTemplate, VoiceTemplate
    {   
        //voice template
        public virtual string Name { get { return "SentenceFactory";} }
        public virtual string FileID { get; }
        public virtual int SpaceSize { get; } //public because these properties are part of interface VoiceTemplate.
        public virtual int ClipLength { get; }
        public virtual int SyllableSize { get; }        

        //pitch        
        protected virtual int[][] smallIntonationOptions { get; }
        protected virtual int[][] mediumIntonationOptions { get; }
        protected virtual int[][] largeIntonationOptions { get; }
        protected int[][][] allPitchSizes;

        //public state        
        public bool FinishedPlaying {get; protected set;}

        //index
        protected int letterIndex; 
        protected int optionsIndex;                      
        protected int arraysIndex; 
        
        //loading data            
        bool Loading;
        protected string sentence;
        bool previousWasSpace;
        int syllableMeasure;          
        protected int[][] intonationChoice;

        //play data
        int currentTick;

        public IList <string> Results
        {
            get
            {
                return resultsField.AsReadOnly();
            }
        }
        List <string> resultsField;

        //objects     
        public Pronunciation Pronunciation { get; private set; }   
        private List <TimelineClip> timeline = new List <TimelineClip>();      
        protected Random rng = SoundPlayer.NumberGenerator; //some voices need to change this object's usage. Therefore, it is protected.

        public SentenceFactory()
        {            
            Loading = false;
            FinishedPlaying = true;
            Pronunciation = new Pronunciation();
            resultsField = new List <string>();
            timeline.Capacity = OutputManager.MAX_LETTERS; //lists resize constantly when filling. better to know its limit and prevent that to increase performance;
                                                           //also i dont expect there to ever be more phonemes than letters. It will only resize in rare, designer created, occasions.
            allPitchSizes = new int[][][]     
            {
                smallIntonationOptions,
                mediumIntonationOptions,
                largeIntonationOptions,
            };
        }

        public void FactoryReset (string inputSentence)
        {
            Loading = true;
            FinishedPlaying = false;           

            letterIndex = 0; 
            optionsIndex = 0;
            arraysIndex = 0;

            sentence = inputSentence;
            
            previousWasSpace = false;
            syllableMeasure = 0;
            intonationChoice = null;

            currentTick = 0;
            resultsField.Clear();

            Pronunciation.FactoryReset (sentence);
            timeline.Clear();
        }

        //this function will extract what phonemes it can from the sentence and save performance by taking its sweet time.
        public void Run()
        {    
            if (Loading == true)
            {   
                if (letterIndex < sentence.Length)
                {                
                    letterIndex++;
                    ProcessSentence(); 
                }   

                else
                {
                    Loading = false;    
                }        
            }

            else 
            {
                FinishedPlaying = SoundPlayer.PlaySentence (timeline, currentTick);
                currentTick++;
            }    
        } 
        
        private void ProcessSentence()
        {   
            resultsField = Pronunciation.GetLettersPronunciation (sentence, letterIndex);

            for (int i = 0; i < resultsField.Count; i++)
            {
                if (resultsField[i] != "") //AdjacentEvaluation() can return an empty string sometimes.
                {
                    if (resultsField[i] != " ") //empty string is simple a pause in the speech
                    {                                                   
                        string soundChoice = resultsField[i] + FileID + GetPhonemesIntonation();    
                        AddToTimeline (soundChoice);                    
                        syllableMeasure++;

                        if (syllableMeasure == SyllableSize) //cues a pause using the current setting SyllableSize.
                        {
                            previousWasSpace = true; //pronunciation class inserts spaces for low energy letters. i dont want double spaces so thats the purpose of this var.
                            IncrementSyllables();                       
                        }   
                    }

                    else
                    {
                        if (previousWasSpace == false)
                        {
                            IncrementSyllables();
                        }       
                        
                        else
                        {
                            previousWasSpace = false;
                        }                 
                    }
                }
            }
        }

        protected virtual string GetPhonemesIntonation()
        {
            string choice = " ";
            
            if (intonationChoice == null)
            {
                foreach (int[][] size in allPitchSizes)
                {
                    if (sentence.Length <= size[0].Length)
                    { 
                        intonationChoice = size; //just a pointer
                        optionsIndex = rng.Next (size.GetLength (1));
                    }
                }
            }

            if (arraysIndex >= intonationChoice[optionsIndex].Length)
            {
                arraysIndex = 0; //in the unlikely event there are more phonemes than letters, it will start from the beginning.
            }            
            choice += intonationChoice[optionsIndex][arraysIndex];
            arraysIndex++;
            return choice;
        }

        private void AddToTimeline (string inputSound)
        {
            int startPoint = 0;                                                                      
                        
            if (timeline.Count != 0)
            {
                startPoint = timeline[timeline.Count - 1].StartPoint + SpaceSize;         
            }  
            timeline.Add (new TimelineClip (startPoint, inputSound));
        }

        private void IncrementSyllables()
        {
            AddToTimeline (SoundPlayer.SPACE);
            syllableMeasure = 0;
        }          
    }

    public struct TimelineClip
    {
        public int StartPoint { get; }
        public string ClipsSound { get; }

        internal TimelineClip (int inputPoint, string inputSound)
        {
            StartPoint = inputPoint;
            ClipsSound = inputSound;
        }
    } 
}
