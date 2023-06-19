using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using DYMO.Label.Framework;
using Label = DYMO.Label.Framework.Label;
using System.Collections.ObjectModel;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Insert.xaml
    /// </summary>
    public partial class winProducto_Insert : Window
    {
        CategoriaImpl implCategoria;
        ProductoImpl implProducto;
        CondicionImpl implCondicion;
        List<Producto> listaproductos = new List<Producto>();
        int contador=1;
        string codigoSublote;
        int idSublote = 0;

        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        bool loteRegistrado = false;
        public winProducto_Insert()
        {
            InitializeComponent();
        }        
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addToDataGrid();
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            removeFromDataGrid();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (listaHelper.Count > 0)
            {
                listaproductos.Clear();
                foreach (var item in listaHelper)
                {
                    listaproductos.Add(new Producto(item.IdSucursal,item.IdCategoria,item.IdSublote,item.IdCondicion,item.IdUsuario,
                        item.CodigoSublote,item.NombreProducto,item.Identificador,item.CostoUSD,item.CostoBOB,item.PrecioVentaUSD,item.PrecioVentaBOB,item.Observaciones));
                }

                if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \n Cantidad de productos ingresados al lote: " + listaproductos.Count + ". \n Presione SI para continuar.", "Confirmar lote", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    implProducto = new ProductoImpl();
                    string mensaje = implProducto.InsertTransaction(listaproductos, (cbxLote.SelectedItem as ComboboxItem).Valor);
                    if (mensaje == "LOTE REGISTRADO EXITOSAMENTE.")
                    {
                        loteRegistrado = true;
                        PrintCodigoSublote(listaproductos);
                        MessageBox.Show(mensaje + "\n IMPRIMIENDO LAS ETIQUETAS...");
                        //insertar código para imprimir etiquetas DYMO
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            cbxGetCategoriaFromDatabase();
            cbxGetLoteFromDatabase();
            cbxGetCondicionFromDatabase();
            cbxGetNombreProductoFromDatabase();
            GetCodigoSubLoteFromDatabase();
            GetIDSubLoteFromDatabase();
            txtSucursal.Text = "Sucursal: " + Session.Sucursal_NombreSucursal;
            txtObservaciones.Text = "-";
        }
        private void cbxLote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contador==1)
            {
                GetCodigoSubLoteFromDatabase();
                GetIDSubLoteFromDatabase();
            }
        }
        private void txtCostoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCostoUSD.Text)!=true)
            {
                double costoBOB = Math.Round(double.Parse(txtCostoUSD.Text) * Session.Ajuste_Cambio_Dolar,2);
                txtCostoBOB.Text = costoBOB.ToString();
            }
        }
        private void txtCostoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCostoBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtCostoBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);                
                txtCostoUSD.Text = costoUSD.ToString();
            }
        }
        private void txtPrecioUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrecioUSD.Text) != true)
            {
                double precioBOB = Math.Round(double.Parse(txtPrecioUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioBOB.Text = precioBOB.ToString();
            }
        }
        private void txtPrecioBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrecioBOB.Text) != true)
            {
                double precioUSD = Math.Round(double.Parse(txtPrecioBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioUSD.Text = precioUSD.ToString();
            }
        }
        private void txtIdentificador_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addToDataGrid();
            }
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        void addToDataGrid()
        {
            if (string.IsNullOrEmpty(acbtxtNombreProducto.Text) != true && string.IsNullOrEmpty(txtIdentificador.Text) != true &&
                string.IsNullOrEmpty(txtCostoUSD.Text) != true && string.IsNullOrEmpty(txtCostoBOB.Text) != true &&
                string.IsNullOrEmpty(txtPrecioUSD.Text) != true && string.IsNullOrEmpty(txtPrecioBOB.Text) != true)
            {
                if (double.Parse(txtPrecioUSD.Text) > double.Parse(txtCostoUSD.Text))
                {
                    listaHelper.Add(new DataGridRowDetalleHelper
                    {
                        IdSucursal = Session.Sucursal_IdSucursal,
                        IdCategoria = byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString()),
                        IdSublote = idSublote,
                        IdCondicion = byte.Parse((cbxCondicion.SelectedItem as ComboboxItem).Valor.ToString()),
                        IdUsuario = Session.IdUsuario,
                        CodigoSublote = txtCodigoSublote.Text,
                        NombreProducto = acbtxtNombreProducto.Text,
                        Identificador = txtIdentificador.Text,
                        CostoUSD = double.Parse(txtCostoUSD.Text),
                        CostoBOB = double.Parse(txtCostoBOB.Text),
                        PrecioVentaUSD = double.Parse(txtPrecioUSD.Text),
                        PrecioVentaBOB = double.Parse(txtPrecioBOB.Text),
                        Observaciones = txtObservaciones.Text
                    });

                    contador++;
                    txtCodigoSublote.Text = codigoSublote + "-" + contador;
                    txtIdentificador.Text = "";
                    txtIdentificador.Focus();

                    if (contador!=1)
                    {
                        cbxLote.IsEnabled = false;
                        cbxCategoria.IsEnabled = false;
                        cbxCondicion.IsEnabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("¡El precio de venta no puede ser menor al costo del producto!");
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
            }
        }    
        void removeFromDataGrid()
        {
            if (dgvProductos.Items.IsEmpty != true)
            {
                listaHelper.RemoveAt(contador - 2);
                contador--;
                txtCodigoSublote.Text = codigoSublote + "-" + contador;
            }
            if (contador == 1)
            {
                cbxLote.IsEnabled = true;
                cbxCategoria.IsEnabled = true;
                cbxCondicion.IsEnabled = true;
            }
        }
        void cbxGetCondicionFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCondicion = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCondicion = new CondicionImpl();
                dataTable = implCondicion.SelectForComboBox();
                listcomboboxCondicion = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = Convert.ToByte(dr["idCondicion"]),
                                             Texto = dr["nombreCondicion"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxCondicion)
                {
                    cbxCondicion.Items.Add(item);
                }
                cbxCondicion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetCategoriaFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCategoria = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCategoria = new CategoriaImpl();
                dataTable = implCategoria.SelectForComboBox();
                listcomboboxCategoria = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = Convert.ToByte(dr["idCategoria"]),
                                             Texto = dr["nombreCategoria"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxCategoria)
                {
                    cbxCategoria.Items.Add(item);
                }
                cbxCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetNombreProductoFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxNombreProducto = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProducto = new ProductoImpl();
                dataTable = implProducto.SelectProductNamesForComboBox();
                listcomboboxNombreProducto = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Texto = dr["nombreProducto"].ToString()
                                         }).ToList();
                acbtxtNombreProducto.ItemsSource = listcomboboxNombreProducto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetLoteFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxLote = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProducto = new ProductoImpl();
                dataTable = implProducto.SelectBatchForComboBox();
                listcomboboxLote = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = Convert.ToInt32(dr["idLote"]),
                                             Texto = dr["codigoLote"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxLote)
                {
                    cbxLote.Items.Add(item);
                }
                cbxLote.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GetCodigoSubLoteFromDatabase()
        {
            try
            {
                implProducto = new ProductoImpl();
                codigoSublote = implProducto.GetCodeFormatToInsertProducts((cbxLote.SelectedItem as ComboboxItem).Valor);
                txtCodigoSublote.Text = codigoSublote + "-" + contador;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GetIDSubLoteFromDatabase()
        {
            try
            {
                implProducto = new ProductoImpl();
                idSublote = implProducto.GetSubBatchToInsertProducts((cbxLote.SelectedItem as ComboboxItem).Valor);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9,-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
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
        public void PrintCodigoSublote(List<Producto> ListaProductos)
        {
            var label = Label.Open("LabelWriterCodigoQRProducto.label");
            foreach (var item in ListaProductos)
            {
                label.SetObjectText("lblCodigoSublote", item.CodigoSublote);
                label.SetObjectText("lblCodigoQR", item.CodigoSublote);
                label.Print("DYMO LabelWriter 450");
            }
        }
        public class DataGridRowDetalleHelper
        {
            public byte IdSucursal { get; set; }
            public byte IdCategoria { get; set; }
            public int IdSublote { get; set; }
            public byte IdCondicion { get; set; }
            public byte IdUsuario { get; set; }
            public string CodigoSublote { get; set; }
            public string NombreProducto { get; set; }
            public string Identificador { get; set; }
            public double CostoUSD { get; set; }
            public double CostoBOB { get; set; }
            public double PrecioVentaUSD { get; set; }
            public double PrecioVentaBOB { get; set; }
            public string Observaciones { get; set; }
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
                if (indexSeleccionado == 6)/*NOMBRE PRODUCTO*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                        {
                            listaHelper[i].NombreProducto = valorNuevo.Text.Trim().ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL NOMBRE DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 7)/*IDENTIFICADOR*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        listaHelper[dgvProductos.SelectedIndex].Identificador = valorNuevo.Text.Trim().ToString();
                    }
                    else
                    {
                        MessageBox.Show("EL IDENTIFICADOR DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                    
                }
                else if (indexSeleccionado == 8)/*COSTO USD*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        foreach (var item in listaHelper)
                        {
                            item.CostoUSD = double.Parse(valorNuevo.Text.Trim());
                            item.CostoBOB = Math.Round(item.CostoUSD * Session.Ajuste_Cambio_Dolar,2);
                        }
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.PrecioVentaUSD)
                        {
                            foreach (var item in listaHelper)
                            {
                                item.PrecioVentaUSD = item.CostoUSD + 10;
                                item.PrecioVentaBOB = Math.Round(item.CostoBOB + (Session.Ajuste_Cambio_Dolar * 10), 2);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL COSTO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 9)/*COSTO BOB*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        foreach (var item in listaHelper)
                        {
                            item.CostoBOB = double.Parse(valorNuevo.Text.Trim());
                            item.CostoUSD = Math.Round(item.CostoBOB / Session.Ajuste_Cambio_Dolar, 2);
                        }
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.PrecioVentaBOB)
                        {
                            foreach (var item in listaHelper)
                            {
                                item.PrecioVentaBOB = item.CostoBOB + (Session.Ajuste_Cambio_Dolar * 10);
                                item.PrecioVentaUSD = Math.Round(item.CostoUSD + 10, 2);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL COSTO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 10)/*PRECIO USD*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.CostoUSD)
                        {
                            foreach (var item in listaHelper)
                            {
                                item.PrecioVentaUSD = double.Parse(valorNuevo.Text.Trim());
                                item.PrecioVentaBOB = Math.Round(item.PrecioVentaUSD * Session.Ajuste_Cambio_Dolar, 2);
                            }
                        }
                        else
                        {
                            MessageBox.Show("EL PRECIO EN $. NO PUEDE SER MENOR O IGUAL AL COSTO EN $. DEL PRODUCTO!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL PRECIO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 11)/*PRECIO BOB*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.CostoBOB)
                        {
                            foreach (var item in listaHelper)
                            {
                                item.PrecioVentaBOB = double.Parse(valorNuevo.Text.Trim());
                                item.PrecioVentaUSD = Math.Round(item.PrecioVentaBOB / Session.Ajuste_Cambio_Dolar, 2);
                            }
                        }
                        else
                        {
                            MessageBox.Show("EL PRECIO EN Bs. NO PUEDE SER MENOR O IGUAL AL COSTO EN Bs. DEL PRODUCTO!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL PRECIO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 12)/*OBSERVACIONES*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        listaHelper[dgvProductos.SelectedIndex].Observaciones = valorNuevo.Text.Trim().ToString();
                    }
                    else
                    {
                        MessageBox.Show("LA OBSERVACION DEL PRODUCTO NO PUEDE ESTAR VACÍA!");
                    }
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listaHelper.Count > 0 && loteRegistrado == false)
            {
                MessageBoxResult result =
                  MessageBox.Show(
                    "ATENCIÓN: Se ha agregado uno o más productos al lote para registrar en el sistema, ¿Está seguro de cerrar la ventana sin haber registrado el lote?",
                    "Lote pendiente",
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
