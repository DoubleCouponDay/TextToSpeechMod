namespace SETextToSpeechMod
{
    public sealed class HawkingVoice : SentenceFactory
    {
        public override string Name { get { return "HawkingVoice"; } }
        public override string FileID { get { return "-H"; } }        
        public override int SpaceSize { get { return 4; } }
        public override int ClipLength { get { return 4; } }
        public override int SyllableSize { get { return 3; } }        

        protected override int[][] smallIntonationOptions { get { return smallOptions; } }
        protected override int[][] mediumIntonationOptions { get { return mediumOptions; } }
        protected override int[][] largeIntonationOptions { get { return largeOptions; } }

        readonly int[][] smallOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },           
        };

        readonly int[][] mediumOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },          
        };

        readonly int[][] largeOptions = new int[][] 
        {
            new int[] { },
            new int[] { 0, 1, 2, },          
        };
        string sentenceEndPhonemeID = "-E";

        public HawkingVoice() : base(){}

        //the point of this extended method is to play a special kind of phoneme at the end of every sentence.
        protected override string GetPhonemesIntonation()
        {            
            string choice = "";
            
            if (intonationChoice == null)
            {
                foreach (int[][] size in allPitchSizes)
                {
                    if (sentence.Length <= size[0].Length)
                    { 
                        intonationChoice = size;
                        optionsIndex = rng.Next (size.GetLength (1));
                    }
                }
            }

            if (letterIndex >= sentence.Length - SyllableSize)
            {
                choice = sentenceEndPhonemeID;
            }   
            
            else
            {
                choice = " " + intonationChoice[optionsIndex][arraysIndex];
            }                                    
            arraysIndex++;
            return choice;
        }
    }
}
