using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Threading.Utilities
{
    public class Sequencer
    {
        private bool stopped = false;
        private bool hasRequestsToProcess => true;

        private Queue<int> queue; // This simulates the underlying database
        public Action<Queue<int>> queueChangedNotifier; // Set by the UI to get notified when queue ischanged

        public Sequencer()
        {
            var thread = new Thread(() =>
            {
                while (!stopped && hasRequestsToProcess)
                {
                    Action<Queue<int>> request = (Q) => { }; //< take next request >
                    request(queue); // Process the request
                }
                //<keep the queue 20 items at most by removing any exceeding old items if needed>

                queueChangedNotifier?.Invoke(queue);
                //<wait 10ms before processing next request>
            });

            thread.Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        public Task PerformAsync(Action<Queue<int>> action)
        // please keep the signature of this method, except maybe adding
        // async if you need it and, of course, change the
        // SomeClassThatImplementsTheQueue to a real name
        {
            //<The sequencer might be busy when the method is called, so it must
            //be able to somehow preserve the incoming requests. The Task returned by
            //this method shall be set to the completed AFTER the request processing is
            //completed on the background thread.>
        }
    }
}
