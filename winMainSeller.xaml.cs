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
using System.Data;//ADO.NET
using MySql.Data.MySqlClient;//MySql.Data

using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winMainSeller.xaml
    /// </summary>
    public partial class winMainSeller : Window
    {
        public winMainSeller()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = "Bienvenid@, " + Session.NombreUsuario;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está segur@ de cerrar la sesión actual?", "CERRAR SESIÓN", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                winLogin winLogin = new winLogin();
                winLogin.Show();
                this.Close();
            }
        }
    }
}
