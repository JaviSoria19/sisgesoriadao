using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;//ADO.NET
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCotizacion_Update.xaml
    /// </summary>
    public partial class winCotizacion_Update : Window
    {
        CotizacionImpl implCotizacion;
        Cotizacion cotizacion;
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        int idCotizacion = 0;
        List<int> ListaIDProductos = new List<int>();
        List<short> listaCantidad = new List<short>();
        string clipboardTexto = "";
        DataGridRowDetalleHelper objetoHelper = null;
        public winCotizacion_Update()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            SelectDetalle();
            getSale_Info();
        }
        private void getSale_Info()
        {
            try
            {
                byte estado = implCotizacion.GetEstado(idCotizacion);
                if (estado == 0)
                {
                    btnDeleteSale.IsEnabled = false;
                }
                else
                {
                    if (Session.Rol == 1)
                    {
                        btnDeleteSale.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Imprimir();
        }
        void Imprimir()
        {
            Session.IdCotizacion = idCotizacion;
            winCotizacion_Detalle winCotizacion_Detalle = new winCotizacion_Detalle();
            winCotizacion_Detalle.Show();
            Close();
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
                    idCotizacion = int.Parse(dt.Rows[0][0].ToString());
                    txtIDVenta.Text = "Nro.: " + idCotizacion.ToString("D5");
                    txtNombreCliente.Text = dt.Rows[0][5].ToString();
                    txtTelefono.Text = dt.Rows[0][10].ToString();
                    txtNit.Text = dt.Rows[0][7].ToString();
                    txtNombreEmpresa.Text = dt.Rows[0][6].ToString();
                    txtDireccion.Text = dt.Rows[0][8].ToString();
                    txtCorreo.Text = dt.Rows[0][9].ToString();
                    txtFechaVenta.Text = dt.Rows[0][12].ToString();
                    dtpFechaEntrega.SelectedDate = DateTime.Parse(dt.Rows[0][11].ToString());

                    DataTable dt_two = new DataTable();
                    dt_two = implCotizacion.SelectDetails(Session.IdCotizacion);
                    foreach (DataRow item in dt_two.Rows)
                    {
                        listaHelper.Add(new DataGridRowDetalleHelper
                        {
                            idProducto = int.Parse(item[6].ToString()),
                            nombreProducto = item[2].ToString(),
                            precioUSD = double.Parse(item[8].ToString()),
                            precioBOB = double.Parse(item[9].ToString()),
                            cantidad = short.Parse(item[10].ToString()),
                            totalproductoUSD = double.Parse(item[3].ToString()),
                            totalproductoBOB = double.Parse(item[4].ToString()),
                            totalcantidadUSD = double.Parse(item[11].ToString()),
                            totalcantidadBOB = double.Parse(item[12].ToString()),
                            costoUSD = double.Parse(item[7].ToString())
                        }
                        );
                        ListaIDProductos.Add(int.Parse(item[6].ToString()));
                    }
                    if (double.Parse(dt_two.Rows[0][3].ToString()) == 0)
                    {
                        tglOcultarUSD.IsChecked = true;
                    }
                    foreach (var item in listaHelper)
                    {
                        clipboardTexto += item.nombreProducto + " $. " + item.totalproductoUSD + " Bs. " + item.totalproductoBOB + "\n";
                    }
                    clipboardTexto = clipboardTexto.Trim();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void dgvProductos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int indexSeleccionado = e.Column.DisplayIndex;
            DataGridRowDetalleHelper filaSeleccionada = e.Row.Item as DataGridRowDetalleHelper;
            TextBox valorNuevo = e.EditingElement as TextBox;  // Assumes columns are all TextBoxes
            try
            {
                if (indexSeleccionado == 4)
                {
                    ModificarFilaPorCantidad(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 5)
                {
                    ModificarFilaPorTotalUSD(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 6)
                {
                    ModificarFilaPorTotalBOB(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                dgvProductos.ItemsSource = null;
                dgvProductos.ItemsSource = listaHelper;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nDebido a la excepción presentada, se cerrará esta ventana para evitar errores.");
                this.Close();
            }
        }
        private void ModificarFilaPorCantidad(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            short aux = short.Parse(n.Text);
            if (aux > 0 && aux < 1000)
            {
                listaHelper[i].cantidad = aux;
                listaHelper[i].totalcantidadUSD = Math.Round(aux * listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].totalcantidadBOB = Math.Round(aux * listaHelper[i].totalproductoBOB, 2);
            }
            else
            {
                MessageBox.Show("La cantidad ingresada debe ser mayor a 1 y menor 1000");
                listaHelper[i].cantidad = 1;
                listaHelper[i].totalcantidadUSD = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].totalcantidadBOB = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoBOB, 2);
            }
        }
        private void ModificarFilaPorTotalUSD(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
            double pago = 0;
            pago = Math.Round(double.Parse(n.Text.ToString()), 2);
            if (pago >= limite)
            {
                //Asignación del total USD.
                listaHelper[i].totalproductoUSD = double.Parse(n.Text.ToString());
                listaHelper[i].totalproductoBOB = Math.Round(listaHelper[i].totalproductoUSD * Session.Ajuste_Cambio_Dolar, 2);
                listaHelper[i].totalcantidadUSD = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].totalcantidadBOB = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoBOB, 2);
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarFilaPorTotalBOB(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
            double pago = 0;
            pago = Math.Round(fila.precioUSD * (1 - Math.Round(100 - double.Parse(n.Text.ToString()) / fila.precioBOB * 100, 2) / 100), 2);
            if (pago >= limite)
            {
                //Asignación del total USD.
                listaHelper[i].totalproductoBOB = double.Parse(n.Text.ToString());
                listaHelper[i].totalproductoUSD = Math.Round(listaHelper[i].totalproductoBOB / Session.Ajuste_Cambio_Dolar, 2);
                listaHelper[i].totalcantidadUSD = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].totalcantidadBOB = Math.Round(listaHelper[i].cantidad * listaHelper[i].totalproductoBOB, 2);
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void btndgvRemoverProducto(object sender, RoutedEventArgs e)
        {
            objetoHelper = null;
            objetoHelper = listaHelper[dgvProductos.SelectedIndex];
            if (listaHelper.Count > 1)
            {
                if (MessageBox.Show("¿Está seguro de REMOVER el producto seleccionado de esta cotización? \n" + objetoHelper.nombreProducto, "REMOVER PRODUCTO DE LA COTIZACIÓN", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //TRANSACCION PARA ELIMINAR EL PRODUCTO
                    string deletetransaction = implCotizacion.DeleteAfterQuotationProductTransaction(new Cotizacion { IdCotizacion = idCotizacion }, objetoHelper.idProducto);
                    if (deletetransaction == "DELETEPRODUCTO_EXITOSO")
                    {
                        MessageBox.Show("EL PRODUCTO HA SIDO ELIMINADO DE LA COTIZACIÓN.");
                        Imprimir();
                    }
                    else
                    {
                        MessageBox.Show(deletetransaction);
                    }
                }
            }
            else
            {
                if (MessageBox.Show("La cotización cuenta con sólo 1 producto, ¿Desea eliminar la cotización?", "ELIMINAR COTIZACIÓN", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int delete = implCotizacion.Delete(new Cotizacion { IdCotizacion = idCotizacion });
                    if (delete > 0)
                    {
                        MessageBox.Show("LA COTIZACIÓN HA SIDO ELIMINADA CON ÉXITO.");
                        this.Close();
                    }
                }
            }
        }
        private void tglOcultarUSD_Click(object sender, RoutedEventArgs e)
        {
            if (dgvProductos.Items.Count > 0)
            {
                if ((bool)tglOcultarUSD.IsChecked)
                {
                    foreach (var item in listaHelper)
                    {
                        item.totalproductoUSD = 0;
                        item.totalcantidadUSD = 0;
                    }
                }
                else
                {
                    foreach (var item in listaHelper)
                    {
                        item.totalproductoUSD = Math.Round(item.totalproductoBOB / Session.Ajuste_Cambio_Dolar, 2);
                        item.totalcantidadUSD = Math.Round(item.totalproductoUSD * item.cantidad, 2);
                    }
                }
                dgvProductos.Items.Refresh();
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
            dgvProductos.ItemsSource = listaHelper;
        }
        private void txtClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        public class DataGridRowDetalleHelper
        {
            public int idProducto { get; set; }
            public string nombreProducto { get; set; }
            public double precioUSD { get; set; }
            public double precioBOB { get; set; }
            public short cantidad { get; set; }
            public double totalproductoUSD { get; set; }
            public double totalproductoBOB { get; set; }
            public double totalcantidadUSD { get; set; }
            public double totalcantidadBOB { get; set; }
            public double costoUSD { get; set; }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreEmpresa.Text) == true)
            {
                txtNombreEmpresa.Text = "-";
            }
            if (string.IsNullOrEmpty(txtDireccion.Text) == true)
            {
                txtDireccion.Text = "-";
            }
            if (string.IsNullOrEmpty(txtCorreo.Text) == true)
            {
                txtCorreo.Text = "-";
            }
            if (string.IsNullOrEmpty(txtNombreCliente.Text) != true && string.IsNullOrEmpty(txtNit.Text) != true && string.IsNullOrEmpty(txtTelefono.Text) != true)
            {
                cotizacion = new Cotizacion
                {
                    IdCotizacion = idCotizacion,
                    NombreCliente = txtNombreCliente.Text.Trim(),
                    NombreEmpresa = txtNombreEmpresa.Text.Trim(),
                    Nit = txtNit.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    TiempoEntrega = dtpFechaEntrega.SelectedDate.Value
                };
                if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente?\nPresione SI para continuar.", "Confirmar modificación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    List<Producto> listaProductos = new List<Producto>();
                    if (tglOcultarUSD.IsChecked == true)
                    {
                        foreach (var item in listaHelper)
                        {
                            listaProductos.Add(
                                new Producto(
                                    item.idProducto, 0, item.totalproductoBOB
                                    )
                                );
                            listaCantidad.Add(item.cantidad);
                        }
                    }
                    else
                    {
                        foreach (var item in listaHelper)
                        {
                            listaProductos.Add(
                                new Producto(
                                    item.idProducto, item.totalproductoUSD, item.totalproductoBOB
                                    )
                                );
                            listaCantidad.Add(item.cantidad);
                        }
                    }
                    implCotizacion = new CotizacionImpl();
                    string mensaje = implCotizacion.UpdateTransaction(listaProductos, cotizacion, listaCantidad);
                    if (mensaje == "COTIZACION MODIFICADA EXITOSAMENTE.")
                    {
                        MessageBox.Show(mensaje);
                        Imprimir();
                    }
                    else
                    {
                        MessageBox.Show(mensaje);
                    }
                }
            }
            else
            {
                MessageBox.Show("¡Debe rellenar todos los datos obligatorios (*)!");
            }
        }
        private void btnDeleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ATENCIÓN: ¿ESTÁ SEGUR@ DE ELIMINAR LA COTIZACIÓN?", "ELIMINAR COTIZACIÓN", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                int delete = implCotizacion.Delete(new Cotizacion { IdCotizacion = idCotizacion });
                if (delete > 0)
                {
                    MessageBox.Show("LA COTIZACIÓN HA SIDO ELIMINADA CON ÉXITO.");
                    this.Close();
                }
            }
        }
    }
}
