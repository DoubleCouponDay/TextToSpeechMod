namespace SETextToSpeechMod
{
    public class WordCounter //simply a collection of data without guarded sets. easy to pass.
    {                  
        const int NEW_WORD = -1; 
        const int LAST_LETTER = -3;
        int currentWordInt;
        int currentLetter;
        string[] words;
        public bool dumpRemainingLetters {get; private set;}
        public int placeholder {get; private set;}
        public string currentWord {get; private set;}

        public WordCounter (string inputSentence)
        {
            currentWordInt = 0;
            currentLetter = 0;        
            words = inputSentence.Split (' ');
        }


        //chops the sentence into words and stores its results.
        public void IncrementCurrentPosition (int placeholderInput)
        { 
            if (currentWordInt < words.Length)
            {
                if (currentLetter == words[currentWordInt].Length - 1)
                {
                    currentLetter++;
                    dumpRemainingLetters = true;
                    currentWord = words[currentWordInt];
                }
                
                else if (currentLetter < words[currentWordInt].Length)
                {
                    currentLetter++;
                    currentWord = words[currentWordInt];
                }

                else
                {
                    dumpRemainingLetters = false;
                    currentWordInt++;
                    currentLetter = 0;
                    placeholderInput = NEW_WORD; //a space will never have a placeholder.  
                    currentWord = " ";                  
                }
            }    
            
            else
            {
                currentWord = "";
            }   
            placeholder = placeholderInput;
        }

        //resets a certain variable when the sentence ends.
        public int CheckForEnd (int placeholder, int NEW_WORD)
        {
            if (currentWordInt == words.Length - 1 &&
                currentLetter == words[currentWordInt].Length) //resets the placeholder since script doesnt construct a new class for every sentence.
            { 
                placeholder = NEW_WORD;
            }
            return placeholder;
        }
    }
}
