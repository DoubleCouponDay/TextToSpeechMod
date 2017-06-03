namespace SETextToSpeechMod
{
    public class WordIsolator
    {                  
        const string EMPTY = "";
        const char SPACE = ' ';
        public const int NEW_WORD = 0;
        
        public string CurrentWord {get; private set;}
        public bool CurrentWordIsNew {get; private set;}
        public int LettersLeftInWord {get; private set;} //all words have at least one letter therefore LettersLeftInWord can never be zero.
        public int WordsIndexLimit {get; private set;}

        string tempSentence;
        int tempLetterIndex;

        public void FactoryReset()
        {
            CurrentWord = EMPTY;
            CurrentWordIsNew = false; 
            LettersLeftInWord = NEW_WORD;       
            
            tempSentence = EMPTY;
            tempLetterIndex = NEW_WORD;                             
        }

        public void UpdateProperties (string sentence, int letterIndex)
        { 
            tempSentence = sentence;
            tempLetterIndex = letterIndex;

            string wordBuild = "";
            int indexWorkingCopy = tempLetterIndex;

            if (sentence[letterIndex] != SPACE)
            {
                while (indexWorkingCopy > 0 &&
                       tempSentence[indexWorkingCopy - 1] != ' ')
                {
                    indexWorkingCopy--; //locates the start of the word.
                }

                while (indexWorkingCopy < tempSentence.Length &&
                       tempSentence[indexWorkingCopy] != ' ')
                {
                    wordBuild += tempSentence[indexWorkingCopy];
                    indexWorkingCopy++; //locates the end of the word.
                }

                if (wordBuild == CurrentWord)
                {
                    CurrentWordIsNew = false;
                    LettersLeftInWord--;
                }

                else
                {
                    CurrentWordIsNew = true;
                    LettersLeftInWord = wordBuild.Length;
                    WordsIndexLimit = wordBuild.Length - 1;
                }
                CurrentWord = wordBuild;
            }

            else
            {
                LettersLeftInWord = NEW_WORD;
                WordsIndexLimit = NEW_WORD;
                CurrentWord = SPACE.ToString();
            }
        }
    }
}
