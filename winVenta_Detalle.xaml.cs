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
    /// Lógica de interacción para winVenta_Detalle.xaml
    /// </summary>
    public partial class winVenta_Detalle : Window
    {
        VentaImpl implVenta;
        public winVenta_Detalle()
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
        void SelectDetalle()
        {
            if (Session.IdVentaDetalle != 0)
            {
                try
                {
                    DataTable dt = new DataTable();
                    implVenta = new VentaImpl();
                    dt = implVenta.SelectSaleDetails1();
                    int idVenta = int.Parse(dt.Rows[0][0].ToString());
                    txtIdVenta.Text = "Nro.: " + idVenta.ToString("D5");

                    txtSucursal_nombre.Text = dt.Rows[0][1].ToString();
                    txtSucursal_direccion.Text = dt.Rows[0][2].ToString();
                    txtSucursal_telefono.Text = dt.Rows[0][3].ToString();
                    txtSucursal_correo.Text = dt.Rows[0][4].ToString();

                    txtCliente_nombre.Text = "Cliente: " + dt.Rows[0][5].ToString();
                    txtCliente_celular.Text = "Telefono/Celular: " + dt.Rows[0][6].ToString();
                    txtCliente_ci.Text = "C.I.: " + dt.Rows[0][7].ToString();

                    txtObservaciones.Text = "Observaciones: " + dt.Rows[0][20].ToString() + " | Ejecutivo de ventas: " + dt.Rows[0][8].ToString() + " - Cel.: " + dt.Rows[0][9].ToString();

                    txtVenta_fecha.Text = "Fecha: " + dt.Rows[0][21].ToString();

                    double venta_total = double.Parse(dt.Rows[0][18].ToString());
                    double venta_saldo = double.Parse(dt.Rows[0][19].ToString());
                    double venta_adelanto = venta_total - venta_saldo;
                    txtVenta_Total.Text = venta_total + " Bs.";
                    txtVenta_Adelanto.Text = Math.Round(venta_adelanto, 2) + " Bs.";
                    txtVenta_Saldo.Text = venta_saldo + " Bs.";


                    txtProducto_Descripcion.Text = "";
                    txtProducto_Detalle.Text = "";
                    txtProducto_Garantia.Text = "";
                    txtProducto_Cantidad.Text = "";
                    txtProducto_Precio.Text = "";
                    txtProducto_DescuentoPorcentaje.Text = "";
                    txtProducto_DescuentoBOB.Text = "";
                    txtProducto_TotalBOB.Text = "";
                    foreach (DataRow item in dt.Rows)
                    {
                        if (item[10].ToString().Length > 30)
                        {
                            txtProducto_Descripcion.Text += item[10].ToString() + "\n";
                        }
                        else
                        {
                            txtProducto_Descripcion.Text += item[10].ToString() + "\n \n";
                        }
                        txtProducto_Detalle.Text += item[11].ToString() + "\n \n";
                        txtProducto_Garantia.Text += item[12].ToString() + " Meses\n \n" ;
                        txtProducto_Cantidad.Text += item[13].ToString() + "\n \n";
                        txtProducto_Precio.Text += item[14].ToString() + "\n \n";
                        txtProducto_DescuentoPorcentaje.Text += item[15].ToString() + "\n \n";
                        txtProducto_DescuentoBOB.Text += item[16].ToString() + "\n \n";
                        txtProducto_TotalBOB.Text += item[17].ToString() + "\n \n";
                    }

                    txtPagos.Text = "";
                    DataTable dt_two = new DataTable();
                    dt_two = implVenta.SelectSaleDetails2();
                    if (dt_two.Rows.Count > 0)
                    {
                        foreach (DataRow item_two in dt_two.Rows)
                        {
                            txtPagos.Text += item_two[1].ToString() + "    " + item_two[0].ToString() + "\n";
                        }
                    }
                    else
                    {
                        txtPagos.Text = "-";
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
