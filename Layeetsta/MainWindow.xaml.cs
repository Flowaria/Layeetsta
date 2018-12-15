using Layeetsta.Util;
using Layeetsta.Web;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

    public class LayestaFile
    {
        public string Uri { get; set; }
        public bool Succeed { get; set; }
        public int ErrorCode { get; set; }
    }

    public class LayestaChartList
    {
        public bool Succeed { get; set; }
        public List<LayestaChart> Levels { get; set; }
        public int ErrorCode { get; set; }
    }

    public class LayestaChart
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public string SongArtist { get; set; }
        public string Difficulties { get; set; }
        public int DownloadCount { get; set; }
        public bool ShouldDisplay { get; set; }
        public string Designer { get; set; }
    }

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

            TryLogin();
            RefreshContent(true);
        }

        public void TryLogin()
        {
            var l = new LoginWindow();
            l.ShowDialog();
        }

        private async void RefreshContent(bool firsttime=false)
        {
            try
            {
                var respond = await API.GetLevelList();

                ChartList.Items.Clear();

                var selections = new List<ChartSelection>();
                foreach (var level in respond.Levels)
                {
                    var item = new ChartSelection();
                    item.Charter = level.Designer;
                    item.DownloadCount = level.DownloadCount;
                    item.Artist = level.SongArtist;
                    item.SongName = level.Title;
                    item.GUID = level.Guid;
                    item.Index = selections.Count + 1;
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
                

                foreach(var sel in selections)
                {
                    ChartList.Items.Add(sel);
                }
            }
            catch (LayestaWebAPINeedLoginException ex)
            {
                if(firsttime)
                    Environment.Exit(0);

                TryLogin();
                RefreshContent();
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

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshContent();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if(ChartList.SelectedItems.Count > 0)
            {
                var dialog = new DownloadWindow();
                foreach(var item in ChartList.SelectedItems)
                {
                    var sel = item as ChartSelection;
                    var i = new LayestaInfo();
                    i.SongName = sel.SongName;
                    i.Charter = sel.Charter;
                    i.GUID = sel.GUID;
                    dialog.Selections.Add(i);
                } 
                dialog.ShowDialog();
            }
        }
    }
}
