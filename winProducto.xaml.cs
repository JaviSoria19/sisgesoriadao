using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para winProducto.xaml
    /// </summary>
    public partial class winProducto : Window
    {
        ProductoImpl implProducto;
        Producto producto;
        byte operacion;
        double productosCostoTotalUSD = 0, productosCostoTotalBOB = 0, productosVentaTotalUSD = 0, productosVentaTotalBOB = 0;
        //Implementaciones para obtener ID's y Nombres para los Combobox
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        CondicionImpl implCondicion;
        public winProducto()
        {
            InitializeComponent();
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Insert winProducto_Insert = new winProducto_Insert();
            winProducto_Insert.Show();
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (producto != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UN PRODUCTO DEBE SELECCIONAR UN REGISTRO!";
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (producto != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        implProducto = new ProductoImpl();
                        int n = implProducto.Delete(producto);
                        if (n > 0)
                        {
                            labelSuccess(lblInfo);
                            lblInfo.Content = "REGISTRO ELIMINADO CON ÉXITO.";
                            Select();
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA ELIMINAR UN PRODUCTO DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (operacion)
            {
                //UPDATE
                case 2:
                    if (MessageBox.Show("Está realmente segur@ de modificar el registro seleccionado?", "Confirmar Modificación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        producto.IdCategoria = byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString());
                        producto.IdCondicion = byte.Parse((cbxCondicion.SelectedItem as ComboboxItem).Valor.ToString());
                        producto.IdUsuario = Session.IdUsuario;
                        producto.NombreProducto = txtNombreProducto.Text.Trim();
                        producto.Identificador = txtIdentificador.Text.Trim();
                        producto.CostoUSD = double.Parse(txtCostoUSD.Text.Trim());
                        producto.CostoBOB = double.Parse(txtCostoBOB.Text.Trim());
                        producto.PrecioVentaUSD = double.Parse(txtPrecioUSD.Text.Trim());
                        producto.PrecioVentaBOB = double.Parse(txtPrecioBOB.Text.Trim());
                        producto.Observaciones = txtObservaciones.Text.Trim();
                        implProducto = new ProductoImpl();
                        try
                        {
                            int n = implProducto.Update(producto);
                            if (n > 0)
                            {
                                labelSuccess(lblInfo);
                                lblInfo.Content = "REGISTRO MODIFICADO CON ÉXITO.";
                                Select();
                                DisabledButtons();
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
            //La siguiente línea de código comentada fue una prueba para obtener y aprender como funciona los valores del combobox.
            //MessageBox.Show("VALOR INTERNO: " + (cbxMarca.SelectedItem as ComboboxItem).Valor.ToString() + " ¿SELECTED ITEM?: " + cbxMarca.SelectedItem + " ¿SELECTED INDEX?: " + cbxMarca.SelectedIndex + "REGISTROS : " + cbxMarca.Items.Count);
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DisabledButtons();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetCategoriaFromDatabase();
            cbxGetCondicionFromDatabase();
            cbxGetGroupConcatSucursal();
            cbxGetSucursalFromDatabase();
        }
        void cbxGetGroupConcatSucursal()
        {
            try
            {
                implSucursal = new SucursalImpl();
                string sucursalGroupConcatID = implSucursal.SelectGroupConcatIDForComboBox();
                if (sucursalGroupConcatID != null)
                {
                    cbxSucursal.Items.Add(new ComboboxItem("TODOS", sucursalGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    int id = int.Parse(d.Row.ItemArray[0].ToString());
                    implProducto = new ProductoImpl();
                    producto = implProducto.Get(id);
                    if (producto != null)
                    {
                        txtCodigoSublote.Text = producto.CodigoSublote.Trim();
                        txtNombreProducto.Text = producto.NombreProducto.Trim();
                        txtIdentificador.Text = producto.Identificador.Trim();
                        txtCostoUSD.Text = producto.CostoUSD.ToString();
                        txtCostoBOB.Text = producto.CostoBOB.ToString();
                        txtPrecioUSD.Text = producto.PrecioVentaUSD.ToString();
                        txtPrecioBOB.Text = producto.PrecioVentaBOB.ToString();
                        txtObservaciones.Text = producto.Observaciones.ToString();
                        for (int i = 0; i < cbxCategoria.Items.Count; i++)
                        {
                            cbxCategoria.SelectedIndex = i;
                            if ((cbxCategoria.SelectedItem as ComboboxItem).Valor == producto.IdCategoria.ToString())
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < cbxCondicion.Items.Count; i++)
                        {
                            cbxCondicion.SelectedIndex = i;
                            if ((cbxCondicion.SelectedItem as ComboboxItem).Valor == producto.IdCondicion.ToString())
                            {
                                break;
                            }
                        }
                        labelSuccess(lblInfo);
                        lblInfo.Content = "PRODUCTO SELECCIONADO.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLike();
            }
            if (e.Key == Key.Escape)
            {
                CleanText();
            }
        }
        private void txtCostoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCostoUSD.Text) != true)
            {
                double costoBOB = Math.Round(double.Parse(txtCostoUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
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
        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }
        private void Select()
        {
            try
            {
                productosCostoTotalUSD = 0;
                productosCostoTotalBOB = 0;
                productosVentaTotalUSD = 0;
                productosVentaTotalBOB = 0;
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.Select().Rows.Count;
                if (dgvDatos.Items.Count > 0)
                {
                    foreach (DataRowView item in dgvDatos.Items)
                    {
                        productosCostoTotalUSD += double.Parse(item[7].ToString());
                        productosCostoTotalBOB += double.Parse(item[8].ToString());
                        productosVentaTotalUSD += double.Parse(item[9].ToString());
                        productosVentaTotalBOB += double.Parse(item[10].ToString());
                    }
                }
                productosCostoTotalUSD = Math.Round(productosCostoTotalUSD, 2);
                productosCostoTotalBOB = Math.Round(productosCostoTotalBOB, 2);
                productosVentaTotalUSD = Math.Round(productosVentaTotalUSD, 2);
                productosVentaTotalBOB = Math.Round(productosVentaTotalBOB, 2);
                txtTotalProductosCostoUSD.Text = "Total Costo $us.: " + productosCostoTotalUSD.ToString();
                txtTotalProductosCostoBOB.Text = "Total Costo Bs.: " + productosCostoTotalBOB.ToString();
                txtTotalProductosVentaUSD.Text = "Total Venta $us.: " + productosVentaTotalUSD.ToString();
                txtTotalProductosVentaBOB.Text = "Total Venta Bs.: " + productosVentaTotalBOB.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectLike()
        {
            try
            {
                productosCostoTotalUSD = 0;
                productosCostoTotalBOB = 0;
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectLikeReporteValorado((cbxSucursal.SelectedItem as ComboboxItem).Valor, txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeReporteValorado((cbxSucursal.SelectedItem as ComboboxItem).Valor, txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                if (dgvDatos.Items.Count > 0)
                {
                    foreach (DataRowView item in dgvDatos.Items)
                    {
                        productosCostoTotalUSD += double.Parse(item[7].ToString());
                        productosCostoTotalBOB += double.Parse(item[8].ToString());
                    }
                }
                txtTotalProductosCostoUSD.Text = "Total Costo $us.: " + productosCostoTotalUSD.ToString();
                txtTotalProductosCostoBOB.Text = "Total Costo Bs.: " + productosCostoTotalBOB.ToString();
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
                                             Valor = dr["idCategoria"].ToString(),
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
                                             Valor = dr["idCondicion"].ToString(),
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
        void cbxGetSucursalFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxSucursal = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implSucursal = new SucursalImpl();
                dataTable = implSucursal.SelectForComboBox();
                listcomboboxSucursal = (from DataRow dr in dataTable.Rows
                                        select new ComboboxItem()
                                        {
                                            Valor = dr["idSucursal"].ToString(),
                                            Texto = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursal.Items.Add(item);
                }
                cbxSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void EnabledButtons()
        {
            btnInsert.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            txtNombreProducto.IsEnabled = true;
            txtNombreProducto.Focus();
            txtIdentificador.IsEnabled = true;
            txtCostoUSD.IsEnabled = true;
            txtCostoBOB.IsEnabled = true;
            txtPrecioUSD.IsEnabled = true;
            txtPrecioBOB.IsEnabled = true;
            txtObservaciones.IsEnabled = true;

            cbxCategoria.IsEnabled = true;
            cbxCondicion.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtNombreProducto.IsEnabled = false;
            txtIdentificador.IsEnabled = false;
            txtCostoUSD.IsEnabled = false;
            txtCostoBOB.IsEnabled = false;
            txtPrecioUSD.IsEnabled = false;
            txtPrecioBOB.IsEnabled = false;
            txtObservaciones.IsEnabled = false;

            cbxCategoria.IsEnabled = false;
            cbxCondicion.IsEnabled = false;
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
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

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        void CleanText()
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }
        public void labelDanger(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.Background = new SolidColorBrush(Colors.Red);
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public string Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, string valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }
        private void btnPrintQR_Click(object sender, RoutedEventArgs e)
        {
            if (producto != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está seguro de imprimir la etiqueta con el código de producto: " + producto.CodigoSublote + "?", "Imprimir etiqueta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var label = DYMO.Label.Framework.Label.Open("LabelWriterCodigoQRProducto.label");
                    label.SetObjectText("lblCodigoSublote", producto.CodigoSublote);
                    label.SetObjectText("lblCodigoQR", producto.CodigoSublote);
                    label.Print("DYMO LabelWriter 450 Turbo");
                }
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA IMPRIMIR LA ETIQUETA CON EL CÓDIGO DE PRODUCTO DEBE SELECCIONAR UN REGISTRO!";
            }
        }

    }
}
