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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCotizacion_Insert.xaml
    /// </summary>
    public partial class winCotizacion_Insert : Window
    {
        ProductoImpl implProducto;
        Producto producto;
        List<Producto> listaProductos = new List<Producto>();
        CotizacionImpl implCotizacion;
        Cotizacion cotizacion;
        public winCotizacion_Insert()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            acbxGetProductosFromDatabase();
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addToDataGrid_andList(producto);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreCliente.Text) != true && string.IsNullOrEmpty(txtNombreEmpresa.Text) != true && string.IsNullOrEmpty(txtNit.Text) != true &&
                string.IsNullOrEmpty(txtDireccion.Text) != true && string.IsNullOrEmpty(txtCorreo.Text) != true && string.IsNullOrEmpty(txtTelefono.Text) != true)
            {
                cotizacion = new Cotizacion(Session.IdUsuario,
                    Session.Sucursal_IdSucursal,
                    txtNombreCliente.Text.Trim(),
                    txtNombreEmpresa.Text.Trim(),
                    txtNit.Text.Trim(),
                    txtDireccion.Text.Trim(),
                    txtCorreo.Text.Trim(),
                    txtTelefono.Text.Trim(),
                    dtpFechaEntrega.SelectedDate.Value
                    );
                if (listaProductos.Count > 0)
                {
                    if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \n Cantidad de productos ingresados en la cotización: " + listaProductos.Count + ". \n Presione SI para continuar.", "Confirmar cotización", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        implCotizacion = new CotizacionImpl();
                        string mensaje = implCotizacion.InsertTransaction(listaProductos, cotizacion);
                        if (mensaje == "COTIZACION REGISTRADA EXITOSAMENTE.")
                        {
                            MessageBox.Show(mensaje);
                            cotizacion = implCotizacion.GetLastFromBranch();
                            if (cotizacion != null)
                            {
                                try
                                {
                                    Session.IdCotizacion = cotizacion.IdCotizacion;
                                    winCotizacion_Detalle winCotizacion_Detalle = new winCotizacion_Detalle();
                                    winCotizacion_Detalle.Show();
                                    Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(mensaje);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("¡Debe ingresar como mínimo 1 producto a la cotización!");
                }
            }
            else
            {
                MessageBox.Show("¡Debe rellenar todos los datos obligatorios (*)!");
            }
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn columna1 = new DataGridTextColumn
            {
                Header = "Producto",
                Binding = new Binding("NombreProducto")
            };
            DataGridTextColumn columna2 = new DataGridTextColumn
            {
                Header = "Precio USD.",
                Binding = new Binding("PrecioVentaUSD")
            };
            DataGridTextColumn columna3 = new DataGridTextColumn
            {
                Header = "Precio Bs.",
                Binding = new Binding("PrecioVentaBOB")
            };
            dgvProductos.Columns.Add(columna1);
            dgvProductos.Columns.Add(columna2);
            dgvProductos.Columns.Add(columna3);
        }
        private void dgvProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeFromDataGridandList(dgvProductos.SelectedIndex);
        }
        private void acbtxtNombreProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (acbtxtNombreProducto.SelectedItem != null)
            {
                if (e.Key == Key.Enter)
                {
                    try
                    {
                        implProducto = new ProductoImpl();
                        producto = implProducto.Get((acbtxtNombreProducto.SelectedItem as ComboboxItem).Valor);
                        if (producto != null)
                        {
                            txtNombreProducto.Text = producto.NombreProducto.Trim();
                            txtPrecioUSD.Text = producto.PrecioVentaUSD.ToString();
                            txtPrecioBOB.Text = producto.PrecioVentaBOB.ToString();

                            txtCotizacionUSD.Text = producto.PrecioVentaUSD.ToString();
                            txtCotizacionBOB.Text = producto.PrecioVentaBOB.ToString();
                            txtCotizacionUSD.IsEnabled = true;
                            txtCotizacionBOB.IsEnabled = true;
                            btnAdd.IsEnabled = true;
                            tglOcultarUSD.IsEnabled = true;
                            acbtxtNombreProducto.Text = "";
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            if (e.Key == Key.Escape)
            {
                (sender as AutoCompleteBox).Text = "";
            }
        }
        private void txtCotizacionUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCotizacionUSD.Text) != true)
            {
                double costoBOB = Math.Round(double.Parse(txtCotizacionUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtCotizacionBOB.Text = costoBOB.ToString();
            }
        }
        private void txtCotizacionBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCotizacionBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtCotizacionBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtCotizacionUSD.Text = costoUSD.ToString();
            }
        }
        private void txtCotizacionBOB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addToDataGrid_andList(producto);
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        private void txtCotizacionUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addToDataGrid_andList(producto);
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        private void dtpFechaEntrega_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaEntrega.SelectedDate = DateTime.Today;
        }
        void acbxGetProductosFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxNombreProducto = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProducto = new ProductoImpl();
                dataTable = implProducto.SelectProductIDandNamesForAutoCompleteBox();
                listcomboboxNombreProducto = (from DataRow dr in dataTable.Rows
                                              select new ComboboxItem()
                                              {
                                                  Valor = Convert.ToInt32(dr["idProducto"]),
                                                  Texto = dr["nombreProducto"].ToString()

                                              }).ToList();
                acbtxtNombreProducto.ItemsSource = listcomboboxNombreProducto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void addToDataGrid_andList(Producto producto)
        {
            bool validoParaInsercion = true;
            for (int i = 0; i < listaProductos.Count; i++)
            {
                if (producto.NombreProducto == listaProductos[i].NombreProducto)
                {
                    MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                    validoParaInsercion = false;
                    break;
                }
            }
            if (validoParaInsercion == true)
            {
                if (string.IsNullOrEmpty(txtCotizacionUSD.Text) != true && string.IsNullOrEmpty(txtCotizacionBOB.Text) != true)
                {
                    if (tglOcultarUSD.IsChecked == true)
                    {
                        producto.PrecioVentaUSD = 0;
                        producto.PrecioVentaBOB = double.Parse(txtCotizacionBOB.Text);
                        dgvProductos.Items.Add(producto);
                        listaProductos.Add(producto);
                    }
                    else
                    {
                        producto.PrecioVentaUSD = double.Parse(txtCotizacionUSD.Text);
                        producto.PrecioVentaBOB = double.Parse(txtCotizacionBOB.Text);
                        dgvProductos.Items.Add(producto);
                        listaProductos.Add(producto);
                    }
                }
            }
        }
        void removeFromDataGridandList(int posicion)
        {
            if (dgvProductos.SelectedItem != null && dgvProductos.Items.Count > 0)
            {
                if (dgvProductos.Items.IsEmpty != true && listaProductos != null)
                {
                    if (MessageBox.Show("Está realmente segur@ de remover este producto de la lista?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        dgvProductos.Items.RemoveAt(posicion);
                        listaProductos.RemoveAt(posicion);
                    }
                }
            }
        }
        void pdf(DataTable dataTable, Cotizacion cotizacion)
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Cotizacion_" + cotizacion.FechaRegistro.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";


            string idCotizaciontextual = String.Format("{0:D5}", cotizacion.IdCotizacion);

            string paginahtml_texto = Properties.Resources.PlantillaReporteCotizacion.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@NOMBRESUCURSAL", Session.Sucursal_NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@DIRECCION", Session.Sucursal_Direccion);
            paginahtml_texto = paginahtml_texto.Replace("@TELEFONO", Session.Sucursal_Telefono);
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
            foreach (DataRow item in dataTable.Rows)
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
        }
        private void txtClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public int Valor { get; set; }
            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, int valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }
    }
}
