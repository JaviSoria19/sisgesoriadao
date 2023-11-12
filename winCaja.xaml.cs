using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
            try
            {
                caja = implCaja.GetByBranch();
                if (caja != null)
                {
                    if (dgvDatos.Items.IsEmpty != true)
                    {
                        imprimirPDF(2);
                    }
                    else
                    {
                        MessageBox.Show("¡No puede imprimir el reporte de la caja actual si ésta encuentra vacía!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                dgvDatos.Columns[6].Visibility = Visibility.Collapsed;
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
                            /*imprimirPDF(1);*/
                            totalCajaUSD = 0;
                            totalCajaBOB = 0;
                            Select();

                            Session.IdCaja = caja.IdCaja;
                            Session.Caja_Operacion = 2; //CIERRE DE CAJA
                            winCaja_Detalle winCaja_Detalle = new winCaja_Detalle();
                            winCaja_Detalle.Show();
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
        void imprimirPDF(byte operacion)
        {
            //1 CIERRE
            //2 IMPRESION SOLAMENTE
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();

            if (operacion == 1)//CIERRE
            {
                guardar.FileName = "Caja_" + Session.Sucursal_NombreSucursal + "_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            }
            else
            {
                guardar.FileName = "CajaActiva_" + Session.Sucursal_NombreSucursal + "_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            }

            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteCajaActiva.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", Session.NombreUsuario);
            paginahtml_texto = paginahtml_texto.Replace("@SUCURSAL", Session.Sucursal_NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAAPERTURA", caja.FechaRegistro.ToString("dd/MM/yyyy HH:mm"));
            if (operacion == 1)//CIERRE
            {
                paginahtml_texto = paginahtml_texto.Replace("@FECHACIERRE", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            }
            else
            {
                paginahtml_texto = paginahtml_texto.Replace("@FECHACIERRE", caja.FechaActualizacion);
            }

            paginahtml_texto = paginahtml_texto.Replace("@FECHASISTEMA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            string filas = string.Empty;
            double totalUSD = 0;
            double totalBOB = 0;

            double efectivoUSD = 0, efectivoBOB = 0, transferenciaUSD = 0, transferenciaBOB = 0, tarjetaUSD = 0, tarjetaBOB = 0;
            foreach (DataRowView item in dgvDatos.Items)
            {
                filas += "<tr>";
                filas += "<td>" + item[0].ToString() + "</td>";//usuario
                filas += "<td>" + item[5].ToString() + "</td>";//fecha
                filas += "<td>" + item[1].ToString() + "</td>";//detalle
                filas += "<td>" + item[2].ToString() + "</td>";//tipo
                filas += "<td>" + item[3].ToString() + "</td>";//usd
                filas += "<td>" + item[4].ToString() + "</td>";//bob
                filas += "</tr>";
                totalUSD += double.Parse(item[3].ToString());
                totalBOB += double.Parse(item[4].ToString());
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
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD", "$us. " + totalUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TOTALBOB", "Bs. " + totalBOB.ToString());

            paginahtml_texto = paginahtml_texto.Replace("@EFECTIVO_TOTALUSD", "$us. " + efectivoUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@EFECTIVO_TOTALBOB", "Bs. " + efectivoBOB.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TRANSFERENCIA_TOTALUSD", "$us. " + transferenciaUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TRANSFERENCIA_TOTALBOB", "Bs. " + transferenciaBOB.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TARJETA_TOTALUSD", "$us. " + tarjetaUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TARJETA_TOTALBOB", "Bs. " + tarjetaBOB.ToString());

            string filasVentasPendientes = string.Empty;
            foreach (DataRowView row in dgvVentasPendientes.Items)
            {
                filasVentasPendientes += "<tr>";
                filasVentasPendientes += "<td>" + row[1].ToString() + "</td>";//usuario
                filasVentasPendientes += "<td>" + row[2].ToString() + "</td>";//fecha
                filasVentasPendientes += "<td>" + row[3].ToString() + "</td>";//detalle
                filasVentasPendientes += "<td>" + row[4].ToString() + "</td>";//tipo
                filasVentasPendientes += "</tr>";
            }
            paginahtml_texto = paginahtml_texto.Replace("@VENTASPENDIENTES", filasVentasPendientes);

            if (guardar.ShowDialog() == true)
            {
                try
                {
                    using (FileStream stream = new FileStream(guardar.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        pdfDoc.Add(new Phrase(""));

                        using (StringReader sr = new StringReader(paginahtml_texto))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }
                        pdfDoc.Close();
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dgvVentasPendientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvVentasPendientes.SelectedItem != null && dgvVentasPendientes.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvVentasPendientes.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_Update winVenta_Update = new winVenta_Update();
                    winVenta_Update.Show();
                    dgvVentasPendientes.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }

        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[6].ToString());
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgvDatos.Items.IsEmpty != true)
                {
                    Session.Caja_Operacion = 1;
                    winCaja_Detalle winCaja_Detalle = new winCaja_Detalle();
                    winCaja_Detalle.Show();
                }
                else
                {
                    MessageBox.Show("¡No puede imprimir el reporte de la caja actual si ésta encuentra vacía!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
        private void btnCopy2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvVentasPendientes);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }
        private void btnExcel2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvVentasPendientes);
        }
    }
}
