namespace SETextToSpeechMod
{
    class HawkingVoice : SentenceFactory
    {
        const int H_SPACE_SIZE = 4;
        const int H_CLIP_LENGTH = 4; 
        const int H_SYLLABLE_SIZE = 3;
        const string H_VOICE_ID = "-H";

        protected override int SpaceSize { get { return H_SPACE_SIZE; } }
        protected override int ClipLength { get { return H_CLIP_LENGTH; } }
        protected override int SyllableSize { get { return H_SYLLABLE_SIZE; } }
        protected override string VoiceID { get { return H_VOICE_ID; } }

        public HawkingVoice (string input) : base (input){}
    }
}
