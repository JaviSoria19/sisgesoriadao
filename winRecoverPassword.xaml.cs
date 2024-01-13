﻿using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            sendUserAndPin();
        }
        void sendUserAndPin()
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
                        txtPassword.Focus();
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
        private void btnConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            confirmPassword();
        }
        void confirmPassword()
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
        private void TextBoxUppercase(object sender, KeyEventArgs e)
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
                txtRePassword.Focus();
            }
        }

        private void txtRePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                confirmPassword();
            }
        }

        private void txtPin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendUserAndPin();
            }
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPin.Focus();
            }
        }
    }
}
