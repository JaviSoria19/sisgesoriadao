﻿using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;//ADO.NET
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
        List<short> listaCantidad = new List<short>();
        CotizacionImpl implCotizacion;
        Cotizacion cotizacion;
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        public winCotizacion_Insert()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            acbxGetProductosFromDatabase();
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtNombreCliente.Focus();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                if (listaHelper.Count > 0)
                {
                    if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \nCantidad de productos ingresados en la cotización: " + listaHelper.Count + ". \nPresione SI para continuar.", "Confirmar cotización", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        listaProductos.Clear();
                        listaCantidad.Clear();
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
                        string mensaje = implCotizacion.InsertTransaction(listaProductos, cotizacion, listaCantidad);
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
        private void acbtxtNombreProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (acbtxtNombreProducto.SelectedItem != null)
            {
                if (e.Key == Key.Enter)
                {
                    bool validoParaInsercion = true;
                    for (int i = 0; i < listaHelper.Count; i++)
                    {
                        if (acbtxtNombreProducto.Text == listaHelper[i].nombreProducto)
                        {
                            MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                            validoParaInsercion = false;
                            break;
                        }
                    }
                    if (validoParaInsercion == true)
                    {
                        try
                        {
                            implProducto = new ProductoImpl();
                            producto = implProducto.Get((acbtxtNombreProducto.SelectedItem as ComboboxItem).Valor);
                            if (producto != null)
                            {
                                if (tglOcultarUSD.IsChecked == true)
                                {
                                    listaHelper.Add(new DataGridRowDetalleHelper
                                    {
                                        idProducto = producto.IdProducto,
                                        nombreProducto = producto.NombreProducto,
                                        precioUSD = producto.PrecioVentaUSD,
                                        precioBOB = producto.PrecioVentaBOB,
                                        cantidad = 1,
                                        totalproductoUSD = 0,
                                        totalproductoBOB = producto.PrecioVentaBOB,
                                        totalcantidadUSD = 0,
                                        totalcantidadBOB = producto.PrecioVentaBOB,
                                        costoUSD = producto.CostoUSD
                                    });
                                }
                                else
                                {
                                    listaHelper.Add(new DataGridRowDetalleHelper
                                    {
                                        idProducto = producto.IdProducto,
                                        nombreProducto = producto.NombreProducto,
                                        precioUSD = producto.PrecioVentaUSD,
                                        precioBOB = producto.PrecioVentaBOB,
                                        cantidad = 1,
                                        totalproductoUSD = producto.PrecioVentaUSD,
                                        totalproductoBOB = producto.PrecioVentaBOB,
                                        totalcantidadUSD = producto.PrecioVentaUSD,
                                        totalcantidadBOB = producto.PrecioVentaBOB,
                                        costoUSD = producto.CostoUSD
                                    });
                                }
                                dgvProductos.Items.Refresh();
                                tglOcultarUSD.IsEnabled = true;
                                acbtxtNombreProducto.Text = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            if (e.Key == Key.Escape)
            {
                (sender as AutoCompleteBox).Text = "";
            }
        }
        private void dtpFechaEntrega_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime fecha = DateTime.Today;
            dtpFechaEntrega.SelectedDate = fecha.AddDays(3);
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
        private void txtClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
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
            if (dgvProductos.SelectedItem != null && dgvProductos.Items.Count > 0)
            {
                if (MessageBox.Show("Está realmente segur@ de remover este producto de la cotización?", "Remover producto", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    listaHelper.RemoveAt(dgvProductos.SelectedIndex);
                }
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
    }
}
