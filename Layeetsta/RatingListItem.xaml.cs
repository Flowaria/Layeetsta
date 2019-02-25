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
    /// RatingListItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RatingListItem : UserControl
    {
        public static SolidColorBrush NormalBrush, BlackBrush;
        public static LinearGradientBrush DisqualifiedBrush;

        static RatingListItem()
        {
            NormalBrush = new SolidColorBrush();
            NormalBrush.Color = Color.FromArgb(255, 255, 255, 255);

            BlackBrush = new SolidColorBrush();
            BlackBrush.Color = Color.FromArgb(255, 0, 0, 0);

            DisqualifiedBrush = new LinearGradientBrush();
            DisqualifiedBrush.StartPoint = new Point(0, 0);
            DisqualifiedBrush.EndPoint = new Point(1, 1);
            DisqualifiedBrush.GradientStops.Add(new GradientStop(Colors.OrangeRed, 0.0));
            DisqualifiedBrush.GradientStops.Add(new GradientStop(Colors.White, 0.25));
        }

        public string Username { get; set; } = "";
        public float Rating { get; set; } = 0.0f;
        public string Comment { get; set; } = "";
        public int ThumbUps { get; set; } = 0;
        public int ThumbDowns { get; set; } = 0;

        private bool _countasscore = false;
        public bool CountAsScore
        {
            get
            {
                return _countasscore;
            }
            set
            {
                if (!value)
                {
                    Background = DisqualifiedBrush;
                }
                else
                {
                    Background = NormalBrush;
                }
                _countasscore = value;
            }
        }

        public RatingListItem()
        {
            InitializeComponent();
            Height = 64;
            Foreground = BlackBrush;
        }
    }
}
