using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Threading.Utilities
{
    public class Sequencer
    {
        private bool stopped = false;
        private bool hasRequestsToProcess => requestQueue.Any();

        // Lock to prevent issues resulting from simultaneous access
        private readonly object queueLock = new object(); 

        // Queue to store incoming requests
        private Queue<Action<Queue<int>>> requestQueue = new Queue<Action<Queue<int>>>();

        // Queue to simulate the underlying database
        private readonly Queue<int> queue = new Queue<int>();

        // Used to notify the UI of changes to the queue
        public Action<Queue<int>> queueChangedNotifier; 


        public Sequencer()
        {
            var thread = new Thread(() =>
            {
                while (!stopped && hasRequestsToProcess)
                {
                    // Take next request and process it.
                    Action<Queue<int>> request = requestQueue.Dequeue();
                    request(queue); 

                    // Trim the queue if needed
                    while (queue.Count>20)
                    {
                        queue.Dequeue();
                    }

                    queueChangedNotifier?.Invoke(queue);
                    Thread.Sleep(10);
                }
            });

            thread.Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        public Task PerformAsync(Action<Queue<int>> action)
        {
            lock (queueLock)
            {
                return Task.Run(() => requestQueue.Enqueue(action));
            }
        }
    }
}
