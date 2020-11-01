using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Threading.Utilities;
using UIKit;

namespace Threading
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            

            var sequencer = new Sequencer();
            sequencer.queueChangedNotifier = (queue) =>
            {
                BeginInvokeOnMainThread(() =>
                {
                    var tableSource = new QueueTableViewSource();
                    tableSource.TableItems = queue.ToList();
                    queueTableView.Source = tableSource;
                    queueTableView.ReloadData();
                });
            };
            var producer1 = new Producer(2, new List<int>(){ 2000, 1 }, sequencer);
            var producer2 = new Producer(-1, new List<int>() { 1000, 2 }, sequencer);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}