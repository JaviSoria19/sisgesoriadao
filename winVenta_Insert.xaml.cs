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
        double descuentoPorcentaje = 0;
        List<double> listaDescuentosPorcentaje = new List<double>();
        List<MetodoPago> listaMetodoPagos = new List<MetodoPago>();
        List<Producto> listaProductos = new List<Producto>();
        List<Categoria> listaGarantias = new List<Categoria>();
        VentaImpl implVenta;
        Venta venta;
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
                                        txtGarantia.Text = categoria.Garantia.ToString().Trim();
                                        //RELLENA LA INFORMACIÓN DEL PRODUCTO EN UN LABEL.
                                        lblProductFound.Content = producto.CodigoSublote + " " + producto.NombreProducto + " | " + producto.Identificador;
                                        //HABILITA EL CAMPO DEL PRECIO Y EL BOTÓN PARA AÑADIR A LA LISTA.
                                        txtPrecioVentaUSD.Text = producto.PrecioVentaUSD.ToString();
                                        txtTotalUSD.Text = producto.PrecioVentaUSD.ToString();
                                        txtPrecioVentaBOB.Text = producto.PrecioVentaBOB.ToString();
                                        txtTotalBOB.Text = producto.PrecioVentaBOB.ToString();
                                        txtDescuentoPorcentaje.Text = descuentoPorcentaje.ToString();
                                        //HABILITA BOTÓN PARA AÑADIR PRODUCTO Y CAMPOS DE TEXTO.
                                        btnAddProduct.IsEnabled = true;
                                        txtDescuentoPorcentaje.IsEnabled = true;
                                        txtTotalUSD.IsEnabled = true;
                                        txtTotalBOB.IsEnabled = true;
                                        txtGarantia.IsEnabled = true;

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
            //Validación para comprobar que los campos de asignación de precio no estén vacios.
            if (string.IsNullOrEmpty(txtTotalUSD.Text) != true && string.IsNullOrEmpty(txtTotalBOB.Text) != true)
            {
                bool validoParaInsercion = true;
                for (int i = 0; i < listaProductos.Count; i++)
                {
                    if (producto.CodigoSublote == listaProductos[i].CodigoSublote)
                    {
                        MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                        validoParaInsercion = false;
                        break;
                    }
                }
                if (validoParaInsercion == true)
                {
                    //Guardamos el precio de base de datos en las variables:
                    double producto_precioVentaUSD = producto.PrecioVentaUSD;
                    double producto_precioVentaBOB = producto.PrecioVentaBOB;
                    //creamos nuevas variables para desplegar en la tabla:
                    double producto_precioTotalUSD;
                    double producto_precioTotalBOB;
                    double producto_descuentoUSD;
                    double producto_descuentoBOB;
                    //Validación para que en la venta no se asigne un precio inferior del límite de descuento establecido.
                    double limite = producto.PrecioVentaUSD / 100 * (100 - Session.Ajuste_Limite_Descuento);
                    double pago = 0;
                    pago = double.Parse(txtTotalUSD.Text);
                    if (pago >= limite)
                    {
                        //Asignamos el precio al que se va vender el producto:
                        producto.PrecioVentaUSD = double.Parse(txtTotalUSD.Text);
                        producto.PrecioVentaBOB = double.Parse(txtTotalBOB.Text);

                        //Guardamos el producto con el precio asignado previamente a la lista y también guardamos la garantía ingresada.
                        listaProductos.Add(producto);
                        categoria.Garantia = byte.Parse(txtGarantia.Text);
                        listaGarantias.Add(categoria);

                        double varListaDescuentoPorcentaje = descuentoPorcentaje;
                        listaDescuentosPorcentaje.Add(varListaDescuentoPorcentaje);
                        //Asignamos variables auxiliares para establecer el descuento obtenido en USD y BOB.
                        producto_precioTotalUSD = producto.PrecioVentaUSD;
                        producto_descuentoUSD = Math.Round(producto_precioVentaUSD - producto_precioTotalUSD,2);
                        producto_precioTotalBOB = producto.PrecioVentaBOB;
                        producto_descuentoBOB = Math.Round(producto_precioVentaBOB - producto_precioTotalBOB,2);

                        //Insertamos una nueva fila en el DataGridView de productos.
                        dgvProductos.Items.Add(new DataGridRowDetalle
                        {
                            codigoSublote = producto.CodigoSublote,
                            nombreProducto = producto.NombreProducto,
                            identificador = producto.Identificador,
                            precioUSD = producto_precioVentaUSD,
                            precioBOB = producto_precioVentaBOB,
                            descuentoPorcentaje = descuentoPorcentaje,
                            descuentoUSD = producto_descuentoUSD,
                            descuentoBOB = producto_descuentoBOB,
                            totalproductoUSD = producto.PrecioVentaUSD,
                            totalproductoBOB = producto.PrecioVentaBOB,
                            garantia = categoria.Garantia
                        });

                        venta_TotalUSD += producto.PrecioVentaUSD;
                        txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                        venta_TotalBOB += producto.PrecioVentaBOB;
                        txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();

                        venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD,2);
                        txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                        venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB,2);
                        txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();
                    }
                    else
                    {
                        MessageBox.Show("ATENCIÓN: ESTIMAD@ USUARI@, NO ESTÁ PERMITIDO REBAJAR EL PRECIO POR DEBAJO DEL LÍMITE ESTABLECIDO: " + Session.Ajuste_Limite_Descuento + "% MENOS DEL PRECIO DE VENTA. \n" +
                            "PRODUCTO: " + producto.CodigoSublote + " " + producto.NombreProducto + "\n" +
                            "PRECIO MÍNIMO POR DEBAJO DEL PRECIO DE VENTA: " + limite.ToString() + " USD. \n" +
                            "PRECIO INGRESADO: " + pago.ToString() + " USD.");
                    }
                }
            }
        }
        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            if (cliente!=null)
            {
                if (listaProductos.Count>0)
                {
                    if (listaMetodoPagos.Count>0)
                    {
                        if (venta_saldoBOB > 1 || venta_saldoUSD > 1)
                        {
                            if (MessageBox.Show("ATENCIÓN: El saldo de la venta en USD o Bs. es mayor a cero.\n¿Desea registrar la venta con saldo pendiente?", "REGISTRAR VENTA CON SALDO", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                                implVenta = new VentaImpl();
                                try
                                {
                                    string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                    if (mensaje == "VENTA_EXITOSA")
                                    {
                                        MessageBox.Show("VENTA CON SALDO MAYOR A CERO REGISTRADA EXITOSAMENTE.");
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
                            venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                            implVenta = new VentaImpl();
                            try
                            {
                                string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                if (mensaje == "VENTA_EXITOSA")
                                {
                                    MessageBox.Show("VENTA REGISTRADA EXITOSAMENTE.");
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
                            venta = new Venta(cliente.IdCliente, Session.IdUsuario, Session.Sucursal_IdSucursal, venta_TotalUSD, venta_TotalBOB, venta_saldoUSD, venta_saldoBOB, txtObservacionVenta.Text);
                            implVenta = new VentaImpl();
                            try
                            {
                                string mensaje = implVenta.InsertTransaction(venta, listaProductos, listaDescuentosPorcentaje, listaGarantias, listaMetodoPagos, cliente);
                                if (mensaje == "VENTA_EXITOSA")
                                {
                                    MessageBox.Show("VENTA CON SALDO PENDIENTE REGISTRADA EXITOSAMENTE.");
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
            /*
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
                if (listaMetodoPagos.Count>0)
                {
                    string cadena = "";
                    foreach (var item in listaMetodoPagos)
                    {
                        cadena += "Pago USD: " + item.MontoUSD.ToString() + "\n" + "Pago BOB:" + item.MontoBOB.ToString() + "\n" + "Tipo: " + item.Tipo.ToString() + "\n";
                    }
                    MessageBox.Show(cadena);
                }
                if (listaProductos.Count>0)
                {
                    string cadena = "";
                    for (int i = 0; i < listaProductos.Count; i++)
                    {
                        cadena += listaProductos[i].IdProducto + " " + listaProductos[i].CodigoSublote + " Garantia: " + listaGarantias[i].Garantia + "\n";
                    }
                    MessageBox.Show(cadena);
                }
            }
            else
            {
                MessageBox.Show("¡No puede registrar la venta sin un cliente!");
            }
            */

            /*
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
                Binding = new Binding("totalproductoBOB")
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
        private void txtPrecioUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTotalUSD.Text) != true)
            {
                double costoBOB = Math.Round(double.Parse(txtTotalUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtTotalBOB.Text = costoBOB.ToString();
                descuentoPorcentaje = Math.Round(100 - double.Parse(txtTotalUSD.Text) * 100 / producto.PrecioVentaUSD, 2);
                txtDescuentoPorcentaje.Text = descuentoPorcentaje.ToString();
            }
        }
        private void txtPrecioBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTotalBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtTotalBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtTotalUSD.Text = costoUSD.ToString();
                descuentoPorcentaje = Math.Round(100 - double.Parse(txtTotalBOB.Text) * 100 / producto.PrecioVentaBOB, 2);
                txtDescuentoPorcentaje.Text = descuentoPorcentaje.ToString();
            }
        }
        private void txtDescuentoPorcentaje_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescuentoPorcentaje.Text) != true)
            {
                descuentoPorcentaje = double.Parse(txtDescuentoPorcentaje.Text);
                double costoUSD = Math.Round(producto.PrecioVentaUSD * (1 - descuentoPorcentaje / 100), 2);
                txtTotalUSD.Text = costoUSD.ToString();
                double costoBOB = Math.Round(producto.PrecioVentaBOB * (1 - descuentoPorcentaje / 100), 2);
                txtTotalBOB.Text = costoBOB.ToString();
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
        public class DataGridRowDetalle
        {
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
        private void dgvProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeFromDGVProducts(dgvProductos.SelectedIndex);
        }
        void removeFromDGVProducts(int posicion)
        {
            if (dgvProductos.SelectedItem != null && dgvProductos.Items.Count > 0)
            {
                if (dgvProductos.Items.IsEmpty != true && listaProductos != null)
                {
                    if (MessageBox.Show("Está realmente segur@ de remover este producto de la venta?", "Remover producto", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        venta_TotalUSD -= listaProductos[posicion].PrecioVentaUSD;
                        txtVentaTotalVentaUSD.Text = venta_TotalUSD.ToString();
                        venta_TotalBOB -= listaProductos[posicion].PrecioVentaBOB;
                        txtVentaTotalVentaBOB.Text = venta_TotalBOB.ToString();

                        venta_saldoUSD = Math.Round(venta_TotalUSD - venta_pagoTotalUSD,2);
                        txtVentaTotalSaldoUSD.Text = venta_saldoUSD.ToString();
                        venta_saldoBOB = Math.Round(venta_TotalBOB - venta_pagoTotalBOB,2);
                        txtVentaTotalSaldoBOB.Text = venta_saldoBOB.ToString();

                        dgvProductos.Items.RemoveAt(posicion);
                        listaProductos.RemoveAt(posicion);
                        listaGarantias.RemoveAt(posicion);
                        listaDescuentosPorcentaje.RemoveAt(posicion);
                    }
                }
            }
        }
    }
}
