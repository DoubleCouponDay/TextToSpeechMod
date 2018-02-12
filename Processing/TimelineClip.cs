using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Processing
{
    public struct TimelineClip
    {
        public int StartPoint { get; }
        public string ClipsSound { get; }

        internal TimelineClip (int inputPoint, string inputSound)
        {
            this.StartPoint = inputPoint;
            this.ClipsSound = inputSound;
        }
    } 
}
