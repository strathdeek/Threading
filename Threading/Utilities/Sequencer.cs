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

        // Lock to prevent issues resulting from simultaneous access
        private readonly object queueLock = new object(); 

        // Queue to store incoming requests
        private Queue<DatabaseWriteAction> requestQueue = new Queue<DatabaseWriteAction>();

        // Queue to simulate the underlying database
        private readonly Queue<int> queue = new Queue<int>();

        // Used to notify the UI of changes to the queue
        public Action<Queue<int>> queueChangedNotifier; 


        public Sequencer()
        {
            Start();
        }

        public void Start()
        {
            stopped = false;
            var thread = new Thread(() =>
            {
                while (!stopped)
                {
                    // Take next request and process it.
                    if(requestQueue.TryDequeue(out DatabaseWriteAction request))
                    {
                        request.WriteAction?.Invoke(queue);

                        // Trim the queue if needed
                        while (queue.Count > 20)
                        {
                            queue.Dequeue();
                        }

                        queueChangedNotifier?.Invoke(queue);
                        Thread.Sleep(10);
                        request.ActionPerformedCallback?.Invoke();
                    }
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
            bool actionPerformed = false;
            lock (queueLock)
            {
                var writeAction = new DatabaseWriteAction()
                {
                    WriteAction = action,
                    ActionPerformedCallback = () =>
                    {
                        actionPerformed = true;
                    }
                };
                requestQueue.Enqueue(writeAction);
            }

            // Here we wait for the background thread to invoke the ActionPerformedCallback, indicating a successful write operation
            // We are performing a 'busy wait', however depending our performance preferences we could instead implement a 'slow poll'
            // by inserting a Task.Delay(*frequency of poll*) into the while loop.
            while (!actionPerformed)
            {
            }
            return Task.CompletedTask;
        }

        private struct DatabaseWriteAction
        {
            public Action<Queue<int>> WriteAction;
            public Action ActionPerformedCallback;
        }
    }
}
