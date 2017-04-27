using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Output
{
    public class SpeechTask
    {
        /// <summary>
        /// Generation Object which is the primary focus of a SpeechTask.
        /// </summary>
        public TimelineFactory MainProcess { get; private set;}

        public Task ReturnInfo { get; private set;}

        /// <summary>
        /// This is an unmanaged object type and, when finished with, must be explicitly disposed using SpeechTask.Dispose()
        /// Same deal if you cancel this token once. One instance per cancel call and dispose before reinstantiating.
        /// </summary>
        public CancellationTokenSource TaskCanceller { get; private set;}

        public SpeechTask (TimelineFactory itsSpeech)
        {
            MainProcess = itsSpeech;
            ReturnInfo = new Task (() => {return;}); //just in case the default value of IsCompleted is not true.
            ReturnInfo.RunSynchronously();
            TaskCanceller = new CancellationTokenSource();
        }

        public async Task RunAsync()
        {
            ReturnInfo = MainProcess.RunAsync();
            await ReturnInfo;
        }

        /// <summary>
        /// Disposes TaskCanceller property and instatiates a new one; ready for a fresh cancellation.
        /// </summary>
        public void RenewCancellationSource()
        {
            Dispose();
            TaskCanceller = new CancellationTokenSource();
        }
       
        public void Dispose()
        {
            TaskCanceller.Cancel();
            TaskCanceller.Dispose();
        }
    }
}
