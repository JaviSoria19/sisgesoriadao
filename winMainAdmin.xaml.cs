﻿using System;
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
    /// Lógica de interacción para winMainAdmin.xaml
    /// </summary>
    public partial class winMainAdmin : Window
    {
        VentaImpl implVenta;
        public winMainAdmin()
        {
            InitializeComponent();
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
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            winAjuste winAjuste = new winAjuste();
            winAjuste.Show();
        }
        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            winEmpleado winEmpleado = new winEmpleado();
            winEmpleado.Show();
        }
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            winUsuario winUsuario = new winUsuario();
            winUsuario.Show();
        }
        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            winCliente winCliente = new winCliente();
            winCliente.Show();
        }
        private void btnBranches_Click(object sender, RoutedEventArgs e)
        {
            winSucursal winSucursal = new winSucursal();
            winSucursal.Show();
        }
        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto winProducto = new winProducto();
            winProducto.Show();
        }
        private void btnAddProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Insert winProducto_Insert = new winProducto_Insert();
            winProducto_Insert.Show();
        }
        private void btnBatches_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Lote winProducto_Lote = new winProducto_Lote();
            winProducto_Lote.Show();
        }
        private void btnHistoryProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Historial winProducto_Historial = new winProducto_Historial();
            winProducto_Historial.Show();
        }
        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            winCategoria winCategoria = new winCategoria();
            winCategoria.Show();
        }
        private void btnConditionsProducts_Click(object sender, RoutedEventArgs e)
        {
            winCondicion winCondicion = new winCondicion();
            winCondicion.Show();
        }
        private void btnTransferProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Transferencia winProducto_Transferencia = new winProducto_Transferencia();
            winProducto_Transferencia.Show();
        }

        private void btnReceiveProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Transferencia_Recibir winProducto_Transferencia_Recibir = new winProducto_Transferencia_Recibir();
            winProducto_Transferencia_Recibir.Show();
        }
        private void btnMovementProductHistory_Click(object sender, RoutedEventArgs e)
        {
            winTransferencia winTransferencia = new winTransferencia();
            winTransferencia.Show();
        }
        private void btnSaleAdd_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Insert winVenta_Insert = new winVenta_Insert();
            winVenta_Insert.Show();
        }
        private void btnQuotationAdd_Click(object sender, RoutedEventArgs e)
        {
            winCotizacion_Insert winCotizacion_Insert = new winCotizacion_Insert();
            winCotizacion_Insert.Show();
        }
        private void btnQuotations_Click(object sender, RoutedEventArgs e)
        {
            winCotizacion winCotizacion = new winCotizacion();
            winCotizacion.Show();
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            LoadInfoFromDB();
        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = "Bienvenid@ a " + Session.Sucursal_NombreSucursal + " , " + Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            LoadInfoFromDB();
        }
        void LoadInfoFromDB()
        {
            try
            {
                implVenta = new VentaImpl();
                infoTotalSalesFromToday.Text = "VENTAS DE HOY: " + implVenta.GetTodaySales(DateTime.Today);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                implVenta = new VentaImpl();
                infoTotalProductsFromToday.Text = "PRODUCTOS VENDIDOS DE HOY: " + implVenta.GetTodayProducts(DateTime.Today);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                implVenta = new VentaImpl();
                var (mUSD, mBOB) = implVenta.GetCashAmounts();
                infoCashAmount.Text = "EFECTIVO EN CAJA: " + mUSD.ToString() + " USD. | " + mBOB.ToString() + " BS.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
