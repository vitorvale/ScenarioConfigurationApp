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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private User user = new User();
        private string oldPassword = string.Empty;
        public string newPassword = string.Empty;
        private string newPasswordRepeat = string.Empty;
        public bool shouldSave = false;
        public ChangePasswordWindow(User _user)
        {
            this.user = _user;

            InitializeComponent();
        }

        private void OldPasswordBox_Changed(object sender, RoutedEventArgs e)
        {
            oldPassword = OldPasswordBox.Password;
        }

        private void NewPasswordBox_Changed(object sender, RoutedEventArgs e)
        {
            newPassword = NewPasswordBox.Password;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (oldPassword.Equals(user.Password) && newPassword.Equals(newPasswordRepeat))
            {
                shouldSave = true;
                Close();
            }
            else if (!oldPassword.Equals(user.Password) && newPassword.Equals(newPasswordRepeat))
            {
                MessageBox.Show("Palavra-passe antiga incorreta!");
            }
            else
            {
                MessageBox.Show("Palavra-passe nova e a repetida não correspondem!");
            }
        }

        private void NewPasswordRepeatBox_Changed(object sender, RoutedEventArgs e)
        {
            newPasswordRepeat = NewPasswordRepeatBox.Password;
        }
    }
}
