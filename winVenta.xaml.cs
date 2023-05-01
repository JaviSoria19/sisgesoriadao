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
    /// Lógica de interacción para winVenta.xaml
    /// </summary>
    public partial class winVenta : Window
    {
        VentaImpl implVenta;
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        UsuarioImpl implUsuario;
        double totalUSD = 0, totalBOB = 0;
        string cadenaFiltroSucursal = string.Empty, cadenaFiltroCategoria = string.Empty, cadenaFiltroUsuario = string.Empty, cadenaFechaInicio = string.Empty, cadenaFechaFin = string.Empty;
        public winVenta()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetGroupConcatSucursal();
            cbxGetSucursalFromDatabase();
            cbxGetGroupConcatCategoria();
            cbxGetCategoriaFromDatabase();
            cbxGetGroupConcatUsuarios();
            cbxGetUsuarioFromDatabase();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        void SelectLike()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasGlobales(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor).DefaultView;
                //dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implVenta.SelectLikeReporteVentasGlobales(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor).Rows.Count;
                totalUSD = 0;
                totalBOB = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    totalUSD += double.Parse(item[8].ToString());
                    totalBOB += double.Parse(item[9].ToString());
                }
                txtTotalUSD.Text = "Total $us.: " + totalUSD.ToString();
                txtTotalBOB.Text = "Total Bs.: " + totalBOB.ToString();
                if (dgvDatos.Items.Count > 0)
                {
                    btnPrintPDF.IsEnabled = true;
                }
                else
                {
                    btnPrintPDF.IsEnabled = false;
                }
                cadenaFiltroSucursal = (cbxSucursal.SelectedItem as ComboboxItem).Texto;
                cadenaFiltroCategoria = (cbxCategoria.SelectedItem as ComboboxItem).Texto;
                cadenaFiltroUsuario = (cbxUsuario.SelectedItem as ComboboxItem).Texto;
                cadenaFechaInicio = dtpFechaInicio.SelectedDate.Value.ToString("dd/MM/yyyy");
                cadenaFechaFin = dtpFechaFin.SelectedDate.Value.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatSucursal()
        {
            try
            {
                implSucursal = new SucursalImpl();
                string sucursalGroupConcatID = implSucursal.SelectGroupConcatIDForComboBox();
                if (sucursalGroupConcatID != null)
                {
                    cbxSucursal.Items.Add(new ComboboxItem("TODOS", sucursalGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetSucursalFromDatabase()
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
                                            Valor = dr["idSucursal"].ToString(),
                                            Texto = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursal.Items.Add(item);
                }
                cbxSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatCategoria()
        {
            try
            {
                implCategoria = new CategoriaImpl();
                string categoriaGroupConcatID = implCategoria.SelectGroupConcatIDForComboBox();
                if (categoriaGroupConcatID != null)
                {
                    cbxCategoria.Items.Add(new ComboboxItem("TODOS", categoriaGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetCategoriaFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCategoria = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCategoria = new CategoriaImpl();
                dataTable = implCategoria.SelectForComboBox();
                listcomboboxCategoria = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = dr["idCategoria"].ToString(),
                                             Texto = dr["nombreCategoria"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxCategoria)
                {
                    cbxCategoria.Items.Add(item);
                }
                cbxCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatUsuarios()
        {
            try
            {
                implUsuario = new UsuarioImpl();
                string usuarioGroupConcatID = implUsuario.SelectGroupConcatIDForComboBox();
                if (usuarioGroupConcatID != null)
                {
                    cbxUsuario.Items.Add(new ComboboxItem("TODOS", usuarioGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetUsuarioFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxUsuario = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implUsuario = new UsuarioImpl();
                dataTable = implUsuario.SelectForComboBox();
                listcomboboxUsuario = (from DataRow dr in dataTable.Rows
                                       select new ComboboxItem()
                                       {
                                           Valor = dr["idUsuario"].ToString(),
                                           Texto = dr["nombreUsuario"].ToString()
                                       }).ToList();
                foreach (var item in listcomboboxUsuario)
                {
                    cbxUsuario.Items.Add(item);
                }
                cbxUsuario.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public string Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, string valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            imprimirPDF();
        }
        private void imprimirPDF()
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Ventas_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteVenta.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", Session.NombreUsuario);
            paginahtml_texto = paginahtml_texto.Replace("@FECHASISTEMA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHAINICIO", cadenaFechaInicio);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAFIN", cadenaFechaFin);
            paginahtml_texto = paginahtml_texto.Replace("@FILTROSUCURSAL", cadenaFiltroSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FILTROCATEGORIA", cadenaFiltroCategoria);
            paginahtml_texto = paginahtml_texto.Replace("@FILTROUSUARIOS", cadenaFiltroUsuario);
            int pdf_contador = 1;
            double columnastotalUSD = 0, columnastotalBOB = 0;
            string filas = string.Empty;
            foreach (DataRowView item in dgvDatos.Items)
            {
                filas += "<tr>";
                filas += "<td>" + pdf_contador + "</td>";//nro
                filas += "<td>" + item[0].ToString() + "</td>";//fecha
                filas += "<td>" + item[1].ToString() + "</td>";//sucursal
                filas += "<td>" + item[2].ToString() + "</td>";//usuario
                filas += "<td>" + item[3].ToString() + "</td>";//nro venta
                filas += "<td>" + item[4].ToString() + "</td>";//codigo producto
                filas += "<td>" + item[5].ToString() + "</td>";//nombre producto
                filas += "<td>" + item[7].ToString() + "</td>";//categoria
                filas += "<td>" + item[8].ToString() + "</td>";//precio USD
                filas += "<td>" + item[9].ToString() + "</td>";//precio BOB
                filas += "</tr>";
                pdf_contador++;
                columnastotalUSD += double.Parse(item[8].ToString());
                columnastotalBOB += double.Parse(item[9].ToString());
            }
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD", "$us. " + columnastotalUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TOTALBOB", "Bs. " + columnastotalBOB.ToString());
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

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logo, System.Drawing.Imaging.ImageFormat.Png);
                        img.ScaleToFit(90, 90);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.LeftMargin + 10, pdfDoc.Top - 100);
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
    }
}
