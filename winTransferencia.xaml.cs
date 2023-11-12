using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        SucursalImpl implSucursal;
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
            SelectLike();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxSelectSucursalFromDatabase();
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
            try
            {
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectLikeMovementsHistory(cbxSucursalOrigen.SelectedItem.ToString(), cbxSucursalDestino.SelectedItem.ToString(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeMovementsHistory(cbxSucursalOrigen.SelectedItem.ToString(), cbxSucursalDestino.SelectedItem.ToString(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Select();
            cbxSucursalOrigen.SelectedIndex = 0;
            cbxSucursalDestino.SelectedIndex = 0;
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            DataRowView d = (DataRowView)dgvDetalle.Items[0];
            DateTime fechaRegistro = DateTime.Parse(d.Row.ItemArray[6].ToString());

            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Transferencia_" + fechaRegistro.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteTransferencia.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@NOMBRESUCURSAL", Session.Sucursal_NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAREGISTRO", fechaRegistro.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHAIMPRESION", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
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
        void cbxSelectSucursalFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxSucursal = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implSucursal = new SucursalImpl();
                dataTable = implSucursal.SelectForComboBox();
                listcomboboxSucursal = (from DataRow dr in dataTable.Rows
                                        select new ComboboxItem()
                                        {
                                            Valor = Convert.ToByte(dr["idSucursal"]),
                                            Texto = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursalOrigen.Items.Add(item);
                    cbxSucursalDestino.Items.Add(item);
                }
                cbxSucursalOrigen.SelectedIndex = 0;
                cbxSucursalDestino.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public byte Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, byte valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }

        private void btnCopy2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDetalle);
        }
    }
}
