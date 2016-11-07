namespace SETextToSpeechMod
{
    sealed class HawkingVoice : SentenceFactory
    {
        const int H_SPACE_SIZE = 4;
        const int H_CLIP_LENGTH = 4; 
        const int H_SYLLABLE_SIZE = 3;
        const string H_VOICE_ID = "-H";

        protected override int SpaceSize { get { return 4; } }
        protected override int ClipLength { get { return 4; } }
        protected override int SyllableSize { get { return 3; } }
        protected override string VoiceID { get { return "-H"; } }

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

        public HawkingVoice (string input) : base (input){}
    }
}
