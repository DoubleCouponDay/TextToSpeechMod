namespace SETextToSpeechMod
{
    public sealed class MarekVoice : SentenceFactory, VoiceTemplate
    {
        public override string FileID { get { return "-M"; } }
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        public MarekVoice() : base(){}

        protected override void AddIntonations (int timelineIndex) { }
    }
}
