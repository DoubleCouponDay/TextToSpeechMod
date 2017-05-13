using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    class UnconventionalPhraseException : Exception
    {
        public UnconventionalPhraseException (string comment) : base (comment) { }
    }
}
