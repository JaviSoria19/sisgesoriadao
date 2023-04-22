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
    /// Lógica de interacción para winCotizacion_Insert.xaml
    /// </summary>
    public partial class winCotizacion_Insert : Window
    {
        ProductoImpl implProducto;
        Producto producto;
        List<Producto> listaProductos = new List<Producto>();
        CotizacionImpl implCotizacion;
        public winCotizacion_Insert()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            acbxGetProductosFromDatabase();
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
            if (listaProductos.Count > 0)
            {
                if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \n Cantidad de productos ingresados en la cotización: " + listaProductos.Count + ". \n Presione SI para continuar.", "Confirmar cotización", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    implCotizacion = new CotizacionImpl();
                    string mensaje = implCotizacion.InsertTransaction(listaProductos);
                    if (mensaje == "COTIZACION REGISTRADA EXITOSAMENTE.")
                    {
                        MessageBox.Show(mensaje);
                        //insertar código para imprimir PDF
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
                MessageBox.Show("¡Debe ingresar como mínimo 1 producto al lote!");
            }
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9,-]+"); //regex that matches disallowed text
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
<<<<<<< HEAD
                            txtPrecioUSD.Text = producto.PrecioVentaUSD.ToString();
                            txtPrecioBOB.Text = producto.PrecioVentaBOB.ToString();

=======
>>>>>>> 186ced3ae7a536bb98b1c5c744b781d7fd732b66
                            txtCotizacionUSD.Text = producto.PrecioVentaUSD.ToString();
                            txtCotizacionBOB.Text = producto.PrecioVentaBOB.ToString();
                            txtCotizacionUSD.IsEnabled = true;
                            txtCotizacionBOB.IsEnabled = true;
                            btnAdd.IsEnabled = true;
                            tglOcultarUSD.IsEnabled = true;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
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
        }
        private void txtCotizacionUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addToDataGrid_andList(producto);
            }
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
