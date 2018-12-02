using Layeetsta.Util;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public class AuthRespond
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public bool Succeed { get; set; }
        public int ErrorCode { get; set; }
    }

    public partial class LoginWindow : MetroWindow
    {
        public bool Result = false;
        public AuthRespond Auth = null;
        public LoginWindow()
        {
            InitializeComponent();
        }

        public string LoginBasicAuth
        {
            get
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(IdField.Text + ":" + PasswordField.Text));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(IdField.Text != String.Empty && PasswordField.Text != String.Empty)
            {
                var result = SWRequest.RequestJson(@"https://la.schwarzer.wang/auth/login", "Authorization", $"Basic {LoginBasicAuth}");
                if(result != null)
                {
                    var respond = JsonConvert.DeserializeObject<AuthRespond>(result);
                    if (respond != null && respond.Succeed)
                    {
                        Auth = respond;
                        Result = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Login Failed", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error While Login!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
