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
using Label = DYMO.Label.Framework.Label;
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
        List<Double> listaPagosSublote = new List<Double>();
        int contador = 1;
        string codigoSublote;
        int idSublote = 0;
        double sublote_TotalPagosUSD = 0.00;
        double totalProductosSubloteUSD = 0.00;

        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        bool loteRegistrado = false;

        public class PagoDataGridView
        {
            public double montoUSD { get; set; }
        }

        public winProducto_Insert()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
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
            if(string.IsNullOrEmpty(acbtxtNombreProveedor.Text))
            {
                MessageBox.Show("¡Debe seleccionar un proveedor para registrar el lote de productos!");
                return;
            }
            if (listaHelper.Count > 0)
            {
                listaproductos.Clear();
                foreach (var item in listaHelper)
                {
                    listaproductos.Add(new Producto(item.IdSucursal, item.IdCategoria, item.IdSublote, item.IdCondicion, item.IdUsuario,
                        item.CodigoSublote, item.NombreProducto, item.Identificador, item.CostoUSD, item.CostoBOB, item.PrecioVentaUSD, item.PrecioVentaBOB, item.Observaciones));
                }

                if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \n Cantidad de productos ingresados al lote: " + listaproductos.Count + ". \n Presione SI para continuar.", "Confirmar lote", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    VerifyIfBatchAlreadyExists();
                }
            }
            else
            {
                MessageBox.Show("¡Debe ingresar como mínimo 1 producto al lote!");
            }
        }
        private void VerifyIfBatchAlreadyExists()
        {
            implProducto = new ProductoImpl();
            DataTable dt = implProducto.SelectBatchOfProductsToUpdate(listaproductos[0].CodigoSublote);
            byte i = 1;
            bool batchExists = false;
            while (dt.Rows.Count > 0)
            {
                batchExists = true;
                contador = 1;
                GetCodigoSubLoteFromDatabase(i);
                GetIDSubLoteFromDatabase();
                foreach (var item in listaproductos)
                {
                    item.CodigoSublote = codigoSublote + "-" + contador;
                    item.IdSublote = idSublote;
                    contador++;
                }
                i++;
                dt = implProducto.SelectBatchOfProductsToUpdate(listaproductos[0].CodigoSublote);
            }
            string mensaje = implProducto.InsertTransaction(listaproductos, (cbxLote.SelectedItem as ComboboxItem).Valor, acbtxtNombreProveedor.Text, listaPagosSublote);
            if (mensaje == "LOTE REGISTRADO EXITOSAMENTE.")
            {
                if (batchExists)
                {
                    loteRegistrado = true;
                    MessageBox.Show(mensaje + "\nSE HA DETECTADO UN SUB-LOTE DE PRODUCTOS YA EXISTENTE CON EL CÓDIGO DE SUB-LOTE, POR ENDE EL CÓDIGO DE SUBLOTE DE TODOS LOS PRODUCTOS INGRESADOS HA SIDO ACTUALIZADO PARA EVITAR CÓDIGOS DE SUB-LOTE DUPLICADOS.");
                    PrintCodigoSublote(listaproductos);
                    Close();
                }
                else
                {
                    loteRegistrado = true;
                    MessageBox.Show(mensaje);
                    PrintCodigoSublote(listaproductos);
                    Close();
                }
            }
            else
            {
                MessageBox.Show(mensaje);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString("0.00");
            cbxGetCategoriaFromDatabase();
            cbxGetLoteFromDatabase();
            cbxGetCondicionFromDatabase();
            cbxGetNombreProductoFromDatabase();
            cbxGetNombreProovedorFromDatabase();
            GetCodigoSubLoteFromDatabase(0);
            GetIDSubLoteFromDatabase();
            txtSucursal.Text = "Sucursal: " + Session.Sucursal_NombreSucursal;
            txtObservaciones.Text = "-";
            txtTotalSubloteUSD.Text = "0.00";
            txtTotalPagosUSD.Text = "0.00";
            txtSaldoUSD.Text = "0.00";
        }
        private void cbxLote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contador == 1)
            {
                GetCodigoSubLoteFromDatabase(0);
                GetIDSubLoteFromDatabase();
            }
        }
        private void txtCostoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(txtCostoUSD.Text, out double valorUSD))
            {
                double costoBOB = Math.Round(valorUSD * Session.Ajuste_Cambio_Dolar, 2);
                txtCostoBOB.Text = costoBOB.ToString();
            }
            else
            {
                txtCostoUSD.Text = "0";
                txtCostoBOB.Text = "0";
            }
        }

        private void txtCostoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(txtCostoBOB.Text, out double valorBOB))
            {
                double costoUSD = Math.Round(valorBOB / Session.Ajuste_Cambio_Dolar, 2);
                txtCostoUSD.Text = costoUSD.ToString();
            }
            else
            {
                txtCostoBOB.Text = "0";
                txtCostoUSD.Text = "0";
            }
        }

        private void txtPrecioUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(txtPrecioUSD.Text, out double valorUSD))
            {
                double precioBOB = Math.Round(valorUSD * Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioBOB.Text = precioBOB.ToString();
            }
            else
            {
                txtPrecioUSD.Text = "0";
                txtPrecioBOB.Text = "0";
            }
        }

        private void txtPrecioBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (double.TryParse(txtPrecioBOB.Text, out double valorBOB))
            {
                double precioUSD = Math.Round(valorBOB / Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioUSD.Text = precioUSD.ToString();
            }
            else
            {
                txtPrecioBOB.Text = "0";
                txtPrecioUSD.Text = "0";
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
                        NombreProducto = Regex.Replace(acbtxtNombreProducto.Text.Trim(), @"\s+", " "),
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

                    totalProductosSubloteUSD += double.Parse(txtCostoUSD.Text);
                    txtTotalSubloteUSD.Text = totalProductosSubloteUSD.ToString();
                    txtSaldoUSD.Text = (totalProductosSubloteUSD - sublote_TotalPagosUSD).ToString();

                    if (contador != 1)
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
                totalProductosSubloteUSD -= listaHelper[contador - 2].CostoUSD;
                txtTotalSubloteUSD.Text = totalProductosSubloteUSD.ToString();
                txtSaldoUSD.Text = (totalProductosSubloteUSD - sublote_TotalPagosUSD).ToString();

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

        void cbxGetNombreProovedorFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxNombreProveedor = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProducto = new ProductoImpl();
                dataTable = implProducto.SelectProviderNamesForComboBox();
                listcomboboxNombreProveedor = (from DataRow dr in dataTable.Rows
                                              select new ComboboxItem()
                                              {
                                                  Texto = dr["nombreProveedor"].ToString()
                                              }).ToList();
                acbtxtNombreProveedor.ItemsSource = listcomboboxNombreProveedor;
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
        void GetCodigoSubLoteFromDatabase(byte Suma)
        {
            try
            {
                implProducto = new ProductoImpl();
                codigoSublote = implProducto.GetCodeFormatToInsertProducts((cbxLote.SelectedItem as ComboboxItem).Valor, Suma);
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
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
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
            try
            {
                foreach (var item in ListaProductos)
                {
                    label.SetObjectText("lblCodigoSublote", item.CodigoSublote);
                    label.SetObjectText("lblCodigoQR", item.CodigoSublote);
                    label.Print(Session.ModeloDeEtiquetadoraDYMO);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message + "\nATENCIÓN: NO SE PUDO IMPRIMIR LAS ETIQUETAS PORQUE USTED NO CUENTA CON LA MAQUINA ETIQUETADORA " + Session.ModeloDeEtiquetadoraDYMO);
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
            int indexColumna = e.Column.DisplayIndex;
            DataGridRowDetalleHelper filaSeleccionada = e.Row.Item as DataGridRowDetalleHelper;
            TextBox valorNuevo = e.EditingElement as TextBox;

            if (valorNuevo == null || filaSeleccionada == null)
                return;

            try
            {
                string valorTexto = System.Text.RegularExpressions.Regex.Replace(valorNuevo.Text.Trim(), @"\s+", " ");
                bool esValido = true;

                switch (indexColumna)
                {
                    case 6: // NOMBRE PRODUCTO
                        esValido = ActualizarNombreProducto(valorTexto);
                        break;

                    case 7: // IDENTIFICADOR
                        esValido = ActualizarIdentificador(valorTexto);
                        break;

                    case 8: // COSTO USD
                        esValido = ActualizarCostoUSD(valorTexto, filaSeleccionada);
                        break;

                    case 9: // COSTO BOB
                        esValido = ActualizarCostoBOB(valorTexto, filaSeleccionada);
                        break;

                    case 10: // PRECIO USD
                        esValido = ActualizarPrecioUSD(valorTexto, filaSeleccionada);
                        break;

                    case 11: // PRECIO BOB
                        esValido = ActualizarPrecioBOB(valorTexto, filaSeleccionada);
                        break;

                    case 12: // OBSERVACIONES
                        esValido = ActualizarObservaciones(valorTexto);
                        break;
                }

                if (!esValido)
                {
                    e.Cancel = true; // Cancela la edición y restaura el valor anterior
                }
                else
                {
                    RefrescarDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\nDebido a la excepción presentada, se cerrará esta ventana para evitar errores.");
                loteRegistrado = true;
                this.Close();
            }
        }

        private bool ActualizarNombreProducto(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL NOMBRE DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
            {
                listaHelper[i].NombreProducto = valor;
            }
            return true;
        }

        private bool ActualizarIdentificador(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL IDENTIFICADOR DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            listaHelper[dgvProductos.SelectedIndex].Identificador = valor;
            return true;
        }

        private bool ActualizarCostoUSD(string valor, DataGridRowDetalleHelper filaSeleccionada)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL COSTO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            if (!double.TryParse(valor, out double costoUSD))
            {
                MessageBox.Show("EL COSTO EN USD DEBE SER UN VALOR NUMÉRICO VÁLIDO!");
                return false;
            }

            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
            {
                listaHelper[i].CostoUSD = costoUSD;
                listaHelper[i].CostoBOB = Math.Round(costoUSD * Session.Ajuste_Cambio_Dolar, 2);
            }

            if (costoUSD > filaSeleccionada.PrecioVentaUSD)
            {
                for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                {
                    listaHelper[i].PrecioVentaUSD = listaHelper[i].CostoUSD + 10;
                    listaHelper[i].PrecioVentaBOB = Math.Round(listaHelper[i].CostoBOB + (Session.Ajuste_Cambio_Dolar * 10), 2);
                }
            }
            return true;
        }

        private bool ActualizarCostoBOB(string valor, DataGridRowDetalleHelper filaSeleccionada)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL COSTO EN BOB DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            if (!double.TryParse(valor, out double costoBOB))
            {
                MessageBox.Show("EL COSTO EN BOB DEBE SER UN VALOR NUMÉRICO VÁLIDO!");
                return false;
            }

            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
            {
                listaHelper[i].CostoBOB = costoBOB;
                listaHelper[i].CostoUSD = Math.Round(costoBOB / Session.Ajuste_Cambio_Dolar, 2);
            }

            if (costoBOB > filaSeleccionada.PrecioVentaBOB)
            {
                for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                {
                    listaHelper[i].PrecioVentaBOB = listaHelper[i].CostoBOB + (Session.Ajuste_Cambio_Dolar * 10);
                    listaHelper[i].PrecioVentaUSD = Math.Round(listaHelper[i].CostoUSD + 10, 2);
                }
            }
            return true;
        }

        private bool ActualizarPrecioUSD(string valor, DataGridRowDetalleHelper filaSeleccionada)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL PRECIO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            if (!double.TryParse(valor, out double precioUSD))
            {
                MessageBox.Show("EL PRECIO EN USD DEBE SER UN VALOR NUMÉRICO VÁLIDO!");
                return false;
            }

            if (precioUSD <= filaSeleccionada.CostoUSD)
            {
                MessageBox.Show("EL PRECIO EN $. NO PUEDE SER MENOR O IGUAL AL COSTO EN $. DEL PRODUCTO!");
                return false;
            }

            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
            {
                listaHelper[i].PrecioVentaUSD = precioUSD;
                listaHelper[i].PrecioVentaBOB = Math.Round(precioUSD * Session.Ajuste_Cambio_Dolar, 2);
            }
            return true;
        }

        private bool ActualizarPrecioBOB(string valor, DataGridRowDetalleHelper filaSeleccionada)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("EL PRECIO EN BOB DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                return false;
            }

            if (!double.TryParse(valor, out double precioBOB))
            {
                MessageBox.Show("EL PRECIO EN BOB DEBE SER UN VALOR NUMÉRICO VÁLIDO!");
                return false;
            }

            if (precioBOB <= filaSeleccionada.CostoBOB)
            {
                MessageBox.Show("EL PRECIO EN Bs. NO PUEDE SER MENOR O IGUAL AL COSTO EN Bs. DEL PRODUCTO!");
                return false;
            }

            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
            {
                listaHelper[i].PrecioVentaBOB = precioBOB;
                listaHelper[i].PrecioVentaUSD = Math.Round(precioBOB / Session.Ajuste_Cambio_Dolar, 2);
            }
            return true;
        }

        private bool ActualizarObservaciones(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                MessageBox.Show("LA OBSERVACION DEL PRODUCTO NO PUEDE ESTAR VACÍA!");
                return false;
            }

            listaHelper[dgvProductos.SelectedIndex].Observaciones = valor;
            return true;
        }

        private void RefrescarDataGrid()
        {
            dgvProductos.ItemsSource = null;
            dgvProductos.ItemsSource = listaHelper;
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
        private void btnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            addPaymentToDataGridandList();
        }

        private void txtPagoUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                addPaymentToDataGridandList();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }

        void addPaymentToDataGridandList()
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                if (double.Parse(txtPagoUSD.Text) != 0)
                {
                    //Añadiendo métodos de pago a la tabla y a la lista.
                    dgvPagosSublote.Items.Add(new PagoDataGridView
                    {
                        montoUSD = double.Parse(txtPagoUSD.Text)
                    });
                    listaPagosSublote.Add(double.Parse(txtPagoUSD.Text));

                    //Actualizando las cifras de la venta.
                    sublote_TotalPagosUSD += double.Parse(txtPagoUSD.Text);
                    txtTotalPagosUSD.Text = sublote_TotalPagosUSD.ToString();
                    txtSaldoUSD.Text = (totalProductosSubloteUSD - sublote_TotalPagosUSD).ToString();
                    //Vaciando los txt del método de pago de dólar.
                    txtPagoUSD.Text = "";
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

        private void btndgvRemoverPagoSublote(object sender, RoutedEventArgs e)
        {
            removeFromDGVPayment(dgvPagosSublote.SelectedIndex);
        }

        void removeFromDGVPayment(int posicion)
        {
            if (dgvPagosSublote.SelectedItem != null && dgvPagosSublote.Items.Count > 0)
            {
                if (dgvPagosSublote.Items.IsEmpty != true && listaPagosSublote != null)
                {
                    if (MessageBox.Show("Está realmente segur@ de remover este page de sublote?", "Remover pago de sublote", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        sublote_TotalPagosUSD -= listaPagosSublote[posicion];
                        txtTotalPagosUSD.Text = sublote_TotalPagosUSD.ToString();
                        txtSaldoUSD.Text = (totalProductosSubloteUSD - sublote_TotalPagosUSD).ToString();
                        dgvPagosSublote.Items.RemoveAt(posicion);
                        listaPagosSublote.RemoveAt(posicion);
                    }
                }
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
