namespace SETextToSpeechMod
{
    public sealed class GLADOSVoice : SentenceFactory
    {        
        public override string Name { get { return "GLADOSVoice"; } }
        public override string FileID { get { return "-G"; } }
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        protected override int[][] smallIntonationOptions { get { return smallOptions; } }
        protected override int[][] mediumIntonationOptions { get { return mediumOptions; } }
        protected override int[][] largeIntonationOptions { get { return largeOptions; } }

        readonly int[][] smallOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] mediumOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] largeOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },          
        };

        public GLADOSVoice() : base(){}
    }
}
