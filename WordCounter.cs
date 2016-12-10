namespace SETextToSpeechMod
{
    public class WordCounter : StateResetTemplate
    {                  
        public const string EMPTY = "";
        public const string SPACE = " ";
        public const int NEW_WORD = 0;

        public bool DumpRemainingLetters {get; private set;}
        public string CurrentWord {get; private set;}
        public int LetterIndex { get; private set; }

        int wordsIndex;
        string[] words;
        bool giveNewWordGracePeriod;

        public void FactoryReset (string inputSentence)
        {
            words = inputSentence.Split (' ');
            DumpRemainingLetters = false;                
            giveNewWordGracePeriod = true;
            wordsIndex = NEW_WORD;         
            LetterIndex = NEW_WORD;            

            if (words[0] == EMPTY)
            {
                CurrentWord = SPACE; //in case the sentence cant be split.
            }

            else
            {
                CurrentWord = words[wordsIndex];
            }            
        }

        public void IncrementToNextLetter()
        { 
            if (wordsIndex < words.Length)
            {
                if (LetterIndex == words[wordsIndex].Length - 1)
                {
                    DumpRemainingLetters = true;
                }
                
                if (LetterIndex < words[wordsIndex].Length)
                {
                    switch (giveNewWordGracePeriod)
                    {
                        case false:
                            LetterIndex++;                            
                            break;

                        case true:
                            giveNewWordGracePeriod = false;
                            break;
                    }         
                    CurrentWord = words[wordsIndex];      
                }

                else
                {
                    LetterIndex = NEW_WORD;                                                   
                    CurrentWord = SPACE;   
                    DumpRemainingLetters = false;   
                    giveNewWordGracePeriod = true;               
                    wordsIndex++;      
                }
            }     
        }
    }
}
