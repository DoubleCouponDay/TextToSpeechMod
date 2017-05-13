namespace SETextToSpeechMod.VoiceCode.HawkingVoice
{
    public sealed class HawkingVoice : SentenceFactory, VoiceTemplate
    {
        const string NORMAL_PHONEME = " 0";
        const string SENTENCE_END_ID = "-E";
        const string QUESTION_PHONEME_ID = "-Q";
        const string EXCLAMATION_PHONEME_ID = "-!";
        const string COMMA_PHONEME_ID = "-,";

        public override string FileID { get { return "-H"; } }        
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }  
        
        protected HawkingIntonation intonation = new HawkingIntonation();

        public HawkingVoice (SoundPlayer inputEmitter) : base (inputEmitter, this.intonation)
        {
            
        }

        //the point of extending this method is to create a special kind of phoneme at the end of every sentence.
        protected override void AddIntonations (int timelineIndex)
        {       
            TimelineClip previousClip = timeline[timelineIndex];
            string updatedAddress = previousClip.ClipsSound;

            timeline[timelineIndex] = new TimelineClip (previousClip.StartPoint, previousClip.ClipsSound);
        }
    }
}
