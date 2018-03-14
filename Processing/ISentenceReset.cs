using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    interface ISentenceReset
    {
        void FactoryReset(Sentence newSentence);
    }
}
