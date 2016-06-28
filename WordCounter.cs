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
            current_word = 0;
            current_letter = 0;        
            words = inputSentence.Split (' ');
        }

        public string GetCurrentWord (int NEW_WORD, ref int placeholder)
        { 
            if (current_word < words.Length)
            {
                if (current_letter < words[current_word].Length)
                {
                    current_letter++;
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

        public int CheckForEnd (int placeholder, int NEW_WORD)
        {
            if (current_word == words.Length - 1 &&
                current_letter == words[current_word].Length) //resets the placeholder since script doesnt construct a new class for every sentence.
            { 
                placeholder = NEW_WORD;
            }
            return placeholder;
        }
    }
}
