using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCaja_Detalle.xaml
    /// </summary>
    public partial class winCaja_Detalle : Window
    {
        CajaImpl implCaja;
        VentaImpl implVenta;
        double totalCajaUSD = 0;
        double totalCajaBOB = 0;
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        public winCaja_Detalle()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            listaHelper.Add(new DataGridRowDetalleHelper
            {
                Tipo = "Efectivo",
                USD = "$us. 0",
                BOB = "Bs. 0"
            });
            listaHelper.Add(new DataGridRowDetalleHelper
            {
                Tipo = "Transferencia",
                USD = "$us. 0",
                BOB = "Bs. 0"
            });
            listaHelper.Add(new DataGridRowDetalleHelper
            {
                Tipo = "Tarjeta",
                USD = "$us. 0",
                BOB = "Bs. 0"
            });
            dgvMetodosPago.ItemsSource = listaHelper;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPrintPDF.IsEnabled = false;
                btnPrint.IsEnabled = false;
                System.Windows.FrameworkElement fe = ZonaImpresionGrid as System.Windows.FrameworkElement;
                if (fe == null)
                    return;

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    //store original scale
                    Transform originalScale = fe.LayoutTransform;
                    //get selected printer capabilities
                    System.Printing.PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                    //get scale of the print wrt to screen of WPF visual
                    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / fe.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                                   fe.ActualHeight);

                    //Transform the Visual to scale
                    fe.LayoutTransform = new ScaleTransform(scale, scale);

                    //get the size of the printer page
                    System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    //update the layout of the visual to the printer page size.
                    fe.Measure(sz);
                    fe.Arrange(new System.Windows.Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                    //now print the visual to printer to fit on the one page.
                    pd.PrintVisual(ZonaImpresionGrid, "My Print");

                    //apply the original transform.
                    fe.LayoutTransform = originalScale;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                btnPrintPDF.IsEnabled = true;
                btnPrint.IsEnabled = true;
            }
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPrintPDF.IsEnabled = false;
                btnPrint.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(ZonaImpresionGrid, "CAJA");
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                btnPrintPDF.IsEnabled = true;
                btnPrint.IsEnabled = true;
            }
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {

            Select();
        }
        private void dgvVentasPendientes_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.Caja_Operacion == 1 || Session.Caja_Operacion == 2)
            {
                SelectVentasConSaldoPendiente();
            }
            else
            {
                stckpnlSaldoPendiente.Visibility = Visibility.Collapsed;
            }
        }
        private void Select()
        {
            try
            {
                implCaja = new CajaImpl();
                dgvDatos.ItemsSource = null;
                if (Session.Caja_Operacion == 1)//REPORTE LOCAL
                {
                    dgvDatos.ItemsSource = implCaja.SelectPendingCashFromBranch().DefaultView;
                    dgvDatos.Columns[6].Visibility = Visibility.Collapsed;
                    lblDataGridRows.Content = "NÚMERO DE VENTAS: " + implCaja.SelectPendingCashFromBranch().Rows.Count;
                    CalcularTotalCaja();
                    txtCaja_Usuario.Text = "Usuario: " + Session.NombreUsuario;
                    txtCaja_Firma_Usuario.Text = Session.NombreUsuario;
                    txtCaja_Sucursal.Text = "Sucursal: " + Session.Sucursal_NombreSucursal;
                    txtCaja_FechaImpresion.Text = "Fecha de Impresión: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                }
                else if (Session.Caja_Operacion == 2)//CIERRE DE CAJA
                {
                    Caja caja = new Caja();
                    caja = implCaja.Get(Session.IdCaja);
                    dgvDatos.ItemsSource = implCaja.SelectDetails(caja).DefaultView;
                    /*dgvDatos.Columns[6].Visibility = Visibility.Collapsed;*/
                    lblDataGridRows.Content = "NÚMERO DE VENTAS: " + implCaja.SelectDetails(caja).Rows.Count;
                    CalcularTotalCaja();
                    txtCaja_Usuario.Text = "Usuario: " + Session.NombreUsuario;
                    txtCaja_Firma_Usuario.Text = Session.NombreUsuario;
                    txtCaja_Sucursal.Text = "Sucursal: " + Session.Sucursal_NombreSucursal;
                    txtCaja_FechaImpresion.Text = "Fecha de Impresión: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    txtCaja_FechaApertura.Text = "Fecha de Apertura: " + caja.FechaRegistro;
                    txtCaja_FechaCierre.Text = "Fecha de Cierre: " + caja.FechaActualizacion;
                }
                else if (Session.Caja_Operacion == 3)//CAJA DESDE REGISTROS
                {
                    Caja caja = new Caja();
                    caja = implCaja.Get(Session.IdCaja);
                    dgvDatos.ItemsSource = implCaja.SelectDetails(caja).DefaultView;
                    lblDataGridRows.Content = "NÚMERO DE VENTAS: " + implCaja.SelectDetails(caja).Rows.Count;
                    CalcularTotalCaja();
                    UsuarioImpl implUsuario;
                    Usuario usuario;
                    SucursalImpl implSucursal;
                    Sucursal sucursal;
                    implUsuario = new UsuarioImpl();
                    usuario = implUsuario.Get(caja.IdUsuario);
                    if (usuario != null)
                    {
                        implSucursal = new SucursalImpl();
                        sucursal = implSucursal.Get(caja.IdSucursal);
                        if (sucursal != null)
                        {
                            txtCaja_Usuario.Text = "Usuario: " + usuario.NombreUsuario;
                            txtCaja_Firma_Usuario.Text = usuario.NombreUsuario;
                            txtCaja_Sucursal.Text = "Sucursal: " + sucursal.NombreSucursal;
                            txtCaja_FechaImpresion.Text = "Fecha de Impresión: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            txtCaja_FechaApertura.Text = "Fecha de Apertura: " + caja.FechaRegistro;
                            txtCaja_FechaCierre.Text = "Fecha de Cierre: " + caja.FechaActualizacion;
                        }
                    }

                }
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
                CalcularTotalSaldo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CalcularTotalCaja()
        {
            if (dgvDatos.Items.Count > 0)
            {
                double efectivoUSD = 0, efectivoBOB = 0, transferenciaUSD = 0, transferenciaBOB = 0, tarjetaUSD = 0, tarjetaBOB = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    totalCajaUSD += double.Parse(item[3].ToString());
                    totalCajaBOB += double.Parse(item[4].ToString());
                    if (item[2].ToString() == "Efectivo")
                    {
                        efectivoUSD += double.Parse(item[3].ToString());
                        efectivoBOB += double.Parse(item[4].ToString());
                    }
                    else if (item[2].ToString() == "Transferencia")
                    {
                        transferenciaUSD += double.Parse(item[3].ToString());
                        transferenciaBOB += double.Parse(item[4].ToString());
                    }
                    else
                    {
                        tarjetaUSD += double.Parse(item[3].ToString());
                        tarjetaBOB += double.Parse(item[4].ToString());
                    }
                }
                txtCajaTotalUSD.Text = "$us. " + totalCajaUSD.ToString();
                txtCajaTotalBOB.Text = "Bs. " + totalCajaBOB.ToString();
                listaHelper[0].USD = "$us. " + Math.Round(efectivoUSD, 2);
                listaHelper[0].BOB = "Bs. " + Math.Round(efectivoBOB, 2);
                listaHelper[1].USD = "$us. " + Math.Round(transferenciaUSD, 2);
                listaHelper[1].BOB = "Bs. " + Math.Round(transferenciaBOB, 2);
                listaHelper[2].USD = "$us. " + Math.Round(tarjetaUSD, 2);
                listaHelper[2].BOB = "Bs. " + Math.Round(tarjetaBOB, 2);
            }
            else
            {
                txtCajaTotalUSD.Text = totalCajaUSD.ToString();
                txtCajaTotalBOB.Text = totalCajaBOB.ToString();
            }
        }
        private void CalcularTotalSaldo()
        {
            if (dgvVentasPendientes.Items.Count > 0)
            {
                double saldoUSD = 0, saldoBOB = 0;
                foreach (DataRowView item in dgvVentasPendientes.Items)
                {
                    saldoUSD += double.Parse(item[3].ToString());
                    saldoBOB += double.Parse(item[4].ToString());
                }
                txtSaldoTotalUSD.Text = "$us. " + Math.Round(saldoUSD, 2);
                txtSaldoTotalBOB.Text = "Bs. " + Math.Round(saldoBOB, 2);
            }
        }
        public class DataGridRowDetalleHelper
        {
            public string Tipo { get; set; }
            public string USD { get; set; }
            public string BOB { get; set; }
        }
    }
}
