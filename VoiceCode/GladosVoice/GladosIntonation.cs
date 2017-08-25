using SETextToSpeechMod;
using SETextToSpeechMod.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SETextToSpeechMod.VoiceCode.GladosVoice
{
    class GladosIntonation : Intonation
    {
        Random rng = new Random();
        const string SPACE = " ";

        int smallSize
        {
            get
            {
                return 6;
            }
        }

        int mediumSize
        {
            get
            {
                return 50;
            }
        }

        int largeSize
        {
            get
            {
                return OutputManager.MAX_LETTERS;
            }
        }

        int[] allSizes;

        protected virtual int[][] smallIntonationPatterns
        {
            get;
        } 
        
        //an intonation pattern should be designed to loop on itself. it can be any size.
        int[][] mediumIntonationPatterns
        {
            get;
        }

        int[][] largeIntonationPatterns
        {
            get;
        }

        int[][][] allPitchOptions;

        int voiceRange
        {
            get
            {
                return 11;
            }
        }

        public override string VoiceId
        {
            get
            {
                return "-G";
            }
        }

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
        int[] intonationArrayChosen;

        int[] ChoosePitchPattern(int sizeIndex)
        {
            for (int u = 0; u < allSizes.Length; u++)
            {
                //if (timeline.Count <= allSizes[u])
                //{
                //    //ChoosePitchPattern (u);
                //}

                //else if (u == allSizes.Length - 1)
                //{
                //    //ChoosePitchPattern (allSizes.Length - 1); //assuming the array is ordered from largest to smallest!
                //}
            }

            int currentLimit = allPitchOptions[sizeIndex].Length;
            int randomPattern = rng.Next(currentLimit);
            int[] intonationArray = allPitchOptions[sizeIndex][randomPattern];
            return intonationArray;
        }

        protected override string DerivedIntonationChoice(string phoneme, string surroundingPhrase, bool sentenceEndInPhrase)
        {
            string choice = VoiceId + SPACE;

            //if (phoneme != SPACE)
            //{
            //    if (intonationArrayChosen == null)
            //    {

            //    }

            //    else
            //    {
            //        ChoosePitchPattern();
            //    }

            //    if (arraysIndex >= intonationArrayChosen.Length)
            //    {
            //        arraysIndex = 0;
            //    }

            //    if (intonationArrayChosen[arraysIndex] > voiceRange)
            //    {
            //        intonationArrayChosen[arraysIndex] = voiceRange;
            //    }
            //    intonation += intonationArrayChosen[arraysIndex];
            //    timeline[timelineIndex] = new TimelineClip(timeline[timelineIndex].StartPoint, timeline[timelineIndex].ClipsSound + intonation);
            //    arraysIndex++;
            //}  
            return choice;
        }
    }
}
