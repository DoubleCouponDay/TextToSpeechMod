using SETextToSpeechMod.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.VoiceCode.MarekVoice
{
    class MarekIntonation : Intonation
    {
        public override string VoiceId { get { return "-M"; } }                

        protected override string DerivedIntonationChoice (string phoneme, string surroundingPhrase, bool sentenceEndInPhrase)
        {
            return phoneme + VoiceId;
        }
    }
}
