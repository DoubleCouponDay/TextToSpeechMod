namespace SETextToSpeechMod
{
    public sealed class MarekVoice : SentenceFactory
    {
        public override string FileID { get { return "-M"; } }
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        public MarekVoice (SoundPlayer inputEmitter) : base (inputEmitter){}

        protected override void IterateIntonationsOnTimeLine() { }
    }
}
