using MaterialDesignThemes.Wpf;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Windows;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winMainSeller.xaml
    /// </summary>
    public partial class winMainSeller : Window
    {
        VentaImpl implVenta;
        public winMainSeller()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = "Bienvenid@ a " + Session.Sucursal_NombreSucursal + " , " + Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtVersionApp.Text = Session.VersionApp;
            LoadInfoFromDB();
            if (Session.NombreUsuario == "CRAZY STORE")
            {
                btnSaleAdd.IsEnabled = false;
                btnLocalSales.IsEnabled = false;
                btnCashPending.IsEnabled = false;
                btnCashHistory.IsEnabled = false;
                btnQuotationAdd.IsEnabled = false;
                btnQuotations.IsEnabled = false;
                btnTransferProducts.IsEnabled = false;
                btnReceiveProducts.IsEnabled = false;
                btnMovementProductHistory.IsEnabled = false;
                btnHistoryProducts.IsEnabled = false;
                btnCommonSaleAdd.IsEnabled = false;
                btnCommonLocalSales.IsEnabled = false;
                btnRefresh.IsEnabled = false;
            }
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
        private void btnSaleAdd_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Insert winVenta_Insert = new winVenta_Insert();
            winVenta_Insert.Show();
        }
        private void btnLocalSales_Click(object sender, RoutedEventArgs e)
        {
            winVenta_HistorialLocal winVenta_HistorialLocal = new winVenta_HistorialLocal();
            winVenta_HistorialLocal.Show();
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
        private void btnHistoryProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Historial winProducto_Historial = new winProducto_Historial();
            winProducto_Historial.Show();
        }
        private void btnProducts_Inventory_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Inventario winProducto_Inventario = new winProducto_Inventario();
            winProducto_Inventario.Show();
        }
        private void btnCommonSaleAdd_Click(object sender, RoutedEventArgs e)
        {
            winProductoComun_Venta_Insert winProductoComun_Venta_Insert = new winProductoComun_Venta_Insert();
            winProductoComun_Venta_Insert.Show();
        }
        private void btnCommonLocalSales_Click(object sender, RoutedEventArgs e)
        {
            winProductoComun_Venta_Historial winProductoComun_Venta_Historial = new winProductoComun_Venta_Historial();
            winProductoComun_Venta_Historial.Show();
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
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
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
