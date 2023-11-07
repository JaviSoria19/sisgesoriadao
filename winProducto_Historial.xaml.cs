using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Windows;
using System.Windows.Input;
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
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text) != true)
            {
                Select();
            }
            else
            {
                MessageBox.Show("Por favor rellene el campo de búsqueda. (*)");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtBuscar.Focus();
            if (Session.Producto_Historial_CodigoSublote != null)
            {
                txtBuscar.Text = Session.Producto_Historial_CodigoSublote;
                Select();
            }
        }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text) != true)
            {
                if (e.Key == Key.Enter)
                {
                    Select();
                }
            }
            if (e.Key == Key.Escape)
            {
                CleanText();
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

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        void CleanText()
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
    }
}
