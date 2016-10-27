namespace SETextToSpeechMod
{
    class HawkingVoice : SentenceFactory
    {
        const int H_SPACE_SIZE = 4;
        const int H_CLIP_LENGTH = 4; 
        const int H_SYLLABLE_SIZE = 3;
        const string H_VOICE_ID = "-H";

        protected sealed override int SpaceSize { get { return H_SPACE_SIZE; } }
        protected sealed override int ClipLength { get { return H_CLIP_LENGTH; } }
        protected sealed override int SyllableSize { get { return H_SYLLABLE_SIZE; } }
        protected sealed override string VoiceID { get { return H_VOICE_ID; } }

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
