using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Threading
{
    public class QueueTableViewSource : UITableViewSource
    {
        public List<int> TableItems { get; set; }
        string cellIdentifier = "tableCell"; // set in the Storyboard
        UIColor redColor = UIColor.SystemRedColor.ColorWithAlpha(.3f);
        UIColor greenColor = UIColor.SystemGreenColor.ColorWithAlpha(.3f);

        public QueueTableViewSource()
        {
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            var number = TableItems[indexPath.Row];
            cell.TextLabel.Text = number.ToString();
            cell.BackgroundColor = number > 0 ? greenColor : redColor;
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }
    }
}
