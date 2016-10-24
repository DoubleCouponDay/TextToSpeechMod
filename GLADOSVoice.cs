namespace SETextToSpeechMod
{
    class GLADOSVoice : SentenceFactory
    {
        const int G_SPACE_SIZE = 4;
        const int G_CLIP_LENGTH = 4; 
        const int G_SYLLABLE_SIZE = 3;
        const string G_VOICE_ID = "-G";

        protected override int SpaceSize { get { return G_SPACE_SIZE; } }
        protected override int ClipLength { get { return G_CLIP_LENGTH; } }
        protected override int SyllableSize { get { return G_SYLLABLE_SIZE; } }
        protected override string VoiceID { get { return G_VOICE_ID; } }

        public GLADOSVoice (string input) : base (input){}
    }
}
