using SETextToSpeechMod.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SETextToSpeechMod.Output
{
    public class SpeechTask : IDisposable
    {
        public bool IsDisposed
        {
            get; private set;
        }

        /// <summary>
        /// Generating Object which is the primary focus of a SpeechTask.
        /// </summary>
        public TimelineFactory Worker { get; private set;}
        public Task ReturnInfo { get; private set;}

        /// <summary>
        /// This is an unmanaged object type and, when finished with, must be explicitly disposed using SpeechTask.Dispose()
        /// Same deal if you cancel this token once. One instance per cancel call and dispose before reinstantiating.
        /// </summary>
        public CancellationTokenSource TaskCanceller { get; private set;}

        public SpeechTask (TimelineFactory itsSpeech)
        {
            Worker = itsSpeech;
            ReturnInfo = new Task (() => {return;}); 
            TaskCanceller = new CancellationTokenSource();
        }

        public void Run()
        {
            ReturnInfo = Worker.RunAsync();
        }

        public void FactoryReset (Sentence inputSentence)
        {
            TaskCanceller.Cancel();
            RenewCancellationSource();
            Worker.FactoryReset (inputSentence); //reuses instances of sentencefactory instead of instantiating every new sentence.                
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
            if(IsDisposed == false)
            {
                IsDisposed = true;
                TaskCanceller.Cancel();
                TaskCanceller.Dispose();
            }
        }
    }
}
