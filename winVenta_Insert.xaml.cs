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
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

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

        public winVenta_Insert()
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
                                        lblCustomerNombre.Content = "Nombre: " + cliente.Nombre.Trim();
                                        lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                                        lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                                        DisableCustomerButtons();
                                        acbxGetClientesFromDatabase();
                                        btnEditCustomer.IsEnabled = true;
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
                        break;
                        //UPDATE
                    case 2:
                        cliente.Nombre = txtRegister_Nombre.Text.Trim();
                        cliente.NumeroCelular = txtRegister_NumeroCelular.Text.Trim();
                        cliente.NumeroCI = txtRegister_NumeroCI.Text.Trim();
                        implCliente = new ClienteImpl();
                        try
                        {
                            int n = implCliente.Update(cliente);
                            if (n > 0)
                            {
                                lblCustomerNombre.Content = "Nombre: " + cliente.Nombre.Trim();
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
            if (string.IsNullOrEmpty(txtSearchProduct.Text) == false)
            {
                try
                {
                    implProducto = new ProductoImpl();
                    producto = implProducto.GetByCode(txtSearchProduct.Text);
                    if (producto != null)
                    {
                        if (producto.Estado == 1)
                        {
                            if (producto.IdSucursal == Session.Sucursal_IdSucursal)
                            {
                                try
                                {
                                    implCategoria = new CategoriaImpl();
                                    categoria = implCategoria.Get(producto.IdCategoria);
                                    if (categoria != null)
                                    {
                                        AddProductAndWarrantyToListAndDataGrid(producto, categoria);

                                        labelClear(lblSearchProductInfo);
                                        lblSearchProductInfo.Content = "";

                                        txtSearchProduct.Text = "";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    throw;
                                }
                            }
                            else
                            {
                                labelWarning(lblSearchProductInfo);
                                lblSearchProductInfo.Content = "EL PRODUCTO " + producto.CodigoSublote + " ESTÁ DISPONIBLE PERO NO SE ENCUENTRA EN ESTA SUCURSAL, POR FAVOR REALICE LA TRANSFERENCIA CORRESPONDIENTE.";
                                txtSearchProduct.Text = "";
                            }
                        }
                        else if (producto.Estado == 2)
                        {
                            labelWarning(lblSearchProductInfo);
                            lblSearchProductInfo.Content = "EL PRODUCTO " + producto.CodigoSublote + " YA FUE VENDIDO Y NO SE ENCUENTRA EN SISTEMA.";
                            txtSearchProduct.Text = "";
                        }
                        else if (producto.Estado == 3)
                        {
                            labelWarning(lblSearchProductInfo);
                            lblSearchProductInfo.Content = "EL PRODUCTO " + producto.CodigoSublote + " ESTÁ EN ESPERA PARA SER CONFIRMADO EN UNA SUCURSAL.";
                            txtSearchProduct.Text = "";
                        }
                        else
                        {
                            labelDanger(lblSearchProductInfo);
                            lblSearchProductInfo.Content = "EL PRODUCTO " + producto.CodigoSublote + " FUE ELIMINADO DEL SISTEMA.";
                            txtSearchProduct.Text = "";
                        }
                    }
                    else
                    {
                        labelWarning(lblSearchProductInfo);
                        lblSearchProductInfo.Content = "PRODUCTO NO ENCONTRADO.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }        
        void AddProductAndWarrantyToListAndDataGrid(Producto producto, Categoria categoria)
        {
            bool validoParaInsercion = true;
            for (int i = 0; i < listaHelper.Count; i++)
            {
                if (producto.CodigoSublote == listaHelper[i].codigoSublote)
                {
                    MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                    validoParaInsercion = false;
                    break;
                }
            }
            if (validoParaInsercion == true)
            {
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
                    garantia = categoria.Garantia
                });

                venta_TotalUSD += producto.PrecioVentaUSD;
                txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                venta_TotalBOB += producto.PrecioVentaBOB;
                txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();

                venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD, 2);
                txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
                txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
            }
        }
        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            if (cliente != null)
            {
                if (listaHelper.Count > 0)
                {
                    if (listaMetodoPagos.Count > 0)
                    {
                        if (venta_saldoBOB > 1 || venta_saldoUSD > 1)
                        {
                            if (MessageBox.Show("ATENCIÓN: El saldo de la venta en USD o Bs. es mayor a cero.\n¿Desea registrar la venta con saldo pendiente?", "REGISTRAR VENTA CON SALDO", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                ExportarVariablesAListas();
                                venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                                implVenta = new VentaImpl();
                                try
                                {
                                    string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                    if (mensaje == "VENTA_EXITOSA")
                                    {
                                        MessageBox.Show("VENTA CON SALDO MAYOR A CERO REGISTRADA EXITOSAMENTE.");
                                        imprimirVenta();
                                        //PDF
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
                        }
                        else
                        {
                            ExportarVariablesAListas();
                            venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                            implVenta = new VentaImpl();
                            try
                            {
                                string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                if (mensaje == "VENTA_EXITOSA")
                                {
                                    MessageBox.Show("VENTA REGISTRADA EXITOSAMENTE.");
                                    imprimirVenta();
                                    //PDF
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
                    }
                    else
                    {
                        if (MessageBox.Show("ATENCIÓN: No ha insertado ningún método de pago.\n¿Desea registrar la venta con saldo pendiente?", "REGISTRAR VENTA CON SALDO", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            ExportarVariablesAListas();
                            venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                            implVenta = new VentaImpl();
                            try
                            {
                                string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                if (mensaje == "VENTA_EXITOSA")
                                {
                                    MessageBox.Show("VENTA CON SALDO PENDIENTE REGISTRADA EXITOSAMENTE.");
                                    imprimirVenta();
                                    //PDF
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
                    }
                }
                else
                {
                    MessageBox.Show("¡No puede registrar la venta sin uno o más productos!");
                }
            }
            else
            {
                MessageBox.Show("¡No puede registrar la venta sin un cliente!");
            }
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
                listaProductos.Add(new Producto(item.idProducto,item.totalproductoUSD,item.totalproductoBOB));
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
            DataGridTextColumn col1 = new DataGridTextColumn
            {
                Header = "Monto ($.)",
                Binding = new Binding("montoUSD")
            };
            DataGridTextColumn col2 = new DataGridTextColumn
            {
                Header = "Monto (Bs.)",
                Binding = new Binding("montoBOB")
            };
            DataGridTextColumn col3 = new DataGridTextColumn
            {
                Header = "Tipo",
                Binding = new Binding("tipo")
            };
            dgvMetodosPago.Columns.Add(col1);
            dgvMetodosPago.Columns.Add(col2);
            dgvMetodosPago.Columns.Add(col3);
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
                        lblCustomerNombre.Content = "Nombre: " + cliente.Nombre.Trim();
                        lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                        lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
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
                        implCliente = new ClienteImpl();
                        cliente = implCliente.Get((acbtxtNameCustomer.SelectedItem as ComboboxItem).Valor);
                        if (cliente != null)
                        {
                            stackpanelCustomerFound.Visibility = Visibility.Visible;
                            lblCustomerNombre.Content = "Nombre: " + cliente.Nombre.Trim();
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
        }
        private void txtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchProductByCode();
            }
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9,-]+"); //regex that matches disallowed text
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

                venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD,2);
                txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB, 2);
                txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                //Vaciando los txt del método de pago de dólar y boliviano.
                txtPagoUSD.Text = "";
                txtPagoBOB.Text = "";
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
                throw;
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

                    listaHelper.RemoveAt(dgvProductos.SelectedIndex);
                }
            }
        }
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProductos.ItemsSource = listaHelper;
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
                SumarTotalySaldo(dgvProductos.SelectedIndex);
                dgvProductos.ItemsSource = null;
                dgvProductos.ItemsSource = listaHelper;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void ModificarFilaPorDescuentoPorcentaje(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.precioUSD / 100 * (100 - Session.Ajuste_Limite_Descuento),2); ;
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
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarFilaPorDescuentoUSD(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.precioUSD / 100 * (100 - Session.Ajuste_Limite_Descuento),2); ;
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
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarFilaPorDescuentoBOB(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.precioUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
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
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarFilaPorTotalUSD(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.precioUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
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
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarFilaPorTotalBOB(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            double limite = Math.Round(fila.precioUSD / 100 * (100 - Session.Ajuste_Limite_Descuento), 2); ;
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
            }
            else
            {
                MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                        "PRODUCTO: " + fila.codigoSublote + " " + fila.nombreProducto + "\n" +
                        "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                        "PRECIO INGRESADO: " + pago.ToString() + " USD.");
            }
        }
        private void ModificarGarantia(int i, TextBox n, DataGridRowDetalleHelper fila)
        {
            listaHelper[i].garantia = byte.Parse(n.Text.ToString());
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
            txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
            txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();
            txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
            txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
        }
    }
}
