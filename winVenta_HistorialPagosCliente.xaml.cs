using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_HistorialPagosCliente.xaml
    /// </summary>
    public partial class winVenta_HistorialPagosCliente : Window
    {
        VentaImpl implVenta;
        double MontoUSD = 0, MontoBOB = 0;
        public winVenta_HistorialPagosCliente()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Now;
            dtpFechaInicio.SelectedDate = DateTime.Now;
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            SelectMetodosdePago();
        }
        private void SelectMetodosdePago()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectLikePaymentMethodsByCustomers(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, txtBuscar_Producto_o_Codigo.Text).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "Registros: " + dgvDatos.Items.Count;

                MontoBOB = 0;
                MontoUSD = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    MontoUSD += double.Parse(item[2].ToString());
                    MontoBOB += double.Parse(item[3].ToString());
                }
                txtTotalSaldoUSD.Text = "Total Pagos $.: " + MontoUSD;
                txtTotalSaldoBOB.Text = "Total Pagos Bs.: " + MontoBOB;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectMetodosdePago();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }
        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPDF(dgvDatos,"HISTORIAL_DE_PAGOS_CLIENTE");
        }
        private void btndgvModificar(object sender, RoutedEventArgs e)
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
                }
            }
        }

        private void txtBuscar_Producto_o_Codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectMetodosdePago();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
                (sender as TextBox).Focus();
            }
        }

        private void btndgvImprimir(object sender, RoutedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_Detalle winVenta_Detalle = new winVenta_Detalle();
                    winVenta_Detalle.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
