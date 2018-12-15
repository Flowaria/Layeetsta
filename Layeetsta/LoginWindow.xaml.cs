using Layeetsta.Util;
using Layeetsta.Web;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    

    public class AuthData
    {
        public string Id { get; set; }
        public string Pw { get; set; }
    }

    public partial class LoginWindow : MetroWindow
    {
        public bool Result = false;

        public LoginWindow()
        {
            InitializeComponent();

            if(File.Exists("./auth.data"))
            {
                var auth = JsonConvert.DeserializeObject<AuthData>(File.ReadAllText("./auth.data"));
                if(auth != null)
                {
                    IdField.Text = auth.Id;
                    PasswordField.Password = auth.Pw;
                    RememberAuth.IsChecked = true;
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.Content = "Logging in...";
            LoginButton.IsEnabled = false;
            IdField.IsEnabled = false;
            PasswordField.IsEnabled = false;

            try
            {
                await MainWindow.API.Login(IdField.Text, PasswordField.Password);

                if (RememberAuth.IsChecked.HasValue && RememberAuth.IsChecked.Value)
                {
                    var data = new AuthData()
                    {
                        Id = IdField.Text,
                        Pw = PasswordField.Password
                    };
                    var txtdata = JsonConvert.SerializeObject(data);
                    if (txtdata != null)
                        File.WriteAllText("./auth.data", txtdata);
                }

                Result = true;
                Close();
            }
            catch(LayestaWebAPIException ex)
            {
                if(ex.ErrorCode == (int)ErrorCode.WrongPassword)
                {
                    MessageBox.Show("Username or Password Invalid", "Error while login", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ErrorWindow.ShowException(ex);
                }
            }
            catch(Exception ex)
            {
                ErrorWindow.ShowException(ex);
            }

            LoginButton.Content = "Login";
            LoginButton.IsEnabled = true;
            IdField.IsEnabled = true;
            PasswordField.IsEnabled = true;
        }

        public bool CanLogin
        {
            get { return !string.IsNullOrWhiteSpace(IdField.Text) && !string.IsNullOrWhiteSpace(PasswordField.Password); }
        }

        private void PasswordField_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = CanLogin;
        }

        private void IdField_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginButton.IsEnabled = CanLogin;
        }

        private void RememberAuth_Unchecked(object sender, RoutedEventArgs e)
        {
            if (File.Exists("./auth.data"))
            {
                var result = MessageBox.Show("Previous Auth data will be wipe.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if(result == MessageBoxResult.Yes)
                {
                    IdField.Text = "";
                    PasswordField.Password = "";
                    File.Delete("./auth.data");
                }
                else
                {
                    var check = e.Source as CheckBox;
                    check.IsChecked = true;
                }
            }
        }
    }
}
