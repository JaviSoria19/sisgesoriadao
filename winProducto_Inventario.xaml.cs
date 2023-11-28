﻿using iTextSharp.text;
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
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Inventario.xaml
    /// </summary>
    public partial class winProducto_Inventario : Window
    {
        ProductoImpl implProducto;
        //Implementaciones para obtener ID's y Nombres para los Combobox
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        CondicionImpl implCondicion;
        string cadenaFiltroSucursal = string.Empty, cadenaFiltroCondicion = string.Empty, cadenaFiltroCategoria = string.Empty, cadenaFiltroDisponibilidad = string.Empty;
        public winProducto_Inventario()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
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
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            generarPDF();
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
            cbxDisponibilidad.Items.Add(new ComboboxItem("TODOS", "0,1,2"));
            cbxDisponibilidad.Items.Add(new ComboboxItem("DISPONIBLES", "1"));
            cbxDisponibilidad.Items.Add(new ComboboxItem("VENDIDOS", "2"));
            cbxDisponibilidad.Items.Add(new ComboboxItem("ELIMINADOS", "0"));
            cbxDisponibilidad.SelectedIndex = 1;
            txtBuscar.Focus();
            if (Session.Rol != 1)
            {
                dgvDatos.Columns[12].Visibility = Visibility.Collapsed;
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

        private void btnVerifyMode_Click(object sender, RoutedEventArgs e)
        {
            winInventarioDigital winInventarioDigital = new winInventarioDigital();
            winInventarioDigital.Show();
            this.Close();
        }

        private void btnInventoryQuantity_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Inventario_Cantidad winProducto_Inventario_Cantidad = new winProducto_Inventario_Cantidad();
            winProducto_Inventario_Cantidad.Show();
            Close();
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
        private void btndgvCopiarCodigoSublote(object sender, RoutedEventArgs e)
        {
            string codigo = (dgvDatos.SelectedItem as DataRowView)[4].ToString();
            MessageBox.Show("Se ha copiado el código de sublote " + codigo + " al portapapeles!");
            Clipboard.SetText(codigo);
        }
        private void btndgvHistorialdeProducto(object sender, RoutedEventArgs e)
        {
            string codigo = (dgvDatos.SelectedItem as DataRowView)[4].ToString();
            Clipboard.SetText(codigo);
            Session.Producto_Historial_CodigoSublote = codigo;
            winProducto_Historial winProducto_Historial = new winProducto_Historial();
            winProducto_Historial.Show();
        }
        private void btndgvModificar(object sender, RoutedEventArgs e)
        {
            string codigo = (dgvDatos.SelectedItem as DataRowView)[4].ToString();
            Clipboard.SetText(codigo);
            Session.Producto_Historial_CodigoSublote = codigo;
            winProducto_Update winProducto_Update = new winProducto_Update();
            winProducto_Update.Show();
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }

        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPDF(dgvDatos, "INVENTARIO");
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
                    btnPrintPDF.IsEnabled = true;
                }
                else
                {
                    btnPrintPDF.IsEnabled = false;
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
        void generarPDF()
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Inventario_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteInventario.ToString();
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
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD", "$us. " + columnastotalUSD.ToString());
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
    }
}
