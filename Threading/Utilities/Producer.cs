using System;
using System.Collections.Generic;
using System.Threading;

namespace Threading.Utilities
{
    public class Producer
    {
        private bool stopped = false;
        private readonly Sequencer sequencer;
        private readonly int increment;
        private int currentNumber;
        private readonly Queue<int> timingQueue;

        public Producer(int incrementation, Queue<int> timings)
        {
            increment = incrementation;
            timingQueue = timings;
            currentNumber = 0;

            var thread = new Thread(() =>
            {
                while (!stopped)
                {
                    currentNumber += increment; // < generate new number from the sequence >

                    sequencer.PerformAsync((queue) => {
                        queue.Enqueue(currentNumber);
                     });

                    // fetch the current delay, wait that time, then push it to the back
                    var currentDelay = timingQueue.Dequeue();
                    Thread.Sleep(currentDelay);
                    timingQueue.Enqueue(currentDelay);
                }
            });
            thread.Start();
        }

        public void Stop()
        {
            stopped = true;
        }
    }
}
