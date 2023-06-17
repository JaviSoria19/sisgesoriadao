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
using System.Data;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winLogin.xaml
    /// </summary>
    public partial class winLogin : Window
    {
        UsuarioImpl implUsuario;
        Usuario session;
        AjusteImpl implAjuste;
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
        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            winRecoverPassword winRecoverPassword = new winRecoverPassword();
            winRecoverPassword.Show();
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxGetUsuarioFromDatabase();
        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }
        private void txtPassword_Loaded(object sender, RoutedEventArgs e)
        {
            txtPassword.Focus();
        }
        void Login()
        {
            if (txtPassword != null)
            {
                try
                {
                    implUsuario = new UsuarioImpl();
                    session = implUsuario.Login(cbxUsuario.Text, txtPassword.Password);
                    if (session != null)
                    {

                        try
                        {
                            implAjuste = new AjusteImpl();
                            implAjuste.Get();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            throw;
                        }

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
        void cbxGetUsuarioFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxUsuario = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implUsuario = new UsuarioImpl();
                dataTable = implUsuario.SelectForComboBox();
                listcomboboxUsuario = (from DataRow dr in dataTable.Rows
                                     select new ComboboxItem()
                                     {
                                         Texto = dr["nombreUsuario"].ToString()
                                     }).ToList();
                foreach (var item in listcomboboxUsuario)
                {
                    cbxUsuario.Items.Add(item);
                }
                cbxUsuario.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem()
            {

            }
        }

        private void btnEULA_Click(object sender, RoutedEventArgs e)
        {
            winEULA winEULA = new winEULA();
            winEULA.Show();
        }
    }
}
