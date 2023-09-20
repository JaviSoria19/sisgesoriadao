using sisgesoriadao.Implementation;
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
using System.Windows.Media;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_Update.xaml
    /// </summary>
    public partial class winVenta_Update : Window
    {
        ClienteImpl implCliente;
        Cliente cliente;
        ProductoImpl implProducto;
        double venta_TotalUSD = 0;
        double venta_TotalBOB = 0;
        double venta_pagoTotalUSD = 0;
        double venta_pagoTotalBOB = 0;
        double venta_saldoUSD = 0;
        double venta_saldoBOB = 0;
        byte operacion = 0;
        int idVenta = 0;
        VentaImpl implVenta;
        List<int> ListaIDProductos = new List<int>();
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        DataGridRowDetalleHelper objetoHelper = null;
        byte modificacionPrecioValida = 0;
        double auxTotalProductoBOB = 0, auxTotalProductoUSD = 0;
        string clipboardTexto = "";
        public winVenta_Update()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            SearchByPhoneorCI();
        }
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            stackpanelCustomerFound.Visibility = Visibility.Collapsed;
            stackpanelCustomerForm.Visibility = Visibility.Visible;
            stackpanelCustomerButtons.Visibility = Visibility.Visible;
            EnableCustomerButtons();
            operacion = 1;
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            stackpanelCustomerFound.Visibility = Visibility.Collapsed;
            stackpanelCustomerForm.Visibility = Visibility.Visible;
            stackpanelCustomerButtons.Visibility = Visibility.Visible;
            txtRegister_Nombre.Text = cliente.Nombre.Trim();
            txtRegister_NumeroCelular.Text = cliente.NumeroCelular.Trim();
            txtRegister_NumeroCI.Text = cliente.NumeroCI.Trim();
            operacion = 2;

            btnAddCustomer.IsEnabled = false;
            btnEditCustomer.IsEnabled = false;
            EnableCustomerButtons();
        }
        private void btnSaveNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRegister_Nombre.Text) == false && string.IsNullOrEmpty(txtRegister_NumeroCelular.Text) == false && string.IsNullOrEmpty(txtRegister_NumeroCI.Text) == false)
            {
                switch (operacion)
                {
                    //insert
                    case 1:
                        if (MessageBox.Show("¿Está seguro de REGISTRAR un nuevo cliente y MODIFICAR el cliente de esta venta?", "REGISTRAR CLIENTE Y MODIFICAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            cliente = new Cliente(txtRegister_Nombre.Text.Trim(), txtRegister_NumeroCelular.Text.Trim(), txtRegister_NumeroCI.Text.Trim());
                            implCliente = new ClienteImpl();
                            try
                            {
                                int n = implCliente.Insert(cliente);
                                if (n > 0)
                                {
                                    stackpanelCustomerFound.Visibility = Visibility.Visible;
                                    stackpanelCustomerForm.Visibility = Visibility.Collapsed;
                                    stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
                                    try
                                    {
                                        implCliente = new ClienteImpl();
                                        cliente = implCliente.GetByCIorCelular(txtRegister_NumeroCI.Text.Trim());
                                        if (cliente != null)
                                        {
                                            DisableCustomerButtons();
                                            acbxGetClientesFromDatabase();
                                            btnEditCustomer.IsEnabled = true;

                                            int modificacion = implCliente.UpdateSaleCustomer(cliente, idVenta);
                                            if (modificacion > 0)
                                            {
                                                getSale_Customer();
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                            }
                        }
                        break;
                    //UPDATE
                    case 2:
                        if (MessageBox.Show("¿Está seguro de MODIFICAR la información del cliente de esta venta?", "MODIFICAR CLIENTE DE VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            cliente.Nombre = txtRegister_Nombre.Text.Trim();
                            cliente.NumeroCelular = txtRegister_NumeroCelular.Text.Trim();
                            cliente.NumeroCI = txtRegister_NumeroCI.Text.Trim();
                            implCliente = new ClienteImpl();
                            try
                            {
                                int n = implCliente.Update(cliente);
                                if (n > 0)
                                {
                                    acbtxtNameCustomer.Text = cliente.Nombre.Trim();
                                    lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                                    lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                                    DisableCustomerButtons();
                                    acbxGetClientesFromDatabase();
                                    stackpanelCustomerFound.Visibility = Visibility.Visible;
                                    stackpanelCustomerForm.Visibility = Visibility.Collapsed;
                                    stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
            }
        }
        private void btnCancelNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            stackpanelCustomerFound.Visibility = Visibility.Visible;
            stackpanelCustomerForm.Visibility = Visibility.Collapsed;
            stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
            DisableCustomerButtons();
            if (operacion == 2)
            {
                btnAddCustomer.IsEnabled = true;
                btnEditCustomer.IsEnabled = true;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            idVenta = Session.IdVentaDetalle;
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtSucursal.Text = Session.Sucursal_NombreSucursal;
            acbxGetClientesFromDatabase();
            cbxPaymentMethod.Items.Add(new ComboboxItem("EFECTIVO", 1));
            cbxPaymentMethod.Items.Add(new ComboboxItem("TRANSFERENCIA BANCARIA", 2));
            cbxPaymentMethod.Items.Add(new ComboboxItem("TARJETA", 3));
            cbxPaymentMethod.SelectedIndex = 0;

            txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
            txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
            txtVentaTotalPagoUSD.Text = venta_pagoTotalUSD.ToString();
            txtVentaTotalPagoBOB.Text = venta_pagoTotalBOB.ToString();
            txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
            txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

            txtObservacionVenta.Text = "-";

            txtIDVenta.Text = "Nro.: " + idVenta.ToString("D5");
            getSale_Customer();
            getSale_Products();
            getSale_Info();

            if (Session.Rol != 1)
            {
                acbtxtNameCustomer.IsEnabled = false;
                txtSearchCustomer.IsEnabled = false;
                btnSearchCustomer.IsEnabled = false;
                btnAddCustomer.IsEnabled = false;
                btnEditCustomer.IsEnabled = false;
                btnUpdateProducts.IsEnabled = false;

                dgvProductos.IsEnabled = false;

                txtPagoUSD.IsEnabled = false;
                txtPagoBOB.IsEnabled = false;
                cbxPaymentMethod.IsEnabled = false;
                btnAddPaymentMethod.IsEnabled = false;
                dgvMetodosPago.IsEnabled = false;

                txtObservacionVenta.IsEnabled = false;
            }
        }
        private void getSale_Info()
        {
            try
            {
                implVenta = new VentaImpl();
                byte estado = implVenta.GetEstado(idVenta);
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
        private void getSale_Customer()
        {
            try
            {
                implCliente = new ClienteImpl();
                cliente = implCliente.GetFromSale(idVenta);
                if (cliente != null)
                {
                    stackpanelCustomerFound.Visibility = Visibility.Visible;
                    acbtxtNameCustomer.Text = cliente.Nombre.Trim();
                    lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                    lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void getSale_Products()
        {
            try
            {
                DataTable dt = new DataTable();
                implVenta = new VentaImpl();
                dt = implVenta.SelectSaleDetails1();
                txtObservacionVenta.Text = dt.Rows[0][20].ToString();

                venta_TotalBOB = double.Parse(dt.Rows[0][18].ToString());
                venta_saldoBOB = double.Parse(dt.Rows[0][19].ToString());
                venta_pagoTotalBOB = venta_TotalBOB - venta_saldoBOB;
                txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
                txtVentaTotalPagoBOB.Text = Math.Round(venta_pagoTotalBOB, 2).ToString();
                txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                venta_TotalUSD = double.Parse(dt.Rows[0][22].ToString());
                venta_saldoUSD = double.Parse(dt.Rows[0][23].ToString());
                venta_pagoTotalUSD = venta_TotalUSD - venta_saldoUSD;
                txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                txtVentaTotalPagoUSD.Text = Math.Round(venta_pagoTotalUSD, 2).ToString();
                txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                txtFechaVenta.Text = dt.Rows[0][21].ToString();
                ListaIDProductos.Clear();
                foreach (DataRow item in dt.Rows)
                {
                    ListaIDProductos.Add(int.Parse(item[24].ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }
        public void labelClear(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Transparent);
            label.Background = new SolidColorBrush(Colors.Transparent);
            label.Content = "";
        }
        public void labelSuccess(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.Background = new SolidColorBrush(Colors.SpringGreen);
        }
        public void labelWarning(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.Background = new SolidColorBrush(Colors.Gold);
        }
        public void labelDanger(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.Background = new SolidColorBrush(Colors.Red);
        }
        void EnableCustomerButtons()
        {
            txtSearchCustomer.IsEnabled = false;
            btnSearchCustomer.IsEnabled = false;

            txtRegister_Nombre.IsEnabled = true;
            txtRegister_NumeroCelular.IsEnabled = true;
            txtRegister_NumeroCI.IsEnabled = true;
            btnSaveNewCustomer.IsEnabled = true;
            btnCancelNewCustomer.IsEnabled = true;
        }
        void DisableCustomerButtons()
        {
            txtSearchCustomer.IsEnabled = true;
            btnSearchCustomer.IsEnabled = true;

            txtRegister_Nombre.IsEnabled = false;
            txtRegister_NumeroCelular.IsEnabled = false;
            txtRegister_NumeroCI.IsEnabled = false;
            btnSaveNewCustomer.IsEnabled = false;
            btnCancelNewCustomer.IsEnabled = false;
        }
        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            imprimirVenta();
        }
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProductos.ItemsSource = listaHelper;
            SelectProductsFromSale();
        }
        private void SelectProductsFromSale()
        {
            try
            {
                DataTable dt = new DataTable();
                implProducto = new ProductoImpl();
                dt = implProducto.SelectProductsFromSale(idVenta);
                listaHelper.Clear();
                foreach (DataRow item in dt.Rows)
                {
                    listaHelper.Add(new DataGridRowDetalleHelper
                    {
                        idProducto = int.Parse(item[0].ToString()),
                        codigoSublote = item[1].ToString(),
                        nombreProducto = item[2].ToString(),
                        identificador = item[3].ToString(),
                        precioUSD = double.Parse(item[4].ToString()),
                        precioBOB = double.Parse(item[5].ToString()),
                        descuentoPorcentaje = double.Parse(item[6].ToString()),
                        descuentoUSD = double.Parse(item[7].ToString()),
                        descuentoBOB = double.Parse(item[8].ToString()),
                        totalproductoUSD = double.Parse(item[9].ToString()),
                        totalproductoBOB = double.Parse(item[10].ToString()),
                        garantia = byte.Parse(item[11].ToString()),
                        costoUSD = double.Parse(item[12].ToString())
                    }
                    );
                }
                foreach (var item in listaHelper)
                {
                    clipboardTexto += item.codigoSublote + " " + item.nombreProducto + " " + item.identificador + "\n";
                }
                clipboardTexto = clipboardTexto.Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void txtPagoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                double costoBOB = Math.Round(double.Parse(txtPagoUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtPagoBOB.Text = costoBOB.ToString();
            }
        }
        private void txtPagoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtPagoBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtPagoUSD.Text = costoUSD.ToString();
            }
        }
        private void dgvMetodosPago_Loaded(object sender, RoutedEventArgs e)
        {
            SelectMetodosPago();
        }
        void SelectMetodosPago()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvMetodosPago.ItemsSource = null;
                dgvMetodosPago.ItemsSource = implVenta.SelectPaymentMethodsFromSale(idVenta).DefaultView;
                dgvMetodosPago.Columns[0].Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void SearchByPhoneorCI()
        {
            if (string.IsNullOrEmpty(txtSearchCustomer.Text) == false)
            {
                try
                {
                    implCliente = new ClienteImpl();
                    cliente = implCliente.GetByCIorCelular(txtSearchCustomer.Text);
                    if (cliente != null)
                    {
                        if (MessageBox.Show("¿Está seguro de CAMBIAR el cliente de esta venta?\n Cliente encontrado por Celular o C.I.: " + cliente.Nombre, "CAMBIAR CLIENTE Y MODIFICAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            stackpanelCustomerFound.Visibility = Visibility.Visible;

                            int modificacion = implCliente.UpdateSaleCustomer(cliente, idVenta);
                            if (modificacion > 0)
                            {
                                getSale_Customer();
                            }
                            btnEditCustomer.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Cliente no encontrado en sistema, ¿Desea registrarlo?", "Registrar nuevo cliente", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            stackpanelCustomerFound.Visibility = Visibility.Collapsed;
                            stackpanelCustomerForm.Visibility = Visibility.Visible;
                            stackpanelCustomerButtons.Visibility = Visibility.Visible;
                            EnableCustomerButtons();
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
        void acbxGetClientesFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCliente = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCliente = new ClienteImpl();
                dataTable = implCliente.SelectCustomerNamesForComboBox();
                listcomboboxCliente = (from DataRow dr in dataTable.Rows
                                       select new ComboboxItem()
                                       {
                                           Valor = Convert.ToInt32(dr["idCliente"]),
                                           Texto = dr["nombre"].ToString()
                                       }).ToList();
                acbtxtNameCustomer.ItemsSource = listcomboboxCliente;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void txtSearchCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchByPhoneorCI();
            }
        }
        private void acbtxtNameCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (acbtxtNameCustomer.SelectedItem != null)
            {
                if (e.Key == Key.Enter)
                {
                    try
                    {
                        if (MessageBox.Show("¿Está seguro de CAMBIAR el cliente de esta venta?", "CAMBIAR CLIENTE Y MODIFICAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            implCliente = new ClienteImpl();
                            cliente = implCliente.Get((acbtxtNameCustomer.SelectedItem as ComboboxItem).Valor);
                            if (cliente != null)
                            {
                                stackpanelCustomerFound.Visibility = Visibility.Visible;

                                int modificacion = implCliente.UpdateSaleCustomer(cliente, idVenta);
                                if (modificacion > 0)
                                {
                                    getSale_Customer();
                                }
                                btnEditCustomer.IsEnabled = true;
                            }
                            else
                            {
                                btnEditCustomer.IsEnabled = false;
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
        private void btnAddPaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            addPaymentMethodToSale();
        }
        void addPaymentMethodToSale()
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                if (MessageBox.Show("¿Está seguro de REGISTRAR un nuevo pago de esta venta?", "REGISTRAR PAGO Y ACTUALIZAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    double pagoUSD, pagoBOB;
                    byte metodoPago;
                    pagoUSD = double.Parse(txtPagoUSD.Text);
                    pagoBOB = double.Parse(txtPagoBOB.Text);
                    metodoPago = byte.Parse((cbxPaymentMethod.SelectedItem as ComboboxItem).Valor.ToString());
                    string insert = implVenta.InsertPaymentMethodTransaction(idVenta, pagoUSD, pagoBOB, metodoPago);
                    if (insert == "INSERTMETODOPAGO_EXITOSO")
                    {
                        MessageBox.Show("METODO DE PAGO REGISTRADO CON ÉXITO.");
                        getSale_Products();
                        SelectMetodosPago();
                        imprimirVenta();
                    }
                    else
                    {
                        MessageBox.Show(insert);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los montos para realizar el pago.");
            }
        }
        private void dgvMetodosPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeFromDGVPaymentMethod(dgvMetodosPago.SelectedIndex);
        }
        void removeFromDGVPaymentMethod(int posicion)
        {
            if (dgvMetodosPago.SelectedItem != null && dgvMetodosPago.Items.Count > 0)
            {
                if (dgvMetodosPago.Items.Count > 0)
                {
                    if (MessageBox.Show("Está realmente segur@ de eliminar este pago de la venta?", "ELIMINAR PAGO Y ACTUALIZAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            DataRowView d = (DataRowView)dgvMetodosPago.SelectedItem;
                            int id = int.Parse(d.Row.ItemArray[0].ToString());
                            double montoUSD, montoBOB;
                            montoUSD = double.Parse(d.Row.ItemArray[1].ToString());
                            montoBOB = double.Parse(d.Row.ItemArray[2].ToString());
                            string insert = implVenta.DeletePaymentMethodTransaction(idVenta, id, montoUSD, montoBOB);
                            if (insert == "DELETEMETODOPAGO_EXITOSO")
                            {
                                MessageBox.Show("METODO DE PAGO ELIMINADO CON ÉXITO.");
                                getSale_Products();
                                SelectMetodosPago();
                                imprimirVenta();
                            }
                            else
                            {
                                MessageBox.Show(insert);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            throw;
                        }
                    }
                    dgvMetodosPago.SelectedItem = null;
                }
            }
        }
        void imprimirVenta()
        {
            try
            {
                Session.IdVentaDetalle = idVenta;
                winVenta_Detalle winVenta_Detalle = new winVenta_Detalle();
                winVenta_Detalle.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void btnDeleteSale_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ATENCIÓN: ¿ESTÁ SEGUR@ DE ELIMINAR LA VENTA?\nSi es que si, justifiquelo debajo de esta ventana en el cuadro de OBSERVACIONES, esta acción hará que todos los productos involucrados retornen al SISTEMA.", "ELIMINAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (txtObservacionVenta.Text != "-")
                {
                    string deletetransaction = implVenta.DeleteSaleTransaction(idVenta, txtObservacionVenta.Text, ListaIDProductos);
                    if (deletetransaction == "DELETEVENTA_EXITOSO")
                    {
                        MessageBox.Show("LA VENTA HA SIDO ELIMINADA CON ÉXITO, LOS PRODUCTOS HAN RETORNADO AL SISTEMA.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(deletetransaction);
                    }
                }
                else
                {
                    MessageBox.Show("LA OBSERVACIÓN PARA LA ELIMINACIÓN DE LA VENTA NO PUEDE QUEDAR VACÍA!");
                }
            }
        }
        private void txtPagoBOB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                addPaymentMethodToSale();
            }
        }
        private void txtPagoUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                addPaymentMethodToSale();
            }
        }
        private void dgvProductos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int indexSeleccionado = e.Column.DisplayIndex;
            DataGridRowDetalleHelper filaSeleccionada = e.Row.Item as DataGridRowDetalleHelper;
            TextBox valorNuevo = e.EditingElement as TextBox;  // Assumes columns are all TextBoxes
            try
            {
                RestarTotalySaldo(dgvProductos.SelectedIndex);
                if (indexSeleccionado == 6)
                {
                    ModificarFilaPorDescuentoPorcentaje(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 7)
                {
                    ModificarFilaPorDescuentoUSD(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 8)
                {
                    ModificarFilaPorDescuentoBOB(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 9)
                {
                    ModificarFilaPorTotalUSD(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 10)
                {
                    ModificarFilaPorTotalBOB(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                else if (indexSeleccionado == 11)
                {
                    ModificarGarantia(dgvProductos.SelectedIndex, valorNuevo, filaSeleccionada);
                }
                SumarTotalySaldo(dgvProductos.SelectedIndex, modificacionPrecioValida);
                dgvProductos.ItemsSource = null;
                dgvProductos.ItemsSource = listaHelper;
                /*imprimirVenta();*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }
        private void ModificarFilaPorDescuentoPorcentaje(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2);
            double pago = 0;
            pago = Math.Round(fila.precioUSD * (1 - double.Parse(n.Text.ToString()) / 100), 2);
            if (pago >= limite)
            {
                //Asignación del porcentaje de descuento.
                listaHelper[i].descuentoPorcentaje = double.Parse(n.Text.ToString());
                //Asignación del costo total del producto con descuento.
                listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                //Asignación del efectivo reducido del costo total del producto.
                listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                modificacionPrecioValida = 1;
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                modificacionPrecioValida = 0;
            }
        }
        private void ModificarFilaPorDescuentoUSD(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
            double pago = 0;
            pago = Math.Round(fila.precioUSD - double.Parse(n.Text.ToString()), 2);
            if (pago >= limite)
            {
                //Asignación del descuento USD.
                listaHelper[i].descuentoUSD = double.Parse(n.Text.ToString());
                //Asignación del descuento en relación con el total.
                listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD - listaHelper[i].descuentoUSD, 2);
                listaHelper[i].descuentoPorcentaje = Math.Round(listaHelper[i].descuentoUSD / fila.precioUSD * 100, 2);

                listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                modificacionPrecioValida = 1;
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                modificacionPrecioValida = 0;
            }
        }
        private void ModificarFilaPorDescuentoBOB(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
            double pago = 0;
            pago = Math.Round(fila.precioUSD * (1 - Math.Round(double.Parse(n.Text.ToString()) / fila.precioBOB * 100, 2) / 100), 2);
            if (pago >= limite)
            {
                //Asignación del descuento BOB.
                listaHelper[i].descuentoBOB = double.Parse(n.Text.ToString());
                //Asignación del descuento en relación con el total.
                listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB - listaHelper[i].descuentoBOB, 2);
                listaHelper[i].descuentoPorcentaje = Math.Round(listaHelper[i].descuentoBOB / fila.precioBOB * 100, 2);

                listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                modificacionPrecioValida = 1;
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                modificacionPrecioValida = 0;
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

                listaHelper[i].descuentoPorcentaje = Math.Round(100 - listaHelper[i].totalproductoUSD / fila.precioUSD * 100, 2);
                listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                modificacionPrecioValida = 1;
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                modificacionPrecioValida = 0;
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

                listaHelper[i].descuentoPorcentaje = Math.Round(100 - listaHelper[i].totalproductoBOB / fila.precioBOB * 100, 2);
                listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                modificacionPrecioValida = 1;
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                modificacionPrecioValida = 0;
            }
        }
        private void ModificarGarantia(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            listaHelper[i].garantia = byte.Parse(n.Text.ToString());
            modificacionPrecioValida = 2;
        }
        private void RestarTotalySaldo(int i)
        {
            venta_TotalUSD -= listaHelper[i].totalproductoUSD;
            venta_TotalBOB -= listaHelper[i].totalproductoBOB;
            venta_saldoUSD -= listaHelper[i].totalproductoUSD;
            venta_saldoBOB -= listaHelper[i].totalproductoBOB;
            auxTotalProductoUSD = listaHelper[i].totalproductoUSD;
            auxTotalProductoBOB = listaHelper[i].totalproductoBOB;
        }
        private void SumarTotalySaldo(int i, byte modificacionValida)
        {
            if (modificacionPrecioValida == 1)
            {
                venta_TotalUSD += listaHelper[i].totalproductoUSD;
                venta_TotalBOB += listaHelper[i].totalproductoBOB;
                venta_saldoUSD += listaHelper[i].totalproductoUSD;
                venta_saldoBOB += listaHelper[i].totalproductoBOB;
            }
            else
            {
                venta_TotalUSD += auxTotalProductoUSD;
                venta_TotalBOB += auxTotalProductoBOB;
                venta_saldoUSD += auxTotalProductoUSD;
                venta_saldoBOB += auxTotalProductoBOB;
            }
            venta_TotalUSD = Math.Round(venta_TotalUSD, 2);
            venta_TotalBOB = Math.Round(venta_TotalBOB, 2);
            venta_saldoUSD = Math.Round(venta_saldoUSD, 2);
            venta_saldoBOB = Math.Round(venta_saldoBOB, 2);

            txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
            txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
            txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
            txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

            objetoHelper = null;
            objetoHelper = listaHelper[i];
        }
        private void btnUpdateProducts_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de MODIFICAR los precios de esta venta?", "MODIFICAR PRECIO DE PRODUCTOS", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<byte> listaGarantias = new List<byte>();
                List<double> listaDescuentosPorcentaje = new List<double>();
                List<Producto> listaProductos = new List<Producto>();
                listaGarantias.Clear();
                listaDescuentosPorcentaje.Clear();
                listaProductos.Clear();
                foreach (var item in listaHelper)
                {
                    listaGarantias.Add(item.garantia);
                    listaDescuentosPorcentaje.Add(item.descuentoPorcentaje);
                    listaProductos.Add(new Producto
                    {
                        IdProducto = item.idProducto,
                        PrecioVentaUSD = item.totalproductoUSD,
                        PrecioVentaBOB = item.totalproductoBOB,
                    });
                }
                Venta venta = new Venta();
                venta.IdVenta = idVenta;
                venta.TotalUSD = venta_TotalUSD;
                venta.TotalBOB = venta_TotalBOB;
                venta.SaldoUSD = venta_saldoUSD;
                venta.SaldoBOB = venta_saldoBOB;
                try
                {
                    string update = implVenta.UpdateSaleProductsTransaction(venta,listaProductos,listaDescuentosPorcentaje,listaGarantias);
                    if (update == "UPDATEPRODUCTOS_EXITOSO")
                    {
                        MessageBox.Show("PRECIO DE VENTA DE LOS PRODUCTOS MODIFICADO CON ÉXITO.");
                        getSale_Products();
                        SelectMetodosPago();
                        imprimirVenta();
                    }
                    else
                    {
                        MessageBox.Show(update);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void btndgvRemoverProducto(object sender, RoutedEventArgs e)
        {
            objetoHelper = null;
            objetoHelper = listaHelper[dgvProductos.SelectedIndex];
            if (listaHelper.Count > 1)
            {
                if (MessageBox.Show("¿Está seguro de REMOVER el producto seleccionado de esta venta? \n" + objetoHelper.codigoSublote + " " + objetoHelper.nombreProducto, "REMOVER PRODUCTO DE LA VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    RestarTotalySaldo(dgvProductos.SelectedIndex);
                    venta_TotalUSD = Math.Round(venta_TotalUSD, 2);
                    venta_TotalBOB = Math.Round(venta_TotalBOB, 2);
                    venta_saldoUSD = Math.Round(venta_saldoUSD, 2);
                    venta_saldoBOB = Math.Round(venta_saldoBOB, 2);
                    txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                    txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
                    txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                    txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
                    listaHelper.Remove(listaHelper[dgvProductos.SelectedIndex]);
                    dgvProductos.Items.Refresh();

                    Venta venta = new Venta {
                        IdVenta = idVenta,
                        TotalUSD = venta_TotalUSD,
                        TotalBOB = venta_TotalBOB,
                        SaldoUSD = venta_saldoUSD,
                        SaldoBOB = venta_saldoBOB
                    };
                    //TRANSACCION PARA ELIMINAR EL PRODUCTO
                    string deletetransaction = implVenta.DeleteAfterSaleProductTransaction(venta, objetoHelper.idProducto);
                    if (deletetransaction == "DELETEPRODUCTO_EXITOSO")
                    {
                        MessageBox.Show("EL PRODUCTO HA SIDO ELIMINADO DE LA VENTA CON ÉXITO Y HA RETORNADO AL SISTEMA.");
                        getSale_Products();
                        SelectMetodosPago();
                        imprimirVenta();
                        objetoHelper = null;
                    }
                    else
                    {
                        MessageBox.Show(deletetransaction);
                    }
                }
            }
            else
            {
                if (MessageBox.Show("La venta cuenta con sólo 1 producto, ¿Desea eliminar la venta?", "ELIMINAR VENTA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (txtObservacionVenta.Text != "-")
                    {
                        string deletetransaction = implVenta.DeleteSaleTransaction(idVenta, txtObservacionVenta.Text, ListaIDProductos);
                        if (deletetransaction == "DELETEVENTA_EXITOSO")
                        {
                            MessageBox.Show("LA VENTA HA SIDO ELIMINADA CON ÉXITO, EL PRODUCTO HA RETORNADO AL SISTEMA.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(deletetransaction);
                        }
                    }
                    else
                    {
                        MessageBox.Show("LA OBSERVACIÓN PARA LA ELIMINACIÓN DE LA VENTA NO PUEDE QUEDAR VACÍA!");
                    }
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
            MessageBox.Show("¡Se ha copiado la descripción de los Productos e IMEI's o S/N en el portapapeles!");
            Clipboard.SetText(clipboardTexto);
        }

        public class DataGridRowDetalleHelper
        {
            public int idProducto { get; set; }
            public string codigoSublote { get; set; }
            public string nombreProducto { get; set; }
            public string identificador { get; set; }
            public double precioUSD { get; set; }
            public double precioBOB { get; set; }
            public double descuentoPorcentaje { get; set; }
            public double descuentoUSD { get; set; }
            public double descuentoBOB { get; set; }
            public double totalproductoUSD { get; set; }
            public double totalproductoBOB { get; set; }
            public byte garantia { get; set; }
            public double costoUSD { get; set; }
            public DataGridRowDetalleHelper()
            {

            }
        }
    }
}