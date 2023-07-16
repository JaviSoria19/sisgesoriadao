using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_DeudasClientes.xaml
    /// </summary>
    public partial class winVenta_DeudasClientes : Window
    {
        VentaImpl implVenta;
        public winVenta_DeudasClientes()
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
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            SelectVentasConSaldoPendiente();
        }
        private void SelectVentasConSaldoPendiente()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectAllSalesWithPendingBalanceByCustomers().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "Registros: " + dgvDatos.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_Update winVenta_Update = new winVenta_Update();
                    winVenta_Update.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
    }
}
