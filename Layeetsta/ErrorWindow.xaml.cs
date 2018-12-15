using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Layeetsta
{
    /// <summary>
    /// ErrorWindow1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ErrorWindow : MetroWindow
    {
        public static void ShowException(Exception e, bool KillProgram = false)
        {
            var dialog = new ErrorWindow();
            dialog.AppendLine(e.Message);
            dialog.AppendLine("==Source Info==");
            if(e.Source != null)
            {
                dialog.AppendText("at: ");
                dialog.AppendLine(e.Source);
            }
            if(e.TargetSite != null)
            {
                dialog.AppendText("at-method: ");
                dialog.AppendLine(e.TargetSite.Name);
            }
            
            dialog.AppendLine("==Stack Trace==");
            dialog.AppendLine(e.StackTrace);
            
            dialog.ShowDialog();
            if (KillProgram)
                Environment.Exit(0);
        }

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public void AppendText(string str)
        {
            Textbox.Text += str;
        }
        public void AppendLine(string str)
        {
            Textbox.Text += str;
            Textbox.Text += Environment.NewLine;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Textbox.Text);
        }
    }
}
