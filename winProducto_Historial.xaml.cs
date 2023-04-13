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
using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Historial.xaml
    /// </summary>
    public partial class winProducto_Historial : Window
    {
        ProductoImpl implProducto;
        public winProducto_Historial()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text)!=true)
            {
                Select();
            }
        }
        private void Select()
        {
            try
            {
                int n;
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectProductHistory(txtBuscar.Text).DefaultView;
                n = implProducto.SelectProductHistory(txtBuscar.Text).Rows.Count;
                if (n > 0)
                {
                    lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.SelectProductHistory(txtBuscar.Text).Rows.Count;
                }
                else
                {
                    lblDataGridRows.Content = "PRODUCTO NO ENCONTRADO.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text)!=true)
            {
                if (e.Key == Key.Enter)
                {
                    Select();
                }
            }
        }
    }
}
