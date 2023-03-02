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
    /// Lógica de interacción para winRecoverPassword.xaml
    /// </summary>
    public partial class winRecoverPassword : Window
    {
        UsuarioImpl implUsuario;
        Usuario usuario;
        public winRecoverPassword()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text != null && txtPin.Password != null)
            {
                try
                {
                    implUsuario = new UsuarioImpl();
                    usuario = implUsuario.SelectRecoverPasswordWithPin(txtUserName.Text, txtPin.Password);
                    if (usuario != null)
                    {
                        txtUserName.IsEnabled = false;
                        txtPin.IsEnabled = false;
                        lblInfo.Content = "ESTADO: USUARIO ENCONTRADO, INGRESE Y CONFIRME SU NUEVA CONTRASEÑA.";
                        lblInfo.Foreground = new SolidColorBrush(Colors.SpringGreen);
                        txtPassword.IsEnabled = true;
                        txtRePassword.IsEnabled = true;
                        btnConfirmPassword.IsEnabled = true;
                    }
                    else
                    {
                        lblInfo.Content = "ESTADO: USUARIO O PIN INCORRECTOS, INTENTE NUEVAMENTE.";
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

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            winLogin winLogin = new winLogin();
            winLogin.Show();
            this.Close();
        }

        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        private void btnConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtRePassword.Password)
            {
                lblInfo.Content = "ESTADO: LAS CONTRASEÑAS NO COINCIDEN.";
                lblInfo.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                try
                {
                    int n = implUsuario.UpdateRecoverPasswordWithPin(usuario.IdUsuario, txtPassword.Password);
                    if (n > 0)
                    {
                        MessageBox.Show("ATENCIÓN: SU CONTRASEÑA HA SIDO ACTUALIZADA CON ÉXITO, POR FAVOR VUELVA A INICIAR SESIÓN");
                        winLogin winLogin = new winLogin();
                        winLogin.Show();
                        this.Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                }
            }
        }
    }
}
