using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    public struct Sentence
    {
        public char this[int index]
        {
            get => sentenceValue[index];
        }
        private string sentenceValue;

        public int Length => sentenceValue.Length;

        public Sentence (string sentenceValue)
        {
            this.sentenceValue = sentenceValue;
        }

        public override string ToString()
        {
            return sentenceValue;
        }        
    }
}
