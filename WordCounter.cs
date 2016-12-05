namespace SETextToSpeechMod
{
    public class WordCounter : StateResetTemplate
    {                  
        int wordsIndex;
        string[] words;

        public bool DumpRemainingLetters {get; private set;}
        public string CurrentWord {get; private set;}
        public int LetterIndex { get; private set; }

        public void FactoryReset (string inputSentence)
        {
            words = inputSentence.Split (' ');
            DumpRemainingLetters = false;                
            CurrentWord = "";
            wordsIndex = 0;         
            LetterIndex = 0;
        }
        
        //wordcounter 
        public void CheckNextLetter()
        { 
            if (wordsIndex < words.Length)
            {
                if (LetterIndex == words[wordsIndex].Length - 1)
                {
                    DumpRemainingLetters = true;
                }
                
                if (LetterIndex < words[wordsIndex].Length)
                {
                    LetterIndex++;
                    CurrentWord = words[wordsIndex];
                }

                else
                {
                    DumpRemainingLetters = false;
                    wordsIndex++;
                    LetterIndex = 0;
                    CurrentWord = " ";                  
                }
            }    
            
            else
            {
                CurrentWord = "";
            }   
        }
    }
}
