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
    public partial class ChartSelection : UserControl
    {
        private string _charter = "";
        public string Charter
        {
            get
            {
                return _charter;
            }
            set
            {
                _charter = value;
            }
        }

        private string _song = "";
        public string SongName
        {
            get
            {
                return _song;
            }
            set
            {
                _song = value;
            }
        }

        private string _artist = "";
        public string Artist
        {
            get
            {
                return _artist;
            }
            set
            {
                _artist = value;
            }
        }

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

        private string _difficulty = "";
        public string Difficulty
        {
            get
            {
                return _difficulty;
            }
            set
            {
                _difficulty = value;
            }
        }

        public string GUID = null;

        public ChartSelection()
        {
            InitializeComponent();
        }
    }
}
