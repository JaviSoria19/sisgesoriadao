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
using System.Windows.Data;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winInventarioDigital.xaml
    /// </summary>
    public partial class winInventarioDigital : Window
    {
        ProductoImpl implProducto;

        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        CondicionImpl implCondicion;
        string cadenaFiltroSucursal = string.Empty, cadenaFiltroCondicion = string.Empty, cadenaFiltroCategoria = string.Empty, cadenaFiltroDisponibilidad = string.Empty;
        public winInventarioDigital()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
            /*
            MessageBox.Show(
                "Sucursal: " + (cbxSucursal.SelectedItem as ComboboxItem).Valor + "\n" +
                "Condicion: " + (cbxCondicion.SelectedItem as ComboboxItem).Valor + "\n" +
                "Categoria: " + (cbxCategoria.SelectedItem as ComboboxItem).Valor + "\n" +
                "Disponibilidad: " + (cbxDisponibilidad.SelectedItem as ComboboxItem).Valor
                );
            */
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetGroupConcatSucursal();
            cbxGetSucursalFromDatabase();
            cbxGetGroupConcatCategoria();
            cbxGetCategoriaFromDatabase();
            cbxGetGroupConcatCondicion();
            cbxGetCondicionFromDatabase();

            cbxDisponibilidad.Items.Add(new ComboboxItem("DISPONIBLES", "1"));
            cbxDisponibilidad.Items.Add(new ComboboxItem("ELIMINADOS", "0"));
            cbxDisponibilidad.SelectedIndex = 0;

            txtBuscar.Focus();
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
        void cbxGetGroupConcatCondicion()
        {
            try
            {
                implCondicion = new CondicionImpl();
                string condicionGroupConcatID = implCondicion.SelectGroupConcatIDForComboBox();
                if (condicionGroupConcatID != null)
                {
                    cbxCondicion.Items.Add(new ComboboxItem("TODOS", condicionGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetCondicionFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCondicion = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCondicion = new CondicionImpl();
                dataTable = implCondicion.SelectForComboBox();
                listcomboboxCondicion = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = dr["idCondicion"].ToString(),
                                             Texto = dr["nombreCondicion"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxCondicion)
                {
                    cbxCondicion.Items.Add(item);
                }
                cbxCondicion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLike();
            }
            if (e.Key == Key.Escape)
            {
                CleanText();
            }
        }
        private void tglDisableButtons_Click(object sender, RoutedEventArgs e)
        {
            enableOrDisableButtons();
        }
        void enableOrDisableButtons()
        {
            if (tglDisableButtons.IsChecked == true)
            {
                txtBuscar.IsEnabled = false;
                cbxSucursal.IsEnabled = false;
                cbxCondicion.IsEnabled = false;
                cbxCategoria.IsEnabled = false;
                cbxDisponibilidad.IsEnabled = false;
                btnSearch.IsEnabled = false;
            }
            else
            {
                txtBuscar.IsEnabled = true;
                cbxSucursal.IsEnabled = true;
                cbxCondicion.IsEnabled = true;
                cbxCategoria.IsEnabled = true;
                cbxDisponibilidad.IsEnabled = true;
                btnSearch.IsEnabled = true;
            }
        }
        void SelectLike()
        {
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectLikeInventoryFilter(txtBuscar.Text, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCondicion.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxDisponibilidad.SelectedItem as ComboboxItem).Valor).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeInventoryFilter(txtBuscar.Text, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCondicion.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxDisponibilidad.SelectedItem as ComboboxItem).Valor).Rows.Count;
                cadenaFiltroSucursal = (cbxSucursal.SelectedItem as ComboboxItem).Texto;
                cadenaFiltroCondicion = (cbxCondicion.SelectedItem as ComboboxItem).Texto;
                cadenaFiltroCategoria = (cbxCategoria.SelectedItem as ComboboxItem).Texto;
                cadenaFiltroDisponibilidad = (cbxDisponibilidad.SelectedItem as ComboboxItem).Texto;
                if (dgvDatos.Items.Count > 0)
                {
                    txtCodigoProducto.IsEnabled = true;
                    txtCodigoProducto.Focus();
                    dgvDatosVerificados.ItemsSource = null;
                    tglDisableButtons.IsChecked = true;
                    enableOrDisableButtons();
                    btnPrintPDF.IsEnabled = true;
                }
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
        private void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(txtCodigoProducto.Text.Trim()))
                {
                    foreach (DataRowView item in dgvDatos.Items)
                    {
                        if (item[4].ToString() == txtCodigoProducto.Text.Trim())
                        {//Producto Encontrado
                            DataRowView dtAuxiliar = item;
                            item.Delete();
                            dgvDatosVerificados.Items.Add(dtAuxiliar);
                            lblDataGridRowsVerificados.Content = "TOTAL DE PRODUCTOS VERIFICADOS: " + dgvDatosVerificados.Items.Count.ToString();
                            break;
                        }
                    }
                    txtCodigoProducto.Text = "";
                }
                else
                {
                    MessageBox.Show("Ingrese un parámetro de búsqueda.");
                    txtCodigoProducto.Text = "";
                }
            }
        }

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        void CleanText()
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
        private void dgvDatosVerificados_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn columna1 = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("ID")
            };
            DataGridTextColumn columna2 = new DataGridTextColumn
            {
                Header = "Sucursal",
                Binding = new Binding("Sucursal")
            };
            DataGridTextColumn columna3 = new DataGridTextColumn
            {
                Header = "Categoria",
                Binding = new Binding("Categoria")
            };
            DataGridTextColumn columna4 = new DataGridTextColumn
            {
                Header = "Condicion",
                Binding = new Binding("Condicion")
            };
            DataGridTextColumn columna5 = new DataGridTextColumn
            {
                Header = "Codigo",
                Binding = new Binding("Codigo")
            };
            DataGridTextColumn columna6 = new DataGridTextColumn
            {
                Header = "Producto",
                Binding = new Binding("Producto")
            };
            DataGridTextColumn columna7 = new DataGridTextColumn
            {
                Header = "Identificador",
                Binding = new Binding("Identificador")
            };
            DataGridTextColumn columna8 = new DataGridTextColumn
            {
                Header = "Observaciones",
                Binding = new Binding("Observaciones")
            };
            DataGridTextColumn columna9 = new DataGridTextColumn
            {
                Header = "Disponibilidad",
                Binding = new Binding("Disponibilidad")
            };
            DataGridTextColumn columna10 = new DataGridTextColumn
            {
                Header = "Precio USD",
                Binding = new Binding("Precio USD")
            };
            dgvDatosVerificados.Columns.Add(columna1);
            dgvDatosVerificados.Columns.Add(columna2);
            dgvDatosVerificados.Columns.Add(columna3);
            dgvDatosVerificados.Columns.Add(columna4);
            dgvDatosVerificados.Columns.Add(columna5);
            dgvDatosVerificados.Columns.Add(columna6);
            dgvDatosVerificados.Columns.Add(columna7);
            dgvDatosVerificados.Columns.Add(columna8);
            dgvDatosVerificados.Columns.Add(columna9);
            dgvDatosVerificados.Columns.Add(columna10);
            dgvDatosVerificados.Columns[0].Visibility = Visibility.Collapsed;
        }

        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            generarPDF();
        }
        void generarPDF()
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "InventarioDigital_" + cadenaFiltroSucursal + "_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteInventarioDigital.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", Session.NombreUsuario);
            paginahtml_texto = paginahtml_texto.Replace("@FECHASISTEMA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@SUCURSAL", cadenaFiltroSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@DISPONIBILIDAD", cadenaFiltroDisponibilidad);
            paginahtml_texto = paginahtml_texto.Replace("@CONDICION", cadenaFiltroCondicion);
            paginahtml_texto = paginahtml_texto.Replace("@CATEGORIA", cadenaFiltroCategoria);
            int pdf_contador = 1;
            double columnastotalUSD = 0;
            string filas = string.Empty;
            foreach (DataRowView item in dgvDatos.Items)
            {
                filas += "<tr>";
                filas += "<td>" + pdf_contador + "</td>";//nro
                filas += "<td>" + item[4].ToString() + "</td>";//codigo
                filas += "<td>" + item[5].ToString() + "</td>";//producto
                filas += "<td>" + item[1].ToString() + "</td>";//sucursal
                filas += "<td>" + item[2].ToString() + "</td>";//categoria
                filas += "<td>" + item[3].ToString() + "</td>";//condicion
                filas += "<td>" + item[9].ToString() + "</td>";//precioventa usd
                filas += "<td>" + item[6].ToString() + "</td>";//detalle
                filas += "</tr>";
                pdf_contador++;
                columnastotalUSD += double.Parse(item[9].ToString());
            }
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD_UNO", "$us. " + columnastotalUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@FILAS_UNO", filas);

            int pdf_contador_two = 1;
            double columnastotalUSD_two = 0;
            string filas_two = string.Empty;
            foreach (DataRowView item in dgvDatosVerificados.Items)
            {
                filas_two += "<tr>";
                filas_two += "<td>" + pdf_contador_two + "</td>";//nro
                filas_two += "<td>" + item[4].ToString() + "</td>";//codigo
                filas_two += "<td>" + item[5].ToString() + "</td>";//producto
                filas_two += "<td>" + item[1].ToString() + "</td>";//sucursal
                filas_two += "<td>" + item[2].ToString() + "</td>";//categoria
                filas_two += "<td>" + item[3].ToString() + "</td>";//condicion
                filas_two += "<td>" + item[9].ToString() + "</td>";//precioventa usd
                filas_two += "<td>" + item[6].ToString() + "</td>";//detalle
                filas_two += "</tr>";
                pdf_contador_two++;
                columnastotalUSD_two += double.Parse(item[9].ToString());
            }
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD_DOS", "$us. " + columnastotalUSD_two.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@FILAS_DOS", filas_two);


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
    }
}
