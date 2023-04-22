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
    /// Lógica de interacción para winTransferencia.xaml
    /// </summary>
    public partial class winTransferencia : Window
    {
        ProductoImpl implProducto;
        int idTransferencia = 0;
        int pdf_contador = 1;
        public winTransferencia()
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
                    idTransferencia = int.Parse(d.Row.ItemArray[0].ToString());
                    SelectDetails(idTransferencia);
                    if (idTransferencia != 0)
                    {
                        btnPrintPDF.IsEnabled = true;
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
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectMovementsHistory().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.SelectMovementsHistory().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectLike()
        {
            if (string.IsNullOrEmpty(txtBuscar.Text)!=true)
            {
                try
                {
                    dgvDatos.ItemsSource = null;
                    dgvDatos.ItemsSource = implProducto.SelectLikeMovementsHistory(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                    dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                    lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeMovementsHistory(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void SelectDetails(int idTransferencia)
        {
            try
            {
                dgvDetalle.ItemsSource = null;
                dgvDetalle.ItemsSource = implProducto.SelectMovementsHistory_Details(idTransferencia).DefaultView;
                dgvDetalle.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridViewDetalles.Content = "NÚMERO DE REGISTROS: " + implProducto.SelectMovementsHistory_Details(idTransferencia).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Transferencia_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteTransferencia.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@NOMBRESUCURSAL",Session.Sucursal_NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", Session.NombreUsuario);
            string filas = string.Empty;
            foreach (DataRowView item in dgvDetalle.Items)
            {
                filas += "<tr>";
                filas += "<td>" + pdf_contador + "</td>";
                filas += "<td>" + item[1].ToString() + "</td>";
                filas += "<td>" + item[2].ToString() + "</td>";
                filas += "<td>" + item[3].ToString() + "</td>";
                filas += "<td>" + item[4].ToString() + "</td>";
                filas += "<td>" + item[5].ToString() + "</td>";
                filas += "</tr>";
                pdf_contador++;
            }
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);


            if (guardar.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(guardar.FileName,FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc,stream);
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
        }
    }
}
