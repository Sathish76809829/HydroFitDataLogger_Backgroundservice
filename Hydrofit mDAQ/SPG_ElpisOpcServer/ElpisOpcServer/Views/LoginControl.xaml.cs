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

namespace ElpisOpcServer.Views
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        
        
        public LoginControl()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (btnLogin.Content.ToString() == "Login")
            {
                if (!string.IsNullOrEmpty(txtUserName.Text) && !string.IsNullOrEmpty(txtPasswword.Password))
                {
                    if (txtUserName.Text == "Admin" && txtPasswword.Password == "Testing123")
                    {

                        ElpisOPCServerMainWindow.isLogin = true;
                        ElpisOPCServerMainWindow.LogerStatus = txtUserName.Text;


                       // MessageBox.Show("Login Sucessful");
                        btnLogin.Content = "Logout";
                    }
                    else if (txtUserName.Text == "User" && txtPasswword.Password == "Testing123")
                    {
                        ElpisOPCServerMainWindow.isLogin = true;
                        ElpisOPCServerMainWindow.LogerStatus = txtUserName.Text;
                       // MessageBox.Show("Login Sucessful");
                        btnLogin.Content = "Logout";
                    }
                    else
                    {
                        MessageBox.Show("please use the  valid user & password");
                    }
                }
                else
                {
                    MessageBox.Show("please UserName or Password ");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtUserName.Text) && !string.IsNullOrEmpty(txtPasswword.Password))
                {
                    ElpisOPCServerMainWindow.isLogin = false;

                    //MessageBox.Show("Logout Sucessful");
                    btnLogin.Content = "Login";
                    txtUserName.Text = string.Empty;
                    txtPasswword.Password = string.Empty;
                }
                else
                {
                    MessageBox.Show("Logout Details are Empty");
                }

            }

        }
    }
}
