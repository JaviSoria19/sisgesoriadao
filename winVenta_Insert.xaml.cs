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
    /// Lógica de interacción para winVenta_Insert.xaml
    /// </summary>
    public partial class winVenta_Insert : Window
    {
        ClienteImpl implCliente;
        Cliente cliente;
        ProductoImpl implProducto;
        Producto producto;
        CategoriaImpl implCategoria;
        Categoria categoria;
        EmpleadoImpl implEmpleado;
        byte esVentaPorMayor = 0;
        double venta_TotalUSD = 0;
        double venta_TotalBOB = 0;
        double venta_pagoTotalUSD = 0;
        double venta_pagoTotalBOB = 0;
        double venta_saldoUSD = 0;
        double venta_saldoBOB = 0;

        List<double> listaDescuentosPorcentaje = new List<double>();
        List<MetodoPago> listaMetodoPagos = new List<MetodoPago>();
        List<Producto> listaProductos = new List<Producto>();
        List<byte> listaGarantias = new List<byte>();
        VentaImpl implVenta;
        Venta venta;
        byte operacion = 0;
        bool ventaRegistrada = false;

        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();

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
        }
        public class MetodoPagoDataGridView
        {
            public double montoUSD { get; set; }
            public double montoBOB { get; set; }
            public string tipo { get; set; }
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

        public winVenta_Insert()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        void cbxGetEmpleadoFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxEmpleado = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implEmpleado = new EmpleadoImpl();
                dataTable = implEmpleado.SelectForComboBox();
                listcomboboxEmpleado = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = Convert.ToByte(dr["idEmpleado"]),
                                             Texto = dr["empleado"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxEmpleado)
                {
                    cbxEmployees.Items.Add(item);
                }

                cbxEmployees.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            btnSaveNewCustomer.Background = new SolidColorBrush(Colors.LimeGreen);
            btnSaveNewCustomer.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
            btnSaveNewCustomer.Content = "REGISTRAR CLIENTE";
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
            btnSaveNewCustomer.Background = new SolidColorBrush(Colors.Orange);
            btnSaveNewCustomer.BorderBrush = new SolidColorBrush(Colors.Orange);
            btnSaveNewCustomer.Content = "EDITAR CLIENTE";
            btnAddCustomer.IsEnabled = false;
            btnEditCustomer.IsEnabled = false;
            EnableCustomerButtons();
        }
        private void btnSaveNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!CamposClienteValidos())
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
                return;
            }

            implCliente = new ClienteImpl();

            switch (operacion)
            {
                case 1:
                    InsertarNuevoCliente();
                    break;
                case 2:
                    EditarClienteExistente();
                    break;
                default:
                    break;
            }
        }

        private bool CamposClienteValidos()
        {
            return !string.IsNullOrWhiteSpace(txtRegister_Nombre.Text)
                && !string.IsNullOrWhiteSpace(txtRegister_NumeroCelular.Text)
                && !string.IsNullOrWhiteSpace(txtRegister_NumeroCI.Text);
        }

        private void InsertarNuevoCliente()
        {
            cliente = new Cliente(
                txtRegister_Nombre.Text.Trim(),
                txtRegister_NumeroCelular.Text.Trim(),
                txtRegister_NumeroCI.Text.Trim()
            );

            try
            {
                int resultado = implCliente.Insert(cliente);
                if (resultado > 0)
                {
                    MostrarPanelClienteEncontrado();
                    GetUltimoClienteRegistrado();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transacción no completada; comuníquese con el Administrador de Sistemas, error: \n" + ex.Message);
            }
        }

        private void GetUltimoClienteRegistrado()
        {
            try
            {
                implCliente = new ClienteImpl();
                cliente = implCliente.GetLastRegisteredCustomer();

                if (cliente != null)
                {
                    MostrarDatosClienteEnPantalla();
                    DisableCustomerButtons();
                    acbxGetClientesFromDatabase();
                    btnEditCustomer.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transacción no completada; comuníquese con el Administrador de Sistemas, error: \n" + ex.Message);
            }
        }

        private void EditarClienteExistente()
        {
            cliente.Nombre = txtRegister_Nombre.Text.Trim();
            cliente.NumeroCelular = txtRegister_NumeroCelular.Text.Trim();
            cliente.NumeroCI = txtRegister_NumeroCI.Text.Trim();

            try
            {
                int resultado = implCliente.Update(cliente);
                if (resultado > 0)
                {
                    MostrarDatosClienteEnPantalla();
                    MostrarPanelClienteEncontrado();
                    btnAddCustomer.IsEnabled = true;
                    btnEditCustomer.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transacción no completada; comuníquese con el Administrador de Sistemas, error: \n" + ex.Message);
            }
        }

        private void MostrarPanelClienteEncontrado()
        {
            stackpanelCustomerFound.Visibility = Visibility.Visible;
            stackpanelCustomerForm.Visibility = Visibility.Collapsed;
            stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
        }

        private void BuscarYMostrarClientePorCI(string numeroCI)
        {
            try
            {
                implCliente = new ClienteImpl();
                cliente = implCliente.GetByCIorCelular(numeroCI);

                if (cliente != null)
                {
                    MostrarDatosClienteEnPantalla();
                    DisableCustomerButtons();
                    acbxGetClientesFromDatabase();
                    btnEditCustomer.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transacción no completada; comuníquese con el Administrador de Sistemas, error: \n" + ex.Message);
            }
        }

        private void MostrarDatosClienteEnPantalla()
        {
            acbtxtNameCustomer.Text = cliente.Nombre.Trim();
            lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
            lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
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
        private void btnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            SearchProductByCode();
        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductAndWarrantyToListAndDataGrid(producto, categoria);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            txtSearchProduct.Focus();
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

            cbxGetEmpleadoFromDatabase();

            foreach (var item in cbxEmployees.Items)
            {
                if (item is ComboboxItem comboboxItem && comboboxItem.Valor == Session.IdEmpleado)
                {
                    cbxEmployees.SelectedItem = item;
                    break;
                }
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
        void SearchProductByCode()
        {
            string codigo = txtSearchProduct.Text?.Trim();
            if (string.IsNullOrWhiteSpace(codigo))
                return;

            try
            {
                implProducto = new ProductoImpl();
                producto = implProducto.GetByCode(codigo);

                if (producto == null)
                {
                    MostrarMensajeProducto("PRODUCTO NO ENCONTRADO.", MessageType.Warning);
                    return;
                }

                if (producto.Estado != 1)
                {
                    MostrarMensajeEstadoProducto(producto);
                    return;
                }

                if (producto.IdSucursal != Session.Sucursal_IdSucursal)
                {
                    MostrarMensajeProducto($"EL PRODUCTO CON EL CÓDIGO {producto.CodigoSublote} ESTÁ DISPONIBLE PERO NO SE ENCUENTRA EN ESTA SUCURSAL, POR FAVOR REALICE LA TRANSFERENCIA CORRESPONDIENTE.", MessageType.Warning);
                    return;
                }

                if (Session.VerificarProductoEnCola(producto, "VENTA PENDIENTE"))
                {
                    Session.Mensaje_ProductoEnCola(producto);
                    LimpiarCampoBusquedaProducto();
                    return;
                }

                implCategoria = new CategoriaImpl();
                categoria = implCategoria.Get(producto.IdCategoria);

                if (categoria != null)
                {
                    AddProductAndWarrantyToListAndDataGrid(producto, categoria);
                    LimpiarCampoBusquedaProducto();
                    lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + dgvProductos.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        enum MessageType { Success, Warning, Danger }

        void MostrarMensajeProducto(string mensaje, MessageType tipo)
        {
            switch (tipo)
            {
                case MessageType.Warning:
                    labelWarning(lblSearchProductInfo);
                    break;
                case MessageType.Danger:
                    labelDanger(lblSearchProductInfo);
                    break;
                default:
                    labelClear(lblSearchProductInfo);
                    break;
            }

            lblSearchProductInfo.Content = mensaje;
            txtSearchProduct.Text = "";
        }

        void MostrarMensajeEstadoProducto(Producto producto)
        {
            string mensaje = string.Empty;

            if (producto.Estado == 2)
            {
                mensaje = $"EL PRODUCTO CON EL CÓDIGO {producto.CodigoSublote} YA FUE VENDIDO Y NO SE ENCUENTRA DISPONIBLE.";
            }
            else if (producto.Estado == 3)
            {
                mensaje = $"EL PRODUCTO CON EL CÓDIGO {producto.CodigoSublote} ESTÁ EN ESPERA PARA SER CONFIRMADO Y RECIBIDO EN UNA SUCURSAL.";
            }
            else
            {
                mensaje = $"EL PRODUCTO CON EL CÓDIGO {producto.CodigoSublote} FUE ELIMINADO DEL SISTEMA Y NO ESTÁ DISPONIBLE.";
            }

            MostrarMensajeProducto(mensaje, MessageType.Danger);
        }

        void LimpiarCampoBusquedaProducto()
        {
            labelClear(lblSearchProductInfo);
            lblSearchProductInfo.Content = "";
            txtSearchProduct.Text = "";
        }

        void AddProductAndWarrantyToListAndDataGrid(Producto producto, Categoria categoria)
        {
            // Verifica si el producto ya está en la lista usando LINQ para mayor claridad
            bool productoYaExiste = listaHelper.Any(item => item.codigoSublote == producto.CodigoSublote);

            if (productoYaExiste)
            {
                MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                return;
            }

            // Agrega el producto a la lista
            listaHelper.Add(new DataGridRowDetalleHelper
            {
                idProducto = producto.IdProducto,
                codigoSublote = producto.CodigoSublote,
                nombreProducto = producto.NombreProducto,
                identificador = producto.Identificador,
                precioUSD = producto.PrecioVentaUSD,
                precioBOB = producto.PrecioVentaBOB,
                descuentoPorcentaje = 0,
                descuentoUSD = 0,
                descuentoBOB = 0,
                totalproductoUSD = producto.PrecioVentaUSD,
                totalproductoBOB = producto.PrecioVentaBOB,
                garantia = categoria.Garantia,
                costoUSD = producto.CostoUSD
            });

            // Actualiza los totales y saldos
            venta_TotalUSD += producto.PrecioVentaUSD;
            txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
            venta_TotalBOB += producto.PrecioVentaBOB;
            txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();

            venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD, 2);
            txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
            venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
            txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
        }
        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarVenta())
                return;

            if (listaMetodoPagos.Count == 0)
            {
                if (!Confirmar("ATENCIÓN: No ha insertado ningún método de pago.\n¿Desea registrar la venta con saldo pendiente?"))
                    return;
            }
            else if (venta_saldoBOB > 1 || venta_saldoUSD > 1)
            {
                if (!Confirmar("ATENCIÓN: El saldo de la venta en USD o Bs. es mayor a cero.\n¿Desea registrar la venta con saldo pendiente?"))
                    return;
            }

            ExportarVariablesAListas();
            venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, byte.Parse((cbxEmployees.SelectedItem as ComboboxItem).Valor.ToString()),
                              esVentaPorMayor, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
            implVenta = new VentaImpl();

            try
            {
                string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                if (mensaje == "VENTA_EXITOSA")
                {
                    MessageBox.Show(ObtenerMensajeExito(), "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    imprimirVenta();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ValidarVenta()
        {
            if (cliente == null)
            {
                MessageBox.Show("¡No puede registrar la venta sin un cliente!");
                return false;
            }

            if (listaHelper.Count == 0)
            {
                MessageBox.Show("¡No puede registrar la venta sin uno o más productos!");
                return false;
            }

            return true;
        }

        private bool Confirmar(string mensaje)
        {
            return MessageBox.Show(mensaje, "REGISTRAR VENTA CON SALDO", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private string ObtenerMensajeExito()
        {
            if (listaMetodoPagos.Count == 0)
                return "VENTA CON SALDO PENDIENTE REGISTRADA EXITOSAMENTE.";

            if (venta_saldoBOB > 1 || venta_saldoUSD > 1)
                return "VENTA CON SALDO MAYOR A CERO REGISTRADA EXITOSAMENTE.";

            return "VENTA REGISTRADA EXITOSAMENTE.";
        }


        private void ExportarVariablesAListas()
        {
            listaDescuentosPorcentaje.Clear();
            listaGarantias.Clear();
            listaProductos.Clear();
            foreach (var item in listaHelper)
            {
                listaDescuentosPorcentaje.Add(item.descuentoPorcentaje);
                listaGarantias.Add(item.garantia);
                listaProductos.Add(new Producto(item.idProducto, item.totalproductoUSD, item.totalproductoBOB));
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
                try
                {
                    double costoBOB = Math.Round(double.Parse(txtPagoUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                    txtPagoBOB.Text = costoBOB.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtPagoBOB.Text = "";
                    txtPagoUSD.Text = "";
                }
            }
        }
        private void txtPagoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                try
                {
                    double costoUSD = Math.Round(double.Parse(txtPagoBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                    txtPagoUSD.Text = costoUSD.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    txtPagoUSD.Text = "";
                    txtPagoBOB.Text = "";
                }
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
                        stackpanelCustomerFound.Visibility = Visibility.Visible;
                        acbtxtNameCustomer.Text = cliente.Nombre.Trim();
                        lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                        lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                        btnEditCustomer.IsEnabled = true;
                    }
                    else
                    {
                        stackpanelCustomerFound.Visibility = Visibility.Collapsed;
                        stackpanelCustomerForm.Visibility = Visibility.Visible;
                        stackpanelCustomerButtons.Visibility = Visibility.Visible;
                        EnableCustomerButtons();
                        txtRegister_NumeroCI.Text = txtSearchCustomer.Text;
                        operacion = 1;
                        btnSaveNewCustomer.Background = new SolidColorBrush(Colors.LimeGreen);
                        btnSaveNewCustomer.BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        btnSaveNewCustomer.Content = "REGISTRAR CLIENTE";
                        btnEditCustomer.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
            if (e.Key == Key.Escape)
            {
                txtSearchCustomer.Text = "";
            }
        }
        private void txtClear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
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
                        implCliente = new ClienteImpl();
                        cliente = implCliente.Get((acbtxtNameCustomer.SelectedItem as ComboboxItem).Valor);
                        if (cliente != null)
                        {
                            stackpanelCustomerFound.Visibility = Visibility.Visible;
                            acbtxtNameCustomer.Text = cliente.Nombre.Trim();
                            lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                            lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                            btnEditCustomer.IsEnabled = true;
                        }
                        else
                        {
                            btnEditCustomer.IsEnabled = false;
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
                acbtxtNameCustomer.Text = "";
            }
        }
        private void txtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchProductByCode();
            }
            if (e.Key == Key.Escape)
            {
                txtSearchProduct.Text = "";
                txtSearchProduct.Focus();
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
            addPaymentMethodToDataGridandList();
        }
        void addPaymentMethodToDataGridandList()
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                if (double.Parse(txtPagoUSD.Text) != 0 || double.Parse(txtPagoBOB.Text) != 0)
                {
                    //Añadiendo métodos de pago a la tabla y a la lista.
                    dgvMetodosPago.Items.Add(new MetodoPagoDataGridView
                    {
                        montoUSD = double.Parse(txtPagoUSD.Text),
                        montoBOB = double.Parse(txtPagoBOB.Text),
                        tipo = (cbxPaymentMethod.SelectedItem as ComboboxItem).Texto.ToString()
                    });
                    listaMetodoPagos.Add(new MetodoPago(
                        double.Parse(txtPagoUSD.Text),
                        double.Parse(txtPagoBOB.Text),
                        byte.Parse((cbxPaymentMethod.SelectedItem as ComboboxItem).Valor.ToString())
                        ));

                    //Actualizando las cifras de la venta.
                    venta_pagoTotalUSD += double.Parse(txtPagoUSD.Text);
                    txtVentaTotalPagoUSD.Text = venta_pagoTotalUSD.ToString();
                    venta_pagoTotalBOB += double.Parse(txtPagoBOB.Text);
                    txtVentaTotalPagoBOB.Text = venta_pagoTotalBOB.ToString();

                    venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD, 2);
                    txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                    venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
                    txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                    //Vaciando los txt del método de pago de dólar y boliviano.
                    txtPagoUSD.Text = "";
                    txtPagoBOB.Text = "";
                }
                else
                {
                    MessageBox.Show("No puede ingresar CERO como método de pago!.");
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
                if (dgvMetodosPago.Items.IsEmpty != true && listaMetodoPagos != null)
                {
                    if (MessageBox.Show("Está realmente segur@ de remover este método de pago de la venta?", "Remover método de pago", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        venta_pagoTotalUSD -= listaMetodoPagos[posicion].MontoUSD;
                        txtVentaTotalPagoUSD.Text = venta_pagoTotalUSD.ToString();
                        venta_pagoTotalBOB -= listaMetodoPagos[posicion].MontoBOB;
                        txtVentaTotalPagoBOB.Text = venta_pagoTotalBOB.ToString();

                        venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD, 2);
                        txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                        venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
                        txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                        dgvMetodosPago.Items.RemoveAt(posicion);
                        listaMetodoPagos.RemoveAt(posicion);
                    }
                }
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listaHelper.Count > 0 && ventaRegistrada == false)
            {
                MessageBoxResult result =
                  MessageBox.Show(
                    "ATENCIÓN: Se ha agregado uno o más productos a la lista para realizar la venta, ¿Está seguro de cerrar la ventana sin haber registrado la venta?",
                    "Venta pendiente",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // If user doesn't want to close, cancel closure
                    e.Cancel = true;
                }
            }
            foreach (var item in listaHelper)
            {
                Session.RemoverProductoEnCola(item.codigoSublote);
            }
        }
        void imprimirVenta()
        {
            try
            {
                ventaRegistrada = true;
                Session.IdVentaDetalle = implVenta.GetIDAfterInsert();
                winVenta_Detalle winVenta_Detalle = new winVenta_Detalle();
                winVenta_Detalle.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btndgvRemoverProducto(object sender, RoutedEventArgs e)
        {
            if (dgvProductos.SelectedItem != null && dgvProductos.Items.Count > 0)
            {
                if (MessageBox.Show("Está realmente segur@ de remover este producto de la venta?", "Remover producto", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    venta_TotalUSD -= listaHelper[dgvProductos.SelectedIndex].totalproductoUSD;
                    txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                    venta_TotalBOB -= listaHelper[dgvProductos.SelectedIndex].totalproductoBOB;
                    txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();

                    venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD, 2);
                    txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                    venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
                    txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                    Session.RemoverProductoEnCola(listaHelper[dgvProductos.SelectedIndex].codigoSublote);
                    listaHelper.RemoveAt(dgvProductos.SelectedIndex);
                    lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + dgvProductos.Items.Count;
                }
            }
        }
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProductos.ItemsSource = listaHelper;
        }
        enum ColumnaEditable
        {
            DescuentoPorcentaje = 6,
            DescuentoUSD = 7,
            DescuentoBOB = 8,
            TotalUSD = 9,
            TotalBOB = 10,
            Garantia = 11
        }

        private void dgvProductos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int columna = e.Column.DisplayIndex;
            int filaIndex = dgvProductos.SelectedIndex;
            if (filaIndex < 0 || !(e.Row.Item is DataGridRowDetalleHelper fila))
                return;

            if (!(e.EditingElement is TextBox textbox) || !double.TryParse(textbox.Text, out double nuevoValor))
                return;

            try
            {
                RestarTotalySaldo(filaIndex);
                ProcesarEdicionDeCelda((ColumnaEditable)columna, filaIndex, nuevoValor, fila);
                SumarTotalySaldo(filaIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RestaurarValoresAnteriores(filaIndex, fila);
            }

            dgvProductos.ItemsSource = null;
            dgvProductos.ItemsSource = listaHelper;
        }

        private void ProcesarEdicionDeCelda(ColumnaEditable columna, int i, double valor, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.costoUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2);
            double pago = 0;

            switch (columna)
            {
                case ColumnaEditable.DescuentoPorcentaje:
                    pago = Math.Round(fila.precioUSD * (1 - valor / 100), 2);
                    if (!ValidarLimite(pago, limite, fila)) return;

                    listaHelper[i].descuentoPorcentaje = valor;
                    listaHelper[i].totalproductoUSD = pago;
                    listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - valor / 100), 2);
                    listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - pago, 2);
                    listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                    break;

                case ColumnaEditable.DescuentoUSD:
                    pago = Math.Round(fila.precioUSD - valor, 2);
                    if (!ValidarLimite(pago, limite, fila)) return;

                    listaHelper[i].descuentoUSD = valor;
                    listaHelper[i].totalproductoUSD = pago;
                    listaHelper[i].descuentoPorcentaje = Math.Round(valor / fila.precioUSD * 100, 2);
                    listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                    listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                    break;

                case ColumnaEditable.DescuentoBOB:
                    double porcentaje = Math.Round(valor / fila.precioBOB * 100, 2);
                    pago = Math.Round(fila.precioUSD * (1 - porcentaje / 100), 2);
                    if (!ValidarLimite(pago, limite, fila)) return;

                    listaHelper[i].descuentoBOB = valor;
                    listaHelper[i].descuentoPorcentaje = porcentaje;
                    listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB - valor, 2);
                    listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD * (1 - porcentaje / 100), 2);
                    listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                    break;

                case ColumnaEditable.TotalUSD:
                    pago = valor;
                    if (!ValidarLimite(pago, limite, fila)) return;

                    listaHelper[i].totalproductoUSD = valor;
                    listaHelper[i].descuentoPorcentaje = Math.Round(100 - valor / fila.precioUSD * 100, 2);
                    listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - valor, 2);
                    listaHelper[i].totalproductoBOB = Math.Round(fila.precioBOB * (1 - listaHelper[i].descuentoPorcentaje / 100), 2);
                    listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - listaHelper[i].totalproductoBOB, 2);
                    break;

                case ColumnaEditable.TotalBOB:
                    double porcentajeBOB = Math.Round(100 - valor / fila.precioBOB * 100, 2);
                    pago = Math.Round(fila.precioUSD * (1 - porcentajeBOB / 100), 2);
                    if (!ValidarLimite(pago, limite, fila)) return;

                    listaHelper[i].totalproductoBOB = valor;
                    listaHelper[i].descuentoPorcentaje = porcentajeBOB;
                    listaHelper[i].descuentoBOB = Math.Round(fila.precioBOB - valor, 2);
                    listaHelper[i].totalproductoUSD = Math.Round(fila.precioUSD * (1 - porcentajeBOB / 100), 2);
                    listaHelper[i].descuentoUSD = Math.Round(fila.precioUSD - listaHelper[i].totalproductoUSD, 2);
                    break;

                case ColumnaEditable.Garantia:
                    byte garantia = (byte)valor;
                    if (garantia <= 60)
                        listaHelper[i].garantia = garantia;
                    else
                        MessageBox.Show("La garantía no puede superar los 60 meses.");
                    break;
            }
        }

        private bool ValidarLimite(double pago, double limite, DataGridRowDetalleHelper fila)
        {
            if (pago < limite)
            {
                MessageBox.Show($"ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: {Session.Ajuste_Limite_Descuento}% MENOS DEL PRECIO DE VENTA.\n" +
                                $"PRODUCTO: {fila.codigoSublote} {fila.nombreProducto}\n" +
                                $"PRECIO MÍNIMO PERMITIDO: {limite} USD\n" +
                                $"PRECIO INGRESADO: {pago} USD.");
                return false;
            }
            return true;
        }

        private void RestaurarValoresAnteriores(int i, DataGridRowDetalleHelper filaAnterior)
        {
            var actual = listaHelper[i];
            actual.descuentoPorcentaje = filaAnterior.descuentoPorcentaje;
            actual.descuentoUSD = filaAnterior.descuentoUSD;
            actual.descuentoBOB = filaAnterior.descuentoBOB;
            actual.totalproductoUSD = filaAnterior.totalproductoUSD;
            actual.totalproductoBOB = filaAnterior.totalproductoBOB;
            actual.garantia = filaAnterior.garantia;
            SumarTotalySaldo(i);
        }
        private void RestarTotalySaldo(int i)
        {
            venta_TotalUSD -= listaHelper[i].totalproductoUSD;
            venta_TotalBOB -= listaHelper[i].totalproductoBOB;
            venta_saldoUSD -= listaHelper[i].totalproductoUSD;
            venta_saldoBOB -= listaHelper[i].totalproductoBOB;
        }

        private void SumarTotalySaldo(int i)
        {
            venta_TotalUSD += listaHelper[i].totalproductoUSD;
            venta_TotalBOB += listaHelper[i].totalproductoBOB;
            venta_saldoUSD += listaHelper[i].totalproductoUSD;
            venta_saldoBOB += listaHelper[i].totalproductoBOB;

            ActualizarTotalesYTextbox();
        }

        private void ActualizarTotalesYTextbox()
        {
            venta_TotalUSD = Math.Round(venta_TotalUSD, 2);
            venta_TotalBOB = Math.Round(venta_TotalBOB, 2);
            venta_saldoUSD = Math.Round(venta_saldoUSD, 2);
            venta_saldoBOB = Math.Round(venta_saldoBOB, 2);

            txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
            txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
            txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
            txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
        }

        private void txtPagoBOB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                addPaymentMethodToDataGridandList();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        private void txtPagoUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                addPaymentMethodToDataGridandList();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }

        private void btndgvRemoverMetodoPago(object sender, RoutedEventArgs e)
        {
            removeFromDGVPaymentMethod(dgvMetodosPago.SelectedIndex);
        }

        private void tglWholesale_Click(object sender, RoutedEventArgs e)
        {
            if(tglWholesale.IsChecked == true)
            {
                esVentaPorMayor = 1;
            }
            else
            {
                esVentaPorMayor = 0;
            }
        }
    }
}
