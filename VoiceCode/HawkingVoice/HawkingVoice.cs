using SETextToSpeechMod.VoiceCode.HawkingVoice;

namespace SETextToSpeechMod
{
    public sealed class HawkingVoice : SentenceFactory
    {     
        public HawkingVoice (SoundPlayer inputEmitter) : base (inputEmitter, new HawkingIntonation()){}

        //the point of extending this method is to create a special kind of phoneme at the end of every sentence.
        void AddIntonations (int timelineIndex)
        {       
            TimelineClip previousClip = timeline[timelineIndex];
            string updatedAddress = previousClip.ClipsSound;

            timeline[timelineIndex] = new TimelineClip (previousClip.StartPoint, previousClip.ClipsSound);
        }
    }
}
