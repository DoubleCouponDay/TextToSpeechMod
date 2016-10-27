namespace SETextToSpeechMod
{
    class GLADOSVoice : SentenceFactory
    {
        const int G_SPACE_SIZE = 4;
        const int G_CLIP_LENGTH = 4; 
        const int G_SYLLABLE_SIZE = 3;
        const string G_VOICE_ID = "-G";

        protected sealed override int SpaceSize { get { return G_SPACE_SIZE; } }
        protected sealed override int ClipLength { get { return G_CLIP_LENGTH; } }
        protected sealed override int SyllableSize { get { return G_SYLLABLE_SIZE; } }
        protected sealed override string VoiceID { get { return G_VOICE_ID; } }

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

        public GLADOSVoice (string input) : base (input){}
    }
}
