using Layeetsta.Util;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Layeetsta
{
    /// <summary>
    /// RatingListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RatingListWindow : MetroWindow
    {
        public string GUID { get; set; }

        public RatingListWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var imgpath = Path.Combine("./Temp/", GUID + ".png");
            if (File.Exists(imgpath))
            {
                CoverImage.Source = new BitmapImage(new Uri(Path.GetFullPath(imgpath), UriKind.Absolute));
            }

            var list = await MainWindow.API.GetRatingList(GUID);

            Title = list.Level.Title;
            RateText.Text = String.Format("{0:0.00} / 5.00", list.Level.Rating);

            foreach(var rating in list.Level.Ratings)
            {
                var item = new RatingListItem()
                {
                    Username = rating.Username,
                    Rating = rating.Rating,
                    Comment = rating.Comment,
                    ThumbUps = rating.ThumbUps,
                    ThumbDowns = rating.ThumbDowns,
                    CountAsScore = false
                };
                RatingList.Items.Add(item);
            }
        }

        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as RatingListItem;
            Clipboard.SetText(item.Comment);
        }

        #region aligncode
        private string currentheader = "ID";

        private SortDescription sort_rule = new SortDescription("Index", ListSortDirection.Ascending);

        private void ListHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = e.OriginalSource as GridViewColumnHeader;
            var ChartList = e.Source as ListView;
            if (header.Column.DisplayMemberBinding != null)
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
                if (ChartList.Items.SortDescriptions.Count > 0)
                    ChartList.Items.SortDescriptions.Clear();

                ChartList.Items.SortDescriptions.Add(sort_rule);
            }
        }

        private ListSortDirection ToggleDirection(ListSortDirection dir)
        {
            return dir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        #endregion

        
    }
}
