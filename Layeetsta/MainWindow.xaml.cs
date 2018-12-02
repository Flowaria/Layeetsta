using Layeetsta.Util;
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
        public string Token = null;
        public string Id = null;

        public MainWindow()
        {
            InitializeComponent();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Directory.CreateDirectory("./Temp");

            var l = new LoginWindow();
            l.ShowDialog();
            if(l.Result && l.Auth.Succeed)
            {
                Token = l.Auth.AccessToken;
                Id = l.Auth.Id;
                Console.WriteLine(Id);
                RefreshContent();
            }
        }

        private void RefreshContent()
        {
            var result = SWRequest.RequestJson(@"https://la.schwarzer.wang/layestalevel/list/all", "Authorization", $"Bearer {Token}");
            var respond = JsonConvert.DeserializeObject<LayestaChartList>(result);

            ChartList.Items.Clear();

            foreach(var level in respond.Levels)
            {
                Console.WriteLine(level.Title);
                AddChart(level);
            }
        }

        private void AddChart(LayestaChart level)
        {
            var item = new ChartSelection();
            item.CoverURL = DownloadImage(level.Guid);
            item.Charter = level.Designer;
            item.Difficulty = level.Difficulties;
            item.Artist = level.SongArtist;
            item.SongName = level.Title;
            item.GUID = level.Guid;
            ChartList.Items.Add(item);
        }

        private string DownloadImage(string guid)
        {
            var imgpath = System.IO.Path.Combine("./Temp/", guid + ".png");
            if(!File.Exists(imgpath))
            {
                var r = SWRequest.RequestJson(@"https://la.schwarzer.wang/auth/oss/download/cover/" + guid, "Authorization", $"Bearer {Token}");
                var respond = JsonConvert.DeserializeObject<LayestaFile>(r);

                SWRequest.RequestDL(respond.Uri, Id, imgpath);
            }
            return System.IO.Path.GetFullPath(imgpath);
        }

        private void DownloadChart(string guid, string filename)
        {
            if (!File.Exists(filename))
            {
                var r = SWRequest.RequestJson(@"https://la.schwarzer.wang/auth/oss/download/layesta/" + guid, "Authorization", $"Bearer {Token}");
                var respond = JsonConvert.DeserializeObject<LayestaFile>(r);

                SWRequest.RequestDL(respond.Uri, Id, filename);
            }
        }

        private void DLLayesta_Click(object sender, RoutedEventArgs e)
        {
            if (ChartList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Theres no file selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ChartList.SelectedItems.Count == 1)
            {
                var file = FileDialog.LayestaSave();
                if(file != null)
                {
                    DownloadChart((ChartList.SelectedItem as ChartSelection).GUID, file);
                }
            }
            else if (ChartList.SelectedItems.Count > 1)
            {

            }
        }

        private void DLLap_Click(object sender, RoutedEventArgs e)
        {
            if (ChartList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Theres no file selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ChartList.SelectedItems.Count == 1)
            {

            }
            else if (ChartList.SelectedItems.Count > 1)
            {

            }
        }

        private void ChartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCounter.Text = String.Format("Selected {0} Items", ChartList.SelectedItems.Count);
        }
    }
}
