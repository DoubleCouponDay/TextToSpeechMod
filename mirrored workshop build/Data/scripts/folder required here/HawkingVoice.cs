namespace SETextToSpeechMod
{
    public sealed class HawkingVoice : SentenceFactory, VoiceTemplate
    {
        public override string FileID { get { return "-H"; } }        
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
            new int[] { 0, 1, 2, },
            new int[] { 0, 1, 2, },
            new int[] { 0, 1, 2, },
        };
        string sentenceEndPhonemeID = "-E";

        public HawkingVoice (SoundPlayer inputEmitter) : base (inputEmitter){}

        //the point of extending this method is to create a special kind of phoneme at the end of every sentence.
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
                                    
                if (timelineIndex >= timeline.Count - SyllableSize)
                {
                    intonation = sentenceEndPhonemeID; //hawking ending phonemes dont have a space between.
                }   
            
                else
                {
                    if (intonationArrayChosen[arraysIndex] > voiceRange)
                    {
                        intonationArrayChosen[arraysIndex] = voiceRange;
                    }                
                    intonation += intonationArrayChosen[arraysIndex];
                }                
                timeline[timelineIndex] = new TimelineClip (timeline[timelineIndex].StartPoint, timeline[timelineIndex].ClipsSound + intonation);
                arraysIndex++;
            }                          
        }
    }
}
