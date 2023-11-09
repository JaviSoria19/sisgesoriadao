using MaterialDesignThemes.Wpf;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            WindowState = WindowState.Maximized;
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        private void btnUpdateProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Update winProducto_Update = new winProducto_Update();
            winProducto_Update.Show();
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
        private void btnCashPending_Click(object sender, RoutedEventArgs e)
        {
            winCaja winCaja = new winCaja();
            winCaja.Show();
        }
        private void btnCashHistory_Click(object sender, RoutedEventArgs e)
        {
            winCaja_Registros winCaja_Registros = new winCaja_Registros();
            winCaja_Registros.Show();
        }
        private void btnCashAdminConfirm_Click(object sender, RoutedEventArgs e)
        {
            winCaja_AdminRecepcion winCaja_AdminRecepcion = new winCaja_AdminRecepcion();
            winCaja_AdminRecepcion.Show();
        }
        private void btnProducts_Inventory_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Inventario winProducto_Inventario = new winProducto_Inventario();
            winProducto_Inventario.Show();
        }
        private void btnSaleHistory_Click(object sender, RoutedEventArgs e)
        {
            winVenta winVenta = new winVenta();
            winVenta.Show();
        }
        private void btnSaleProfit_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Utilidad winVenta_Utilidad = new winVenta_Utilidad();
            winVenta_Utilidad.Show();
        }
        private void btnSaleDeficit_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Perdida winVenta_Perdida = new winVenta_Perdida();
            winVenta_Perdida.Show();
        }
        private void btnLocalSales_Click(object sender, RoutedEventArgs e)
        {
            winVenta_HistorialLocal winVenta_HistorialLocal = new winVenta_HistorialLocal();
            winVenta_HistorialLocal.Show();
        }
        private void btnCommonProduct_Click(object sender, RoutedEventArgs e)
        {
            winProductoComun winProductoComun = new winProductoComun();
            winProductoComun.Show();
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            LoadInfoFromDB();
            SelectDeudores();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = "Bienvenid@ a " + Session.Sucursal_NombreSucursal + " , " + Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtVersionApp.Text = Session.VersionApp;
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
                infoTotalProductsFromToday.Text = "HOY SE HAN CONCRETADO " + implVenta.GetTodayProducts(DateTime.Today) + " VENTAS.";
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
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        private void tglDarkMode_Click(object sender, RoutedEventArgs e)
        {
            darkMode(tglDarkMode.IsChecked.Value);
        }
        private void darkMode(bool vof)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = vof ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            SelectDeudores();
        }
        private void SelectDeudores()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectSalesWithPendingBalanceByCustomers().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implVenta.SelectSalesWithPendingBalanceByCustomers().Rows.Count;
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
                    Session.IdCliente = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_DeudasClientes winVenta_DeudasClientes = new winVenta_DeudasClientes();
                    winVenta_DeudasClientes.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.W)
            {
                Close();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result =
                  MessageBox.Show(
                    "¿Está segur@ de cerrar la sesión actual?",
                    "CERRAR SESIÓN",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                // If user doesn't want to close, cancel closure
                e.Cancel = true;
            }
            else if (result == MessageBoxResult.Yes)
            {
                winLogin winLogin = new winLogin();
                winLogin.Show();
            }
        }
    }
}
