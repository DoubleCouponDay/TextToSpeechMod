using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    public abstract class Intonation : VoiceTemplate
    {
        protected StringBuilder concatLite = new StringBuilder();

        public abstract int ClipLength { get; }
        public abstract string VoiceId { get; }
        public abstract int SpaceSize { get; }
        public abstract int SyllableSize { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneme"></param>
        /// <param name="surroundingPhrase">Must be the same length as Pronunciation.ALGORITHM_PHRASE_SIZE</param>
        /// <param name="sentenceEndInPhrase">signals that the phrase contains the end of the sentence being processed.</param>
        /// <returns></returns>
        public string GetPhonemesIntonation (string phoneme, string surroundingPhrase, bool sentenceEndInPhrase)
        {
            if (surroundingPhrase.Length == Pronunciation.ALGORITHM_PHRASE_SIZE)
            {
                return DerivedIntonationChoice (phoneme, surroundingPhrase, sentenceEndInPhrase);
            }

            else
            {
                throw new UnconventionalPhraseException ("surroundingPhrase must be " + Pronunciation.ALGORITHM_PHRASE_SIZE + " characters long.");
            }
        }

        protected abstract string DerivedIntonationChoice (string phoneme, string surroundingPhrase, bool sentenceEndInPhrase);
    }
}
