using ScenariosConfiguration.Models;
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

namespace ScenariosConfiguration
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string username = string.Empty;
        public string password = string.Empty;
        private List<User> users = new List<User>();
        public int loggedUserId = 0;
        public LoginWindow(List<User> _users)
        {
            users = _users;
            InitializeComponent();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var user in users)
            {
                if (password.Equals(user.Password) && username.Equals(user.Name))
                {
                    loggedUserId = user.Id;
                    Close();
                    return;
                }
            }
            MessageBox.Show("Nome de utilizador ou password incorretos!");
        }

        private void UserNameTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            username = UserNameTxtBox.Text;
        }

        private void PasswordTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            password = PasswordTxtBox.Text;
        }
    }
}
