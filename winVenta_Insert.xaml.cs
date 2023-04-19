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
        List<Producto> listaProductos = new List<Producto>();

        VentaImpl implVenta;
        Venta venta;

        List<MetodoPago> listaMetodoPagos = new List<MetodoPago>();
        public winVenta_Insert()
        {
            InitializeComponent();
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
        }
        private void btnSaveNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRegister_Nombre.Text) == false && string.IsNullOrEmpty(txtRegister_NumeroCelular.Text) == false && string.IsNullOrEmpty(txtRegister_NumeroCI.Text) == false)
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
                                lblCustomerNombre.Content = "Nombre: " + cliente.Nombre.Trim();
                                lblCustomerNumeroCelular.Content = "Celular: " + cliente.NumeroCelular.Trim();
                                lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI.Trim();
                                DisableCustomerButtons();
                                acbxGetClientesFromDatabase();
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
        }
        private void btnCancelNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            stackpanelCustomerFound.Visibility = Visibility.Visible;
            stackpanelCustomerForm.Visibility = Visibility.Collapsed;
            stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
            DisableCustomerButtons();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
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

        private void btnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            SearchProductByCode();
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
                        try
                        {
                            implCategoria = new CategoriaImpl();
                            categoria = implCategoria.Get(producto.IdCategoria);
                            if (categoria != null)
                            {
                                txtGarantia.Text = categoria.Garantia.ToString().Trim();
                                //RELLENA LA INFORMACIÓN DEL PRODUCTO EN UN LABEL.
                                lblProductFound.Content = producto.CodigoSublote + " " + producto.NombreProducto + " | " + producto.Identificador;
                                //HABILITA EL CAMPO DEL PRECIO Y EL BOTÓN PARA AÑADIR A LA LISTA.
                                txtPrecioVentaUSD.Text = producto.PrecioVentaUSD.ToString();
                                txtTotalUSD.Text = producto.PrecioVentaUSD.ToString();
                                txtPrecioVentaBOB.Text = producto.PrecioVentaBOB.ToString();
                                txtTotalBOB.Text = producto.PrecioVentaBOB.ToString();
                                btnAddProduct.IsEnabled = true;

                                labelClear(lblSearchProductInfo);
                                lblSearchProductInfo.Content = "";
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

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (producto != null)
            {
                producto.Precio = double.Parse(txtPrecio.Text);
                //ASIGNA EL VALOR TOTAL DE TODOS LOS PRODUCTOS
                if (producto.Moneda == "USD." && cbxCurrency.Text == "BS.")
                {
                    precioTotal += Math.Round((producto.Precio * 6.97),2);
                    txtPrecioTotal.Text = "Bs. " + precioTotal;
                }
                else if(producto.Moneda == "BS." && cbxCurrency.Text == "USD.")
                {
                    precioTotal += Math.Round((producto.Precio / 6.97),2);
                    txtPrecioTotal.Text = "USD. " + precioTotal;
                }
                else if (producto.Moneda == "BS." && cbxCurrency.Text == "BS.")
                {
                    precioTotal += producto.Precio;
                    txtPrecioTotal.Text = "Bs. " + precioTotal;
                }
                else
                {
                    precioTotal += producto.Precio;
                    txtPrecioTotal.Text = "USD. " + precioTotal;
                }
                

                listaProducto.Add(producto);
                dgvProductos.Items.Add(new DataGridRowDetalle { cantidad = 1, producto = producto.NombreProducto + " " + producto.Color, numeroSerie = producto.NumeroSerie, precio = producto.Precio });
                if (btnSaveAndPDF.IsEnabled == false)
                {
                    btnSaveAndPDF.IsEnabled = true;
                    cbxCurrency.IsEnabled = true;
                    cbxPaymentMethod.IsEnabled = true;
                }
                producto = null;
            }
            else
            {
                labelWarning(lblSearchProductInfo);
                lblSearchProductInfo.Content = "NO PUEDE AÑADIR EL MISMO PRODUCTO 2 VECES.";
            }
            */
        }
        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            if (cliente!=null)
            {
                MessageBox.Show(cliente.IdCliente.ToString() + "\n" + cliente.Nombre);
                MessageBox.Show(
                    "TOTAL: \n" +
                    venta_TotalUSD.ToString() + "\n" +
                    venta_TotalBOB.ToString() + "\n" +
                    "ADELANTO: \n" +
                    venta_pagoTotalUSD.ToString() + "\n" +
                    venta_pagoTotalBOB.ToString() + "\n" +
                    "SALDO: \n" +
                    venta_saldoUSD.ToString() + "\n" +
                    venta_saldoBOB.ToString()
                    );
                if (listaMetodoPagos!=null)
                {
                    string cadena = "";
                    foreach (var item in listaMetodoPagos)
                    {
                        cadena += "Pago USD: " + item.MontoUSD.ToString() + "\n" + "Pago BOB:" + item.MontoBOB.ToString() + "\n" + "Tipo: " + item.Tipo.ToString() + "\n";
                    }
                    MessageBox.Show(cadena);
                }
            }
            else
            {
                MessageBox.Show("¡No puede registrar la venta sin un cliente!");
            }
            /*
            venta = new Venta(cliente.IdCliente,Session.IdUsuario,precioTotal,byte.Parse((cbxPaymentMethod.SelectedItem as ComboboxItem).Valor.ToString()), cbxCurrency.Text,Session.Sucursal_IdSucursal);
            implVenta = new VentaImpl();
            string mensaje = implVenta.InsertTransaction(listaProducto, venta);
            if (mensaje == "LA VENTA SE REGISTRÓ CON ÉXITO.")
            {

            }
            else
            {
                MessageBox.Show(mensaje);
            }
            */
        }
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn columna1 = new DataGridTextColumn
            {
                Header = "Codigo",
                Binding = new Binding("codigoSublote")
            };
            DataGridTextColumn columna2 = new DataGridTextColumn
            {
                Header = "Producto",
                Binding = new Binding("nombreProducto")
            };
            DataGridTextColumn columna3 = new DataGridTextColumn
            {
                Header = "Detalle",
                Binding = new Binding("identificador")
            };
            DataGridTextColumn columna4 = new DataGridTextColumn
            {
                Header = "Precio USD",
                Binding = new Binding("precioUSD")
            };
            DataGridTextColumn columna5 = new DataGridTextColumn
            {
                Header = "Precio Bs.",
                Binding = new Binding("precioBOB")
            };
            DataGridTextColumn columna6 = new DataGridTextColumn
            {
                Header = "Desc. %",
                Binding = new Binding("descuentoPorcentaje")
            };
            DataGridTextColumn columna7 = new DataGridTextColumn
            {
                Header = "Desc. $.",
                Binding = new Binding("descuentoUSD")
            };
            DataGridTextColumn columna8 = new DataGridTextColumn
            {
                Header = "Desc. Bs.",
                Binding = new Binding("descuentoBOB")
            };
            DataGridTextColumn columna9 = new DataGridTextColumn
            {
                Header = "Total USD",
                Binding = new Binding("totalproductoUSD")
            };
            DataGridTextColumn columna10 = new DataGridTextColumn
            {
                Header = "Total Bs.",
                Binding = new Binding("totalproductoUSD")
            };
            DataGridTextColumn columna11 = new DataGridTextColumn
            {
                Header = "Garantia",
                Binding = new Binding("garantia")
            };
            dgvProductos.Columns.Add(columna1);
            dgvProductos.Columns.Add(columna2);
            dgvProductos.Columns.Add(columna3);
            dgvProductos.Columns.Add(columna4);
            dgvProductos.Columns.Add(columna5);
            dgvProductos.Columns.Add(columna6);
            dgvProductos.Columns.Add(columna7);
            dgvProductos.Columns.Add(columna8);
            dgvProductos.Columns.Add(columna9);
            dgvProductos.Columns.Add(columna10);
            dgvProductos.Columns.Add(columna11);
        }
        public class DataGridRowDetalle
        {
            public byte cantidad { get; set; }
            public string producto { get; set; }
            public string numeroSerie { get; set; }
            public double precio { get; set; }
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
        public class MetodoPagoDataGridView
        {
            public double montoUSD { get; set; }
            public double montoBOB { get; set; }
            public string tipo { get; set; }
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

                venta_saldoUSD = venta_TotalUSD - venta_pagoTotalUSD;
                txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                venta_saldoBOB = venta_TotalBOB - venta_pagoTotalBOB;
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

        private void txtPrecioUSD_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtPrecioBOB_KeyUp(object sender, KeyEventArgs e)
        {

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

                        venta_saldoUSD = venta_TotalUSD - venta_pagoTotalUSD;
                        txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                        venta_saldoBOB = venta_TotalBOB - venta_pagoTotalBOB;
                        txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                        dgvMetodosPago.Items.RemoveAt(posicion);
                        listaMetodoPagos.RemoveAt(posicion);
                    }
                }
            }
        }
    }
}
