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
    /// Lógica de interacción para winCaja.xaml
    /// </summary>
    public partial class winCaja : Window
    {
        CajaImpl implCaja;
        Caja caja;
        VentaImpl implVenta;
        double totalCajaUSD;
        double totalCajaBOB;
        public winCaja()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnClosePendingCash_Click(object sender, RoutedEventArgs e)
        {
            getCajaActualyCerrarCajaActual();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtSucursal.Text = Session.Sucursal_NombreSucursal;
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dgvVentasPendientes_Loaded(object sender, RoutedEventArgs e)
        {
            SelectVentasConSaldoPendiente();
        }
        private void Select()
        {
            try
            {
                implCaja = new CajaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCaja.SelectPendingCashFromBranch().DefaultView;
                //dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCaja.SelectPendingCashFromBranch().Rows.Count;
                CalcularTotalCaja();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectVentasConSaldoPendiente()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvVentasPendientes.ItemsSource = null;
                dgvVentasPendientes.ItemsSource = implVenta.SelectSalesWithPendingBalanceFromBranch().DefaultView;
                dgvVentasPendientes.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRowsVentasPendientes.Content = "NÚMERO DE REGISTROS: " + implVenta.SelectSalesWithPendingBalanceFromBranch().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CalcularTotalCaja()
        {
            if (dgvDatos.Items.Count > 0)
            {
                foreach (DataRowView item in dgvDatos.Items)
                {
                    totalCajaUSD += double.Parse(item[3].ToString());
                    totalCajaBOB += double.Parse(item[4].ToString());
                }
                txtCajaTotalUSD.Text = "Total $us.: " + totalCajaUSD.ToString();
                txtCajaTotalBOB.Text = "Total Bs.: " + totalCajaBOB.ToString();
            }
            else
            {
                txtCajaTotalUSD.Text = "Total $us.: " + totalCajaUSD.ToString();
                txtCajaTotalBOB.Text = "Total Bs.: " + totalCajaBOB.ToString();
            }
        }
        void getCajaActualyCerrarCajaActual()
        {
            if (dgvDatos.Items.IsEmpty != true)
            {
                try
                {
                    caja = implCaja.GetByBranch();
                    if (caja != null)
                    {
                        string mensaje = "";
                        mensaje = implCaja.UpdateClosePendingCashTransaction(caja);
                        if (mensaje == "CAJA CERRADA EXITOSAMENTE.")
                        {
                            MessageBox.Show(mensaje);
                            totalCajaUSD = 0;
                            totalCajaBOB = 0;
                            Select();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
            else
            {
                MessageBox.Show("¡No puede realizar el cierre de caja si se encuentra vacía!");
            }
        }
        void imprimirPDF()
        {

        }
    }
}
