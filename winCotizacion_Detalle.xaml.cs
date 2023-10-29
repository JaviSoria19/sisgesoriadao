using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCotizacion_Detalle.xaml
    /// </summary>
    public partial class winCotizacion_Detalle : Window
    {
        CotizacionImpl implCotizacion;
        string clipboardTexto = "";
        public winCotizacion_Detalle()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            SelectDetalle();
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
                Focus();
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
                    printDialog.PrintVisual(ZonaImpresionGrid, "RECIBO");
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
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("¡Se ha copiado la descripción de los productos y precios en el portapapeles!");
            Clipboard.SetText(clipboardTexto);
        }
        void SelectDetalle()
        {
            if (Session.IdCotizacion != 0)
            {
                try
                {
                    DataTable dt = new DataTable();
                    implCotizacion = new CotizacionImpl();
                    dt = implCotizacion.SelectDetails2(Session.IdCotizacion);
                    int idVenta = int.Parse(dt.Rows[0][0].ToString());
                    txtIdVenta.Text = "Nro.: " + idVenta.ToString("D5");

                    txtSucursal_nombre.Text = dt.Rows[0][1].ToString();
                    txtSucursal_direccion.Text = dt.Rows[0][2].ToString();
                    txtSucursal_telefono.Text = dt.Rows[0][3].ToString();
                    txtSucursal_correo.Text = dt.Rows[0][4].ToString();

                    txtCliente_nombre.Text = "Cliente: " + dt.Rows[0][5].ToString();
                    txtCliente_celular.Text = "Telefono/Celular: " + dt.Rows[0][10].ToString();
                    txtCliente_nit_ci.Text = "NIT/C.I.: " + dt.Rows[0][7].ToString();
                    txtCliente_empresa.Text = "Empresa: " + dt.Rows[0][6].ToString();
                    txtCliente_direccion.Text = "Dirección: " + dt.Rows[0][8].ToString();
                    txtCliente_correo.Text = "Correo: " + dt.Rows[0][9].ToString();

                    txtCotizacion_fecha.Text = "Fecha: " + dt.Rows[0][12].ToString();
                    txtCotizacion_tiempoEntrega.Text = "Tiempo de validez: " + dt.Rows[0][11].ToString();

                    txtProducto_Descripcion.Text = "";
                    txtProducto_Cantidad.Text = "";
                    txtProducto_PrecioUSD.Text = "";
                    txtProducto_PrecioBOB.Text = "";

                    DataTable dt_two = new DataTable();
                    dt_two = implCotizacion.SelectDetails(Session.IdCotizacion);
                    double totalUSD = 0, totalBOB = 0;
                    foreach (DataRow item in dt_two.Rows)
                    {
                        txtProducto_Descripcion.Text += item[2].ToString() + "\n \n";
                        txtProducto_Cantidad.Text += "______\n \n";
                        txtProducto_PrecioUSD.Text += item[3].ToString() + "\n \n";
                        totalUSD += double.Parse(item[3].ToString());
                        txtProducto_PrecioBOB.Text += item[4].ToString() + "\n \n";
                        totalBOB += double.Parse(item[4].ToString());
                        clipboardTexto += item[2].ToString() + " $. " + item[3].ToString() + " Bs. " + item[4].ToString() + "\n";
                    }
                    txtVenta_TotalUSD.Text = Math.Round(totalUSD, 2) + " $.";
                    txtVenta_TotalBOB.Text = Math.Round(totalBOB, 2) + " Bs.";

                    clipboardTexto = clipboardTexto.Trim();

                    QRCodeEncoder encoder = new QRCodeEncoder();
                    Bitmap bitmap;
                    encoder.QRCodeScale = 8;
                    bitmap = encoder.Encode(clipboardTexto);
                    using (var memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        var bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = memory;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();
                        bitmapimage.Freeze();
                        imgQR.Source = bitmapimage;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
