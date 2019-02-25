using Layeetsta.Util;
using Layeetsta.Web;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Layeetsta
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class MainWindow : MetroWindow
    {
        public static WebAPI API = new WebAPI();

        public string Token = null;
        public string Id = null;

        public MainWindow()
        {
            InitializeComponent();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Directory.CreateDirectory("./Temp");
            Directory.CreateDirectory("./Download");
            Directory.CreateDirectory("./Resources");
            if(!File.Exists("./Resources/ffmpeg.exe"))
            {
                File.WriteAllBytes("./Resources/ffmpeg.exe", Properties.Resources.ffmpeg);
            }
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await TryLogin();
            await RefreshContent();
        }

        public async Task TryLogin(bool force_login = false)
        {
            try
            {
                if(!force_login && File.Exists("./cache.auth"))
                {
                    API.ApplyAuth("5d79baff-c3fd-4023-a1ab-502eb1b95725", File.ReadAllText("./cache.auth"));
                }
                else
                {
                    await API.Login("5d79baff-c3fd-4023-a1ab-502eb1b95725", "8397121f-8fa4-495e-85b9-e2b023f6b285");
                    File.WriteAllText("./cache.auth", API.Token);
                }
            }
            catch (LayestaWebAPIException ex)
            {
                ErrorWindow.ShowException(ex, true);
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowException(ex, true);
            }
        }

        private async Task RefreshContent()
        {
            try
            {
                var respond = await API.GetLevelList();
                var respond_contest = await API.GetContestLevelList();
                var selections = new List<ChartSelection>();
                foreach (var level in respond.Levels)
                {
                    var item = new ChartSelection()
                    {
                        Charter = level.Designer,
                        DownloadCount = level.DownloadCount,
                        Artist = level.SongArtist,
                        SongName = level.Title,
                        GUID = level.Guid,
                        Index = selections.Count + 1,
                        Rate = level.Rating
                    };
                    
                    if (respond_contest.Levels.Exists((x) => x.Guid.Equals(item.GUID)))
                    {
                        item.ParticipantCurrentContest = true;
                    }

                    selections.Add(item);
                }

                await Task.Run(() =>
                {
                    Parallel.ForEach(respond.Levels, async level =>
                    {
                        var image = await DownloadImage(level.Guid);
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            var item = selections.Find(x => x.GUID.Equals(level.Guid));
                            item.CoverURL = image;
                        }));
                    });
                });

                if(ChartList.ItemsSource != null)
                {
                    (ChartList.ItemsSource as List<ChartSelection>).Clear();
                    ChartList.Items.SortDescriptions.Clear();
                }

                ChartList.ItemsSource = selections;

                ICollectionView view = CollectionViewSource.GetDefaultView(ChartList.ItemsSource);
                view.Filter = (o) =>
                {
                    bool flag = false;
                    ChartSelection item = o as ChartSelection;
                    if (CheckBox_ContestChart.IsChecked.Value)
                    {
                        flag = item.ParticipantCurrentContest;
                    }
                    else
                    {
                        flag = true;
                    }

                    if(flag)
                    {
                        if(SearchBar.Text.Length > 0)
                        {
                            string query = SearchBar.Text.ToLower();
                            return item.SongName.ToLower().Contains(query)
                            || item.Artist.ToLower().Contains(query)
                            || item.Charter.ToLower().Contains(query);
                        }
                        return true;
                    }
                    return false;
                };

            }
            catch (LayestaWebAPINeedLoginException ex)
            {
                await TryLogin(true);
                await RefreshContent();
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowException(ex);
            }
        }

        private async Task<string> DownloadImage(string guid)
        {
            var imgpath = System.IO.Path.Combine("./Temp/", guid + ".png");
            if(!File.Exists(imgpath))
            {
                return await API.DownloadCoverImage(guid, imgpath);
            }
            return System.IO.Path.GetFullPath(imgpath);
        }

        private void ChartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCounter.Text = String.Format("Selected {0} Items", ChartList.SelectedItems.Count);
        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshContent();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if(ChartList.SelectedItems.Count > 0)
            {
                var dialog = new DownloadWindow();
                foreach(var item in ChartList.SelectedItems)
                {
                    var sel = item as ChartSelection;
                    var i = new LayestaInfo()
                    {
                        SongName = sel.SongName,
                        Charter = sel.Charter,
                        GUID = sel.GUID
                    };
                    dialog.Selections.Add(i);
                } 
                dialog.ShowDialog();
            }
        }

        private void DeselectButton_Click(object sender, RoutedEventArgs e)
        {
            ChartList.SelectedItems.Clear();
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if(ChartList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(ChartList.ItemsSource).Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ChartList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(ChartList.ItemsSource).Refresh();
        }

        private void ChartListItem_RightClick(object sender, MouseButtonEventArgs e)
        {
            var chart = ((FrameworkElement)e.OriginalSource).DataContext as ChartSelection;

            var dialog = new RatingListWindow();
            dialog.GUID = chart.GUID;
            dialog.ShowDialog();
            e.Handled = true;
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
