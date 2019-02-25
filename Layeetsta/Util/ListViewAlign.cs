using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Layeetsta.Util
{
    public class ListViewAlign
    {
        private string currentheader = "ID";

        private SortDescription sort_rule = new SortDescription("Index", ListSortDirection.Ascending);

        private void ListHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            var ChartList = header.Parent as ListView;
            if(header.Column.DisplayMemberBinding != null)
            {
                var columnBinding = header.Column.DisplayMemberBinding as Binding;
                var sortBy = columnBinding?.Path.Path ?? header.Column.Header as string;

                if (sortBy == currentheader)
                {
                    sort_rule.Direction = ToggleDirection(sort_rule.Direction);
                }
                else
                {
                    sort_rule.PropertyName = sortBy;
                    sort_rule.Direction = ListSortDirection.Ascending;

                    currentheader = sortBy;
                }
                if(ChartList.Items.SortDescriptions.Count > 0)
                    ChartList.Items.SortDescriptions.Clear();

                ChartList.Items.SortDescriptions.Add(sort_rule);
            }
        }

        private ListSortDirection ToggleDirection(ListSortDirection dir)
        {
            return dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }
    }
}
