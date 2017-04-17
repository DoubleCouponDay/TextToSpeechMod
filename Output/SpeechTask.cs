using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Output
{
    internal class SpeechTask
    {
        public TimelineFactory Speech { get; private set;}
        public Task ReturnInfo { get; private set;}

        /// <summary>
        /// This is an unmanaged object type and, when finished with, must be explicitly disposed using CancellationTokenSource.Dispose().
        /// Same deal if you cancel this source once. One instance per cancel call and dispose before reinstantiating.
        /// </summary>
        public CancellationTokenSource TaskCanceller { get; private set;}

        public SpeechTask (TimelineFactory itsSpeech)
        {
            Speech = itsSpeech;
            ReturnInfo = new Task (() => {return;}); //just in case the default value of IsCompleted is not true.
            ReturnInfo.RunSynchronously();
            TaskCanceller = new CancellationTokenSource();
        }

        public async Task RunAsync()
        {
            ReturnInfo = Speech.RunAsync();
            await ReturnInfo;
        }
    }
}
