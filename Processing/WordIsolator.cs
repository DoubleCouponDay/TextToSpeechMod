using SETextToSpeechMod.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SETextToSpeechMod
{
    public class WordIsolator : IEnumerator<string>, ISentenceReset
    {
        public const int WORD_IS_SPACE = 0;

        public bool CurrentWordIsNew
        {
            get; private set;
        }

        public int LettersLeftInWord //all words have at least one letter therefore LettersLeftInWord can never be zero.
        {
            get; private set;
        } 

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
            currentWord = Constants.EMPTY;
            CurrentWordIsNew = false;
            LettersLeftInWord = newSentence.Length;
            WordsIndexLimit = 0;

            tempSentence = newSentence;
            tempLetterIndex = WORD_IS_SPACE;
        }

        /// <summary>
        /// do not use. Not implemented.
        /// </summary>
        /// <exception cref="NotImplementedException">throws</exception>
        public void Dispose()
        {
            throw new NotImplementedException(" dispose not implemented.");
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
                if(tempSentence[tempLetterIndex] != Constants.SPACE)
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
            currentWord = Constants.SPACE.ToString();
        }

        private void ModifyStateAsNewWord()
        {
            if(currentWord != Constants.SPACE.ToString())
            {
                int workingIndex = tempLetterIndex;
                string wordBuild = string.Empty;

                while(workingIndex < tempSentence.Length && //prevents outofbounds
                        tempSentence[workingIndex] != Constants.SPACE)
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
            currentWord = Constants.EMPTY;
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
