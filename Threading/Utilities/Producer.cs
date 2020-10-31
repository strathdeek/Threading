using System;
using System.Threading;

namespace Threading.Utilities
{
    public class Producer
    {
        private bool stopped = false;
        private readonly Sequencer sequencer;

        public Producer()
        {
            var thread = new Thread(() =>
            {
                while (!stopped)
                {
                    var number = 1; //< generate new number from the sequence>
                    sequencer.PerformAsync((queue) => {
                        queue.Enqueue(number);
                     }); // change it however needed to make it work correctly

                    //< wait the needed delay AFTER adding of the number is completed >
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
