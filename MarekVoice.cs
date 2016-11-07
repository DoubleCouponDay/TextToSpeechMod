namespace SETextToSpeechMod
{
    sealed class MarekVoice : SentenceFactory
    {
        protected override int SpaceSize { get { return 4; } }
        protected override int ClipLength { get { return 4; } }
        protected override int SyllableSize { get { return 3; } }
        protected override string VoiceID { get { return "-M"; } }

        protected override int[][] IntonationOptions
        {
            get
            {
                return new int[][] 
                {
                    new int[] { },
                    new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, },
                };
            }
        }

        public MarekVoice (string input) : base (input){}
    }
}
