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
        private readonly Queue<int> timingQueue = new Queue<int>();

        public Producer(int incrementation, IEnumerable<int> timings, Sequencer s)
        {
            sequencer = s;
            increment = incrementation;
            foreach (var timing in timings)
            {
                timingQueue.Enqueue(timing);
            }
            currentNumber = 0;

            var thread = new Thread(async () =>
            {
                while (!stopped)
                {
                    // Generate the next number in the sequence
                    currentNumber += increment; 

                    await sequencer.PerformAsync((queue) => {
                        queue.Enqueue(currentNumber);
                        Console.WriteLine(currentNumber);
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
