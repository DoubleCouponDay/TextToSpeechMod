using SETextToSpeechMod.Processing;
using SETextToSpeechMod.VoiceCode.GladosVoice;

namespace SETextToSpeechMod
{
    public sealed class GladosVoice : SentenceFactory
    {
        public GladosVoice (SoundPlayer inputEmitter) : base(inputEmitter, new GladosIntonation()) { }
    } 
}
