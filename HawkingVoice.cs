namespace SETextToSpeechMod
{
    public sealed class HawkingVoice : SentenceFactory
    {
        public override string FileID { get { return "-H"; } }        
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        protected override int[][] smallIntonationPatterns { get { return smallOptions; } }
        protected override int[][] mediumIntonationPatterns { get { return mediumOptions; } }
        protected override int[][] largeIntonationPatterns { get { return largeOptions; } }

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
        string sentenceEndPhonemeID = "-E";
        int hawkingPitchRange = 11;

        public HawkingVoice() : base(){}

        //the point of extending this method is to create a special kind of phoneme at the end of every sentence.
        protected override void AddIntonations (int timelineIndex)
        {       
            string intonation = " ";
            
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
                                    
            if (letterIndex >= sentence.Length - SyllableSize)
            {
                intonation = sentenceEndPhonemeID; //hawking ending phonemes dont have a space between.
            }   
            
            else
            {
                intonation += intonationArrayChosen[arraysIndex];
            }                
            timeline[timelineIndex] = new TimelineClip (timeline[timelineIndex].StartPoint, timeline[timelineIndex].ClipsSound + intonation);
            arraysIndex++;                                   
        }
    }
}
