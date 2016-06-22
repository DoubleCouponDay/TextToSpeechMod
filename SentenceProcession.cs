using System;
using System.Collections.Generic;
using System.Text;

namespace SETextToSpeechMod
{   
    class SentenceProcession //the roman numeral class name may help you to understand the code flow.
    {
        //reference settings         
        const int SPACE_SIZE = 8;
        const int SPACE_ADDON = 3;
        const int CLIP_LENGTH = 4; 
        const int SYLLABLE_SIZE = 3;
        const string SOUND_ID = "-M";
        const int chanceOfClang = 10000;

        //state
        bool loading = true;
        public bool finished { get; private set; }

        //indexs
        int letterIndex;
        int clip_index;

        //play data
        string timeline;
        string timeline_copy;
        int timeline_size;
        int current_tick;

        //miscellaneous
        int syllable_measure = 1; //a measure of how far along each syllable is.
        string sentence;

        //objects
        List <CreateClip> clips = new List <CreateClip>(); //total clips for the sentence.
        StringBuilder string_lite = new StringBuilder(); //the quickest way of pasting strings together. must be cleared to prepare for the next use.        
        Random generator = new Random();
        Pronunciation pronunciation;

        //------------------------------------------------------------------------------------------------------------------------------//
        public SentenceProcession (string input)
        {
            sentence = input.Remove (0, 2); //getting rid of the trigger since its unnecessary. while testing, words smaller than the count of 2 will throw therefore...     
            pronunciation = new Pronunciation (sentence);
        }

        //this function will extract what phonemes it can from the sentence and save performance by taking its sweet time.
        public void Load()
        {    
            if (loading == true)
            {   
                if (letterIndex < sentence.Length)
                {    
                    AddPhoneme(); 
                    letterIndex++;
                }   
        
                else if (clip_index < clips.Count) //im getting a string of all the start points to help performance.                    
                {           
                    string_lite.Append ("/"); //leaving out clear this one time is ok since it clears on the next else block.
                    string_lite.Append (clips[clip_index].start_point); 
                    string_lite.Append ("#"); 
                    string_lite.Append (clips[clip_index].clips_sound);         
                    clip_index++;
                } 

                else
                {
                    loading = false;    
                    string_lite.Append ("/"); //the cap on the timeline mix.
                    timeline = string_lite.ToString ();
                    string_lite.Clear ();
                    timeline_copy = timeline;
                }        
            }

            else
            {
                int rng_bonk = generator.Next (chanceOfClang);

                if (rng_bonk == 42)
                {
                    SoundPlayer.PlayClip ("BONK", true); //it hurts to live
                }
                Play();    
            }    
        } 

        //creates a new clip for the current letter.
        private void AddPhoneme()
        {   
            string secondaryPhoneme;
            string primaryPhoneme = pronunciation.GetLettersPronunciation (sentence, letterIndex, out secondaryPhoneme);
            string[] add_results = {primaryPhoneme, secondaryPhoneme};

            for (int i = 0; i < add_results.Length; i++)
            {
                if (add_results[i] != "")
                {
                    if (add_results[i] != " ") //empty string lets my program know that no clip should be created.
                    {                                                                
                        int start_point = timeline_size;         
                        string sound_choice = add_results[i] + SOUND_ID;
                        clips.Add (new CreateClip (start_point, sound_choice)); //add the key transitions into a new object.
                        timeline_size += CLIP_LENGTH; //timeline is expanded for duration after the clip is created.

                        if (syllable_measure == SYLLABLE_SIZE) //cues a space using the current setting SYLLABLE_SIZE.
                        {
                            IncrementSyllables (i);
                        }   
                        
                        else
                        {
                            syllable_measure++;
                        }
                    }

                    else
                    {
                        IncrementSyllables (i);
                    }
                }
            }
        }

        void IncrementSyllables (int i)
        {
            if (i == 1) //spaces between words need to be almost double as large as a syllable space.
            {
                timeline_size += SPACE_ADDON;
            }

            else
            {
                timeline_size += SPACE_SIZE;
            }            
            syllable_measure = 1;
        }

        class CreateClip //template for sound clips; the building blocks of a speech timeline.
        {
            public int start_point {get; set;}
            public string clips_sound {get; set;} 
  
            public CreateClip (int one, string two) //constructor
            {
                this.start_point = one;
                this.clips_sound = two;   
            }
        }

        //this function is in charge of finding clips on the timeline and knowing when to end.
        void Play()
        {   
            string_lite.Append ("/");
            string_lite.Append (current_tick);
            string_lite.Append ("#");
            string tick_string = string_lite.ToString ();
            string_lite.Clear ();
               
            while (timeline_copy.Contains (tick_string)) //Contains confirms int point_index will not be out of bounds.
            {                                    
                int point_index = timeline_copy.IndexOf (tick_string); //returns index of first character of input string found.
                ExtractClip (point_index);
            }    

            if (current_tick < timeline_size)    
            { 
                current_tick++;
            } 
    
            else
            {                
                finished = true;
            }    
        }

        void ExtractClip (int point_index) //performance light function that detects clips ready to play. 
        { 
            bool extracted_num = false;  
            string choice_extracted = "";                                                                   
    
            while (timeline_copy[point_index] != '#') //finds the sound_choice's marker.
            {
                point_index++;    
            }
    
            while (extracted_num == false) //add the sound_choice it finds until it reaches the marker.
            {
                point_index++; //ensures hash is not added to the temp clip number.
        
                if (timeline_copy[point_index] != '/') //an indexed string is a single char; therefore use ''.
                {
                    string_lite.Append (timeline_copy[point_index]);
                }
        
                else
                {
                    extracted_num = true; //while loops finish the list before exiting so i dont have to worry about this line's position.
                    choice_extracted = string_lite.ToString ();
                    string_lite.Clear ();
                    point_index--; //readies the logic to remove the data slot.    
                }    
            }    
    
            //removes the data slot (start_point and sound_choice) but leaves the forward slashes.
            while (timeline_copy[point_index] != '/')
            {
                timeline_copy = timeline_copy.Remove (point_index, 1); //inputs index then count.
                point_index--;
            } 
            SoundPlayer.PlayClip (choice_extracted, false);
        }
    }
}
