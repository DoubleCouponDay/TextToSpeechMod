namespace SETextToSpeechMod
{
    public sealed class GLADOSVoice : SentenceFactory, VoiceTemplate
    {        
        public override string FileID { get { return "-G"; } }
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        protected override int[][] smallIntonationPatterns { get { return smallOptions; } }
        protected override int[][] mediumIntonationPatterns { get { return mediumOptions; } }
        protected override int[][] largeIntonationPatterns { get { return largeOptions; } }

        protected override int voiceRange { get { return 11; } }

        readonly int[][] smallOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] mediumOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] largeOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        public GLADOSVoice (SoundPlayer inputEmitter) : base (inputEmitter){}
    }
}
