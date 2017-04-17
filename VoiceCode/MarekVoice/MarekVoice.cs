using SETextToSpeechMod.VoiceCode.MarekVoice;

namespace SETextToSpeechMod
{
    public sealed class MarekVoice : TimelineFactory
    {      
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } } 
        public override int SpaceSize { get { return 4; } }

        public MarekVoice (SoundPlayer inputEmitter) : base (inputEmitter, new MarekIntonation()){}
    }
}
