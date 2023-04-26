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

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using System.IO;
using iTextSharp.tool.xml;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCotizacion.xaml
    /// </summary>
    public partial class winCotizacion : Window
    {
        Cotizacion cotizacion;
        CotizacionImpl implCotizacion;
        SucursalImpl implSucursal;
        Sucursal sucursal;
        public winCotizacion()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                Select();
            }
            else
            {
                SelectLike();
            }
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Cotizacion_" + cotizacion.FechaRegistro.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";


            string idCotizaciontextual = String.Format("{0:D5}", cotizacion.IdCotizacion);

            string paginahtml_texto = Properties.Resources.PlantillaReporteCotizacion.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@NOMBRESUCURSAL", sucursal.NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@DIRECCION", sucursal.Direccion);
            paginahtml_texto = paginahtml_texto.Replace("@TELEFONO", sucursal.Telefono);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAREGISTRO", cotizacion.FechaRegistro.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHASISTEMA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@IDCOTIZACION", idCotizaciontextual);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_NOMBRECLIENTE", cotizacion.NombreCliente);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_NOMBREEMPRESA", cotizacion.NombreEmpresa);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_NIT", cotizacion.Nit);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_DIRECCION", cotizacion.Direccion);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_CORREO", cotizacion.Correo);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_TELEFONO", cotizacion.Telefono);
            paginahtml_texto = paginahtml_texto.Replace("@COTIZACION_FECHAENTREGA", cotizacion.TiempoEntrega.ToString("dd/MM/yyyy"));
            string filas = string.Empty;
            double total = 0;
            foreach (DataRowView item in dgvDetalle.Items)
            {
                filas += "<tr>";
                filas += "<td> </td>";
                filas += "<td>" + item[2].ToString() + "</td>";
                filas += "<td>" + item[3].ToString() + "</td>";
                filas += "<td>" + item[4].ToString() + "</td>";
                filas += "</tr>";
                total += double.Parse(item[4].ToString());
            }
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);
            paginahtml_texto = paginahtml_texto.Replace("@TOTAL", "Bs. " + total.ToString());


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

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logo, System.Drawing.Imaging.ImageFormat.Png);
                        img.ScaleToFit(90, 90);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.LeftMargin, pdfDoc.Top - 90);
                        pdfDoc.Add(img);
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(txtBuscar.Text))
                {
                    Select();
                }
                else
                {
                    SelectLike();
                }
            }
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    int id = int.Parse(d.Row.ItemArray[0].ToString());
                    cotizacion = implCotizacion.Get(id);
                    if (cotizacion != null)
                    {
                        SelectDetails(cotizacion);
                        implSucursal = new SucursalImpl();
                        sucursal = implSucursal.Get(cotizacion.IdSucursal);
                        if (sucursal != null)
                        {
                            btnPrintPDF.IsEnabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void Select()
        {
            try
            {
                implCotizacion = new CotizacionImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCotizacion.Select().DefaultView;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCotizacion.Select().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectLike()
        {
            if (string.IsNullOrEmpty(txtBuscar.Text) != true)
            {
                try
                {
                    implCotizacion = new CotizacionImpl();
                    dgvDatos.ItemsSource = null;
                    dgvDatos.ItemsSource = implCotizacion.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                    lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implCotizacion.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void SelectDetails(Cotizacion cotizacion)
        {
            try
            {
                dgvDetalle.ItemsSource = null;
                dgvDetalle.ItemsSource = implCotizacion.SelectDetails(cotizacion.IdCotizacion).DefaultView;
                lblDataGridViewDetalles.Content = "NÚMERO DE REGISTROS: " + implCotizacion.SelectDetails(cotizacion.IdCotizacion).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
