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

using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winLogin.xaml
    /// </summary>
    public partial class winLogin : Window
    {
        UsuarioImpl implUsuario;
        Usuario session;
        public winLogin()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
        public void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }
        void Login()
        {
            if (txtUserName.Text != null && txtPassword != null)
            {
                try
                {
                    implUsuario = new UsuarioImpl();
                    session = implUsuario.Login(txtUserName.Text, txtPassword.Password);
                    if (session != null)
                    {
                        winLogin_Sucursal winLogin_Sucursal = new winLogin_Sucursal();
                        winLogin_Sucursal.Show();
                        this.Close();
                    }
                    else
                    {
                        lblInfo.Content = "ESTADO: USUARIO O CONTRASEÑA INCORRECTOS, INTENTE NUEVAMENTE.";
                        lblInfo.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            winRecoverPassword winRecoverPassword = new winRecoverPassword();
            winRecoverPassword.Show();
            this.Close();
        }
    }
}
