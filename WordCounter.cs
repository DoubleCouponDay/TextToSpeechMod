using System.Collections.Generic;

namespace SETextToSpeechMod
{
    public class WordCounter //simply a collection of data without guarded sets. easy to pass.
    {                  
        const int NEW_WORD = -1; 
        const int LAST_LETTER = -3;
        int currentWord;
        int currentLetter;
        string[] words;

        public WordCounter (string inputSentence)
        {
            this.currentWord = 0;
            this.currentLetter = 0;        
            this.words = inputSentence.Split (' ');
        }

        public string AnalyseCurrentPosition (ref int placeholder, ref bool DumpRemainingLetters)
        { 
            if (currentWord < words.Length)
            {
                if (currentLetter == words[currentWord].Length - 1)
                {
                    DumpRemainingLetters = true;
                    return words[currentWord];
                }

                else if (currentLetter < words[currentWord].Length)
                {
                    currentLetter++;
                    return words[currentWord];
                }

                else
                {
                    currentWord++;
                    currentLetter = 0;
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
            if (currentWord == words.Length - 1 &&
                currentLetter == words[currentWord].Length) //resets the placeholder since script doesnt construct a new class for every sentence.
            { 
                placeholder = NEW_WORD;
            }
            return placeholder;
        }
    }
}
