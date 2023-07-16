using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;//ADO.NET
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProductoComun_Venta_Insert.xaml
    /// </summary>
    public partial class winProductoComun_Venta_Insert : Window
    {
        double venta_TotalBOB = 0;
        ProductoComunImpl implProductoComun;
        ProductoComun productoComun;
        List<ProductoComun> listaProductosComunes = new List<ProductoComun>();
        List<string> listaDetalles = new List<string>();
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        bool ventaRegistrada = false;
        public winProductoComun_Venta_Insert()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetProductoComunFromDatabase();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                implProductoComun = new ProductoComunImpl();
                productoComun = implProductoComun.Get((cbxProductoComun.SelectedItem as ComboboxItem).Valor);
                if (productoComun != null)
                {
                    bool validoParaInsercion = true;
                    for (int i = 0; i < listaHelper.Count; i++)
                    {
                        if (productoComun.IdProductoComun == listaHelper[i].idProductoComun)
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
                            idProductoComun = productoComun.IdProductoComun,
                            nombreProductoComun = productoComun.NombreProductoComun,
                            precioMinimoBOB = productoComun.PrecioMinimo,
                            precioSugeridoBOB = productoComun.PrecioSugerido,
                            precioVentaBOB = productoComun.PrecioSugerido,
                            detalle = "-"
                        });
                        venta_TotalBOB += productoComun.PrecioSugerido;
                        txtTotalBOB.Text = "TOTAL: " + venta_TotalBOB + " Bs.";
                    }
                }
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
                    venta_TotalBOB -= listaHelper[dgvProductos.SelectedIndex].precioVentaBOB;
                    txtTotalBOB.Text = "TOTAL: " + venta_TotalBOB + " Bs.";

                    listaHelper.RemoveAt(dgvProductos.SelectedIndex);
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
                    double limite = Math.Round(filaSeleccionada.precioMinimoBOB / 100 * (100 - Session.Ajuste_Limite_Descuento), 2);
                    if (double.Parse(valorNuevo.Text.ToString()) >= limite)
                    {
                        venta_TotalBOB -= listaHelper[dgvProductos.SelectedIndex].precioVentaBOB;
                        try
                        {
                            listaHelper[dgvProductos.SelectedIndex].precioVentaBOB = double.Parse(valorNuevo.Text.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            throw;
                        }
                        venta_TotalBOB += listaHelper[dgvProductos.SelectedIndex].precioVentaBOB;
                        txtTotalBOB.Text = "TOTAL: " + venta_TotalBOB + " Bs.";
                    }
                    else
                    {
                        MessageBox.Show("¡EL PRECIO DE VENTA NO PUEDE SER INFERIOR AL PRECIO SUGERIDO!\nPRODUCTO SELECCIONADO: " + filaSeleccionada.nombreProductoComun
                            + "\nPRECIO MINIMO: " + filaSeleccionada.precioMinimoBOB + "\n LIMITE DE DESCUENTO POR DEBAJO DEL PRECIO MINIMO: " + limite);
                    }
                }
                else if (indexSeleccionado == 5)
                {
                    listaHelper[dgvProductos.SelectedIndex].detalle = valorNuevo.Text.ToString();
                }
                dgvProductos.ItemsSource = null;
                dgvProductos.ItemsSource = listaHelper;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            listaProductosComunes.Clear();
            listaDetalles.Clear();
            foreach (var item in listaHelper)
            {
                listaProductosComunes.Add(new ProductoComun(item.idProductoComun, item.nombreProductoComun, item.precioVentaBOB));
                listaDetalles.Add(item.detalle);
            }

            implProductoComun = new ProductoComunImpl();
            try
            {
                string mensaje = implProductoComun.InsertTransaction(listaProductosComunes, listaDetalles, venta_TotalBOB);
                if (mensaje == "VENTA_EXITOSA")
                {
                    MessageBox.Show("VENTA REGISTRADA EXITOSAMENTE.");
                    ventaRegistrada = true;
                    this.Close();
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
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            dgvProductos.ItemsSource = listaHelper;
        }
        private void cbxGetProductoComunFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxProductoComun = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProductoComun = new ProductoComunImpl();
                dataTable = implProductoComun.SelectForComboBox();
                listcomboboxProductoComun = (from DataRow dr in dataTable.Rows
                                             select new ComboboxItem()
                                             {
                                                 Valor = Convert.ToByte(dr["idProductoComun"]),
                                                 Texto = dr["nombreProductoComun"].ToString()
                                             }).ToList();
                foreach (var item in listcomboboxProductoComun)
                {
                    cbxProductoComun.Items.Add(item);
                }
                cbxProductoComun.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class DataGridRowDetalleHelper
        {
            public int idProductoComun { get; set; }
            public string nombreProductoComun { get; set; }
            public double precioMinimoBOB { get; set; }
            public double precioSugeridoBOB { get; set; }
            public double precioVentaBOB { get; set; }
            public string detalle { get; set; }
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
    }
}
