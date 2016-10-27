namespace SETextToSpeechMod
{
    class MarekVoice : SentenceFactory
    {
        const int M_SPACE_SIZE = 4;
        const int M_CLIP_LENGTH = 4; 
        const int M_SYLLABLE_SIZE = 3;
        const string M_VOICE_ID = "-M";

        protected override int SpaceSize { get { return M_SPACE_SIZE; } }
        protected override int ClipLength { get { return M_CLIP_LENGTH; } }
        protected override int SyllableSize { get { return M_SYLLABLE_SIZE; } }
        protected override string VoiceID { get { return M_VOICE_ID; } }

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
