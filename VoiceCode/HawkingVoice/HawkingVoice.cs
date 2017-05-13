using System;
using SETextToSpeechMod.VoiceCode.HawkingVoice;

namespace SETextToSpeechMod
﻿namespace SETextToSpeechMod.VoiceCode.HawkingVoice
{
    public sealed class HawkingVoice : TimelineFactory
    {     
        public HawkingVoice (SoundPlayer inputEmitter) : base (inputEmitter, new HawkingIntonation()){}

        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }  
        
        protected HawkingIntonation intonation = new HawkingIntonation();

        public HawkingVoice (SoundPlayer inputEmitter) : base (inputEmitter, this.intonation)
        {
            
        }

        //the point of extending this method is to create a special kind of phoneme at the end of every sentence.
        void AddIntonations (int timelineIndex)
        {       
            TimelineClip previousClip = timelinesField[timelineIndex];
            string updatedAddress = previousClip.ClipsSound;
            timelinesField[timelineIndex] = new TimelineClip (previousClip.StartPoint, previousClip.ClipsSound);
        }
    }
}
