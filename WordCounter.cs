using System.Collections.Generic;

namespace SETextToSpeechMod
{
    public class WordCounter //simply a collection of data without guarded sets. easy to pass.
    {                   
        int current_word;
        int current_letter;
        string[] words;

        public WordCounter (string inputSentence)
        {
            words = inputSentence.Split (' ');
        }

        public string GetCurrentWord (int NEW_WORD, ref int placeholder)
        { 
            if (current_word < words.Length) //exception checker because thats a possible state.
            {
                if (current_letter < words[current_word].Length)
                {
                    current_letter++; //this is for the next time the method is used.
                    return words[current_word];
                }

                else
                {
                    current_word++;
                    current_letter = 0;
                    placeholder = NEW_WORD; //a space will never have a placeholder.                    
                    return " "; //empty space is a valid return.
                }
            }

            else
            {
                return ""; 
            }
        }
    }
}
