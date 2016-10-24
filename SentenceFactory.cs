using System;
using System.Text;
using System.Collections.Generic;

namespace SETextToSpeechMod
{   
    class SentenceFactory //the roman numeral class name may help you to understand the code flow.
    {
        //reference settings         
        const int CHANCE_OF_CLANG = 10000;
        protected virtual int SpaceSize { get { return 0; } }
        protected virtual int ClipLength { get { return 0; } }
        protected virtual int SyllableSize { get { return 0; } }
        protected virtual string VoiceID { get { return ""; } }
        protected readonly string SENTENCE;

        //state
        public bool loading {get; protected set;} //made public so the debugger can take readings
        public bool finished {get; protected set;}

        //debugging
        public bool debugging {private get; set;}

        //index
        int letterIndex;
        int syllableMeasurer = 1;    

        //play data
        string timeline;
        string timelineCopy;
        int timelineSize;
        int currentTick;

        //loading data            
        bool previousWasSpace;
        public List <string> results {get; protected set;}

        //objects    
        private Random generator = new Random();
        private StringBuilder stringLite = new StringBuilder();
        public Pronunciation pronunciation {get; protected set;}        

        public SentenceFactory(){}

        protected SentenceFactory (string input)
        {
            SENTENCE = input.Remove (0, 2); //getting rid of the trigger since its unnecessary. while testing, words smaller than the count of 2 will throw therefore...     
            pronunciation = new Pronunciation (SENTENCE);
            loading = true;
        }

        //this function will extract what phonemes it can from the sentence and save performance by taking its sweet time.
        public void Load()
        {    
            if (loading == true)
            {   
                if (letterIndex < SENTENCE.Length)
                {                
                    AddPhoneme(); 
                    letterIndex++;
                }   

                else
                {
                    loading = false;    
                    stringLite.Append ("/"); //the cap on the timeline mix.
                    timeline = stringLite.ToString();
                    stringLite.Clear();
                    timelineCopy = timeline;
                }        
            }

            else
            {
                int rng_bonk = generator.Next (CHANCE_OF_CLANG);

                if (rng_bonk == 0)
                {
                    SoundPlayer.PlayClip (debugging, "BONK"); //it hurts to live
                }
                Play();    
            }    
        } 

        //creates a new clip for the current letter.
        void AddPhoneme()
        {   
            results = pronunciation.GetLettersPronunciation (SENTENCE, letterIndex);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i] != "")
                {
                    if (results[i] != " ") //empty string lets my program know that no clip should be created.
                    {                                                                
                        int startPoint = timelineSize;         
                        string soundChoice = results[i] + VoiceID;
                        AppendToTimeline (startPoint, soundChoice); //add the key transitions into the timeline.
                        timelineSize += ClipLength; //timeline is expanded for duration after the clip is created.

                        if (syllableMeasurer == SyllableSize) //cues a space using the current setting SyllableSize.
                        {
                            previousWasSpace = true; //pronunciation class inserts spaces for low energy letters. i dont want double spaces.
                            IncrementSyllables();                       
                        }   
                        
                        else
                        {
                            syllableMeasurer++;
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

        void IncrementSyllables()
        {
            timelineSize += SpaceSize;        
            syllableMeasurer = 1;
        }

        //creates a string of all the phonemes and their start points (in ticks); better performance than searching a list of objects.
        void AppendToTimeline (int startPoint, string clipsSound) 
        {      
            stringLite.Append ("/");    
            stringLite.Append (startPoint); //leaving out clear this one time is ok since it clears when loading is finished.
            stringLite.Append ("#"); 
            stringLite.Append (clipsSound);                    
        }

        //this function is in charge of finding clips on the timeline and knowing when to end.
        void Play()
        {   
            stringLite.Append ("/");
            stringLite.Append (currentTick);
            stringLite.Append ("#");
            string tick_string = stringLite.ToString();
            stringLite.Clear();
               
            while (timelineCopy.Contains (tick_string)) //Contains confirms int pointIndex will not be out of bounds.
            {                                    
                int pointIndex = timelineCopy.IndexOf (tick_string); //returns index of first character of input string found.
                ExtractClip (pointIndex);
            }    

            if (currentTick < timelineSize)    
            { 
                currentTick++;
            } 
    
            else
            {                
                finished = true;
            }    
        }

        void ExtractClip (int pointIndex) //performance light function that detects clips ready to play. 
        { 
            bool extractedNum = false;  
            string choiceExtracted = "";                                                                   
    
            while (timelineCopy[pointIndex] != '#') //finds the sound_choice's marker.
            {
                pointIndex++;    
            }
    
            while (extractedNum == false) //add the sound_choice it finds until it reaches the marker.
            {
                pointIndex++; //ensures hash is not added to the temp clip number.
        
                if (timelineCopy[pointIndex] != '/') //an indexed string is a single char; therefore use ''.
                {
                    stringLite.Append (timelineCopy[pointIndex]);
                }
        
                else
                {
                    extractedNum = true; //while loops finish the list before exiting so i dont have to worry about this line's position.
                    choiceExtracted = stringLite.ToString();
                    stringLite.Clear();
                    pointIndex--; //readies the logic to remove the data slot.    
                }    
            }    
    
            //removes the data slot (start_point and sound_choice) but leaves the forward slashes.
            while (timelineCopy[pointIndex] != '/')
            {
                timelineCopy = timelineCopy.Remove (pointIndex, 1); //inputs index then count.
                pointIndex--;
            } 
            SoundPlayer.PlayClip (debugging, choiceExtracted);  
        }
    }
}
