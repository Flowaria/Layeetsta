using System;
using System.Collections.Generic;
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
    /// ChartSelection.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class ChartSelection : UserControl
    {
        public string Charter { get; set; } = "";
        public string SongName { get; set; } = "";
        public string Artist { get; set; } = "";

        private string _coverurl = "";
        public string CoverURL
        {
            get
            {
                return _coverurl;
            }
            set
            {
                _coverurl = value;
                
                CoverImage.Source = new BitmapImage(new Uri(_coverurl, UriKind.Absolute));
            }
        }
        public int DownloadCount { get; set; } = 0;
        public int Index { get; set; } = 0;

        public float Rate { get; set; } = 0.0f;

        public string GUID = null;

        public bool ParticipantCurrentContest { get; set; } = false;


        public ChartSelection()
        {
            InitializeComponent();
        }
    }
}
