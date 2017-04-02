namespace SETextToSpeechMod
{
    public sealed class GLADOSVoice : SentenceFactory
    {        
        public override string FileID { get { return "-G"; } }
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        protected override int[][] smallIntonationPatterns { get { return smallOptions; } }
        protected override int[][] mediumIntonationPatterns { get { return mediumOptions; } }
        protected override int[][] largeIntonationPatterns { get { return largeOptions; } }

        protected override int voiceRange { get { return 11; } }

        readonly int[][] smallOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] mediumOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] largeOptions = new int[][] 
        {
            new int[] { 0, 1, 2, },          
        };

        public GLADOSVoice (SoundPlayer inputEmitter) : base (inputEmitter){}

        protected override void AddIntonations (int timelineIndex)
        {
            string intonation = " ";

            if (timeline[timelineIndex].ClipsSound != SPACE)
            {                        
                if (intonationArrayChosen == null)
                {
                    for (int u = 0; u < allSizes.Length; u++)
                    {
                        if (timeline.Count <= allSizes[u])
                        {
                            ChoosePitchPattern (u);
                        }

                        else if (u == allSizes.Length - 1)
                        {
                            ChoosePitchPattern (allSizes.Length - 1); //assuming the array is ordered from largest to smallest!
                        }
                    }
                }

                if (arraysIndex >= intonationArrayChosen.Length)
                {
                    arraysIndex = 0;
                }        
                
                if (intonationArrayChosen[arraysIndex] > voiceRange)
                {
                    intonationArrayChosen[arraysIndex] = voiceRange;
                }                
                intonation += intonationArrayChosen[arraysIndex];
                timeline[timelineIndex] = new TimelineClip (timeline[timelineIndex].StartPoint, timeline[timelineIndex].ClipsSound + intonation);
                arraysIndex++;
            }            
        }
    }
}
