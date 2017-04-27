using System;
using System.Collections.Generic;
using SETextToSpeechMod.Processing;
using System.Threading.Tasks;

namespace SETextToSpeechMod
{   
    public abstract class TimelineFactory
    {   
        const string SPACE = " ";

        public abstract int ClipLength { get; }
        public abstract int SyllableSize { get; }     
        public abstract int SpaceSize { get; }   

        //external feedback        
        public bool IsBusy { get; private set;}
        public bool HasAnOrder { get; private set;}

        //loading data            
        protected string sentence = "";
        bool previousWasSpace;
        int syllableMeasure;          
        protected int[] intonationArrayChosen;
        int spaceOffsetTemp;

        public IList <string> CurrentResults
        {
            get
            {
                return currentResultsField.AsReadOnly();
            }
        }
        List <string> currentResultsField;

        public IList <TimelineClip> Timeline
        {
            get
            {
                return timelinesField.AsReadOnly();
            }
        }
        public List <TimelineClip> timelinesField = new List <TimelineClip>();    

        //objects     
        public Pronunciation Pronunciation { get; private set; }      
        protected Random rng = new Random();
        private SoundPlayer soundPlayerRef; 

        public TimelineFactory (SoundPlayer inputEmitter, Intonation intonationType)
        {       
            Pronunciation = new Pronunciation (intonationType);
            currentResultsField = new List <string>();
            timelinesField.Capacity = OutputManager.MAX_LETTERS; //lists resize constantly when filling. better to know its limit and prevent that to increase performance;            
            soundPlayerRef = inputEmitter; //the reason im using a pointer is there is no need for a SoundPlayer per SentenceFactory.                                                            
        }

        /// <summary>
        /// Initialises a new sentence. You need to Run FactoryReset() before calling this.
        /// </summary>
        /// <param name="inputSentence"></param>
        public void FactoryReset (string inputSentence)
        {       
            IsBusy = false;
            HasAnOrder = true;

            sentence = inputSentence;
            
            previousWasSpace = false;
            syllableMeasure = 0;
            intonationArrayChosen = null;

            currentResultsField.Clear();

            Pronunciation.FactoryReset();
            timelinesField.Clear();            
        }

        //this function will extract what phonemes it can from the sentence and save performance by taking its sweet time.
        public async Task RunAsync()
        {                     
            if (HasAnOrder)
            {
                IsBusy = true;

                await Task.Run (() => { 
                    for (int i = 0; i < sentence.Length; i++)
                    {
                        AddPhonemes (i);
                    }
                });
                await soundPlayerRef.PlaySentence (timelinesField);
                IsBusy = false;
                HasAnOrder = false;
            }            
        } 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentIndex">Index of the sentence the factory is processing.</param>
        private void AddPhonemes (int currentIndex)
        {   
            currentResultsField = Pronunciation.GetLettersPronunciation (sentence, currentIndex);

            for (int i = 0; i < currentResultsField.Count; i++)
            {
                if (currentResultsField[i] != "") //AdjacentEvaluation() can return an empty string sometimes.
                {
                    if (currentResultsField[i] != SPACE)
                    {                                                   
                        AddToTimeline (currentResultsField[i]);                                            

                        if (syllableMeasure >= SyllableSize - 1)
                        {                            
                            IncrementSyllables();                       
                        }   

                        else
                        {
                            previousWasSpace = false;
                            syllableMeasure++;
                        }
                    }

                    else
                    {
                        if (previousWasSpace == false) //prevents syllables and actual whitespace from combining.
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

        private void AddToTimeline (string inputSound)
        {
            int startPoint = 0;                                                                      
                        
            if (timelinesField.Count != 0)
            {
                startPoint = timelinesField[timelinesField.Count - 1].StartPoint + spaceOffsetTemp + ClipLength;        
                spaceOffsetTemp = default (int); 
            }  
            timelinesField.Add (new TimelineClip (startPoint, inputSound));
        }

        private void IncrementSyllables()
        {
            previousWasSpace = true; //pronunciation class inserts spaces for low energy letters. i dont want double spaces so thats the purpose of this var.        {
            spaceOffsetTemp = SpaceSize;
            syllableMeasure = 0;
        } 
    }

    public struct TimelineClip
    {
        public int StartPoint { get; }
        public string ClipsSound { get; }

        internal TimelineClip (int inputPoint, string inputSound)
        {
            this.StartPoint = inputPoint;
            this.ClipsSound = inputSound;
        }
    } 
}
