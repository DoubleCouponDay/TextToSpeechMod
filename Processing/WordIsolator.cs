using SETextToSpeechMod.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SETextToSpeechMod
{
    public class WordIsolator:IEnumerator<string>, ISentenceReset
    {
        const string EMPTY = ""; //cant be set to string.Empty because it must be a constant; not readonly.
        const char SPACE = ' ';
        public const int WORD_IS_SPACE = 0;

        public bool CurrentWordIsNew
        {
            get; private set;
        }
        public int LettersLeftInWord
        {
            get; private set;
        } //all words have at least one letter therefore LettersLeftInWord can never be zero.
        public int WordsIndexLimit
        {
            get; private set;
        }

        /// <summary>
        /// Default state is WordIsolator.EMPTY. MoveNext() to update.
        /// </summary>
        public string Current => currentWord;
        object IEnumerator.Current => currentWord;

        Sentence tempSentence;
        int tempLetterIndex;
        private string currentWord;

        public void FactoryReset(Sentence newSentence)
        {
            currentWord = EMPTY;
            CurrentWordIsNew = false;
            LettersLeftInWord = newSentence.Length;
            WordsIndexLimit = 0;

            tempSentence = newSentence;
            tempLetterIndex = WORD_IS_SPACE;
        }

        /// <summary>
        /// not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">throws</exception>
        [Obsolete]
        public void Dispose()
        {
            throw new NotImplementedException("not implemented.");
        }

        /// <summary>
        /// Moves to to the next char index and updates properties based on the new position, in the sentence.
        /// Returns bool indicating whether end of sentence was reached
        /// </summary>
        public bool MoveNext()
        {
            bool didMove = false;

            if(tempSentence.Length < tempLetterIndex)
            {
                if(tempSentence[tempLetterIndex] != SPACE)
                {
                    if(CurrentWordIsNew)
                    {
                        ModifyStateAsNewWord();
                    }

                    else
                    {
                        ModifyStateAsOldWord();
                    }
                }

                else
                {
                    ModifyStateAsSpace();
                }
                tempLetterIndex++; //expected to initiate the next state when GetNext() is called again.
                LettersLeftInWord--;
                didMove = true;
            }

            else
            {
                ModifyStateAsEnd();
            }
            return didMove;
        }

        private void ModifyStateAsSpace()
        {
            CurrentWordIsNew = true;
            LettersLeftInWord = WORD_IS_SPACE;
            WordsIndexLimit = WORD_IS_SPACE;
            currentWord = SPACE.ToString();
        }

        private void ModifyStateAsNewWord()
        {
            if(currentWord != SPACE.ToString())
            {
                int workingIndex = tempLetterIndex;
                string wordBuild = string.Empty;

                while(workingIndex < tempSentence.Length && //prevents outofbounds
                        tempSentence[workingIndex] != SPACE)
                {
                    wordBuild += tempSentence[workingIndex];
                    workingIndex++;
                }
                currentWord = wordBuild;
            }
            CurrentWordIsNew = false;
            LettersLeftInWord = currentWord.Length;
            WordsIndexLimit = tempLetterIndex + LettersLeftInWord;
        }

        private void ModifyStateAsOldWord()
        {
            LettersLeftInWord--;
        }

        private void ModifyStateAsEnd()
        {
            currentWord = EMPTY;
        }

        /// <summary>
        /// Takes the enumerator to the first item.
        /// </summary>
        public void Reset()
        {
            FactoryReset(tempSentence);
        }
    }
}
