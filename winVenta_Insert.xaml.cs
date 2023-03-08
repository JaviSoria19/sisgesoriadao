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
        double precioTotal = 0;
        List<Producto> listaProducto = new List<Producto>();

        VentaImpl implVenta;
        Venta venta;

        public winVenta_Insert()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtSearchCustomer.Focus();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (Session.Rol == 1)
            {
                winMainAdmin winMainAdmin = new winMainAdmin();
                winMainAdmin.Show();
                this.Close();
            }
            else
            {
                winMainSeller winMainSeller = new winMainSeller();
                winMainSeller.Show();
                this.Close();
            }            
        }

        private void btnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchCustomer.Text)==false)
            {
                try
                {
                    implCliente = new ClienteImpl();
                    cliente = implCliente.GetByCIorCelular(txtSearchCustomer.Text);
                    if (cliente != null)
                    {
                        lblCustomerNombre.Content = "NOMBRE: " + cliente.Nombres + " " + cliente.PrimerApellido + " " + cliente.SegundoApellido;
                        lblCustomerNumeroCI.Content = "C.I.: " + cliente.NumeroCI;
                        lblCustomerNumeroCelular.Content = "CELULAR: " + cliente.NumeroCelular;
                        txtSearchCustomer.IsEnabled = false;
                        btnSearchCustomer.IsEnabled = false;

                        txtSearchProduct.IsEnabled = true;
                        btnSearchProduct.IsEnabled = true;
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
        private void btnSaveNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text) == false && string.IsNullOrEmpty(txtPrimerApellido.Text) == false && string.IsNullOrEmpty(txtNumeroCelular.Text) == false && string.IsNullOrEmpty(txtNumeroCI.Text) == false)
            {
                cliente = new Cliente(txtNombre.Text.Trim(), txtPrimerApellido.Text.Trim(), txtSegundoApellido.Text.Trim(), txtNumeroCelular.Text.Trim(), txtNumeroCI.Text.Trim());
                implCliente = new ClienteImpl();
                try
                {
                    int n = implCliente.Insert(cliente);
                    if (n > 0)
                    {
                        stackpanelCustomerFound.Visibility = Visibility.Visible;
                        stackpanelCustomerForm.Visibility = Visibility.Collapsed;
                        stackpanelCustomerButtons.Visibility = Visibility.Collapsed;
                        txtSearchCustomer.Text = cliente.NumeroCI;
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

            txtNombre.IsEnabled = true;
            txtPrimerApellido.IsEnabled = true;
            txtSegundoApellido.IsEnabled = true;
            txtNumeroCelular.IsEnabled = true;
            txtNumeroCI.IsEnabled = true;
            btnSaveNewCustomer.IsEnabled = true;
            btnCancelNewCustomer.IsEnabled = true;
        }
        void DisableCustomerButtons()
        {
            txtSearchCustomer.IsEnabled = true;
            btnSearchCustomer.IsEnabled = true;

            txtNombre.IsEnabled = false;
            txtPrimerApellido.IsEnabled = false;
            txtSegundoApellido.IsEnabled = false;
            txtNumeroCelular.IsEnabled = false;
            txtNumeroCI.IsEnabled = false;
            btnSaveNewCustomer.IsEnabled = false;
            btnCancelNewCustomer.IsEnabled = false;
        }

        private void btnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchProduct.Text) ==false)
            {
                try
                {
                    implProducto = new ProductoImpl();
                    producto = implProducto.GetBySNorIMEI(txtSearchProduct.Text);
                    if (producto != null)
                    {
                        //RELLENA LA INFORMACIÓN DEL PRODUCTO EN UN LABEL.
                        lblProductFound.Content = "DESCRIPCION: " + producto.NombreProducto + " " + producto.Color + " | " + producto.NumeroSerie;
                        //ASIGNA EL PRECIO DEL PRODUCTO EN UN TEXTBOX PARA MODIFICARLO.
                        txtPrecio.Text = producto.Precio.ToString();

                        //INFORMA QUE EL PRODUCTO HA SIDO HALLADO.
                        labelSuccess(lblSearchProductInfo);
                        lblSearchProductInfo.Content = "PRODUCTO ENCONTRADO: " + producto.NombreProducto;

                        //HABILITA EL CAMPO DEL PRECIO Y EL BOTÓN PARA AÑADIR A LA LISTA.
                        txtPrecio.IsEnabled = true;
                        btnAddProduct.IsEnabled = true;
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
            if (producto != null)
            {
                producto.Precio = double.Parse(txtPrecio.Text);
                //ASIGNA EL VALOR TOTAL DE TODOS LOS PRODUCTOS
                precioTotal = precioTotal + producto.Precio;
                txtPrecioTotal.Text = "Bs. " + precioTotal;

                listaProducto.Add(producto);
                dgvProductos.Items.Add(new DataGridRowDetalle { cantidad = 1, producto = producto.NombreProducto + " " + producto.Color, numeroSerie = producto.NumeroSerie, precio = producto.Precio });
                if (btnSaveAndPDF.IsEnabled == false)
                {
                    btnSaveAndPDF.IsEnabled = true;
                }
                producto = null;
            }
            else
            {
                labelWarning(lblSearchProductInfo);
                lblSearchProductInfo.Content = "NO PUEDE AÑADIR EL MISMO PRODUCTO 2 VECES.";
            }
        }

        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn columna1 = new DataGridTextColumn();
            columna1.Header = "Cantidad";
            columna1.Binding = new Binding("cantidad");
            DataGridTextColumn columna2 = new DataGridTextColumn();
            columna2.Header = "Producto";
            columna2.Binding = new Binding("producto");
            DataGridTextColumn columna3 = new DataGridTextColumn();
            columna3.Header = "SN/IMEI";
            columna3.Binding = new Binding("numeroSerie");
            DataGridTextColumn columna4 = new DataGridTextColumn();
            columna4.Header = "Precio";
            columna4.Binding = new Binding("precio");
            dgvProductos.Columns.Add(columna1);
            dgvProductos.Columns.Add(columna2);
            dgvProductos.Columns.Add(columna3);
            dgvProductos.Columns.Add(columna4);
        }
        public class DataGridRowDetalle
        {
            public byte cantidad { get; set; }
            public string producto { get; set; }
            public string numeroSerie { get; set; }
            public double precio { get; set; }
        }

        private void btnSaveAndPDF_Click(object sender, RoutedEventArgs e)
        {
            venta = new Venta(cliente.IdCliente,Session.IdUsuario,precioTotal,1,"BS.",1);
            implVenta = new VentaImpl();
            string mensaje = implVenta.InsertTransaction(listaProducto, venta);
            MessageBox.Show(mensaje);
        }
    }
}
