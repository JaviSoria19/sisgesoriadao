using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;//ADO.NET
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Update.xaml
    /// </summary>
    public partial class winProducto_Update : Window
    {
        CategoriaImpl implCategoria;
        CondicionImpl implCondicion;
        ProductoImpl implProducto;
        private ObservableCollection<DataGridRowDetalleHelper> listaHelper = new ObservableCollection<DataGridRowDetalleHelper>();
        bool habilitarComboBox = false;
        List<Producto> listaProducto = new List<Producto>();
        public winProducto_Update()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetCategoriaFromDatabase();
            cbxGetCondicionFromDatabase();
            txtBuscar.Focus();
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
        private void cbxCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (habilitarComboBox == true)
            {
                foreach (var item in listaHelper)
                {
                    item.IdCategoria = byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString());
                }
                dgvProductos.Items.Refresh();
            }
        }
        private void cbxCondicion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (habilitarComboBox == true)
            {
                foreach (var item in listaHelper)
                {
                    item.IdCondicion = byte.Parse((cbxCondicion.SelectedItem as ComboboxItem).Valor.ToString());
                }
                dgvProductos.Items.Refresh();
            }
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Select();
            }
            if (e.Key == Key.Escape)
            {
                CleanText();
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
                if (indexSeleccionado == 5)/*NOMBRE PRODUCTO*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        listaHelper[dgvProductos.SelectedIndex].NombreProducto = valorNuevo.Text.Trim().ToString();
                    }
                    else
                    {
                        MessageBox.Show("EL NOMBRE DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 6)/*IDENTIFICADOR*/
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
                else if (indexSeleccionado == 7)/*COSTO USD*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                        {
                            listaHelper[i].CostoUSD = double.Parse(valorNuevo.Text.Trim());
                            listaHelper[i].CostoBOB = Math.Round(listaHelper[i].CostoUSD * Session.Ajuste_Cambio_Dolar, 2);
                        }
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.PrecioVentaUSD)
                        {
                            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                            {
                                listaHelper[i].PrecioVentaUSD = listaHelper[i].CostoUSD + 10;
                                listaHelper[i].PrecioVentaBOB = Math.Round(listaHelper[i].CostoBOB + (Session.Ajuste_Cambio_Dolar * 10), 2);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL COSTO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 8)/*COSTO BOB*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                        {
                            listaHelper[i].CostoBOB = double.Parse(valorNuevo.Text.Trim());
                            listaHelper[i].CostoUSD = Math.Round(listaHelper[i].CostoBOB / Session.Ajuste_Cambio_Dolar, 2);
                        }
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.PrecioVentaBOB)
                        {
                            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                            {
                                listaHelper[i].PrecioVentaBOB = listaHelper[i].CostoBOB + (Session.Ajuste_Cambio_Dolar * 10);
                                listaHelper[i].PrecioVentaUSD = Math.Round(listaHelper[i].CostoUSD + 10, 2);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("EL COSTO EN USD DEL PRODUCTO NO PUEDE ESTAR VACÍO!");
                    }
                }
                else if (indexSeleccionado == 9)/*PRECIO USD*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.CostoUSD)
                        {
                            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                            {
                                listaHelper[i].PrecioVentaUSD = double.Parse(valorNuevo.Text.Trim());
                                listaHelper[i].PrecioVentaBOB = Math.Round(listaHelper[i].PrecioVentaUSD * Session.Ajuste_Cambio_Dolar, 2);
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
                else if (indexSeleccionado == 10)/*PRECIO BOB*/
                {
                    if (!string.IsNullOrEmpty(valorNuevo.Text.Trim().ToString()))
                    {
                        if (double.Parse(valorNuevo.Text.Trim()) > filaSeleccionada.CostoBOB)
                        {
                            for (int i = dgvProductos.SelectedIndex; i < listaHelper.Count; i++)
                            {
                                listaHelper[i].PrecioVentaBOB = double.Parse(valorNuevo.Text.Trim());
                                listaHelper[i].PrecioVentaUSD = Math.Round(listaHelper[i].PrecioVentaBOB / Session.Ajuste_Cambio_Dolar, 2);
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
                else if (indexSeleccionado == 11)/*OBSERVACIONES*/
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
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }
        void Select()
        {
            if (!String.IsNullOrEmpty(txtBuscar.Text.Trim()))
            {
                try
                {
                    DataTable dt = new DataTable();
                    implProducto = new ProductoImpl();
                    dt = implProducto.SelectBatchOfProductsToUpdate(txtBuscar.Text.Trim());
                    listaHelper.Clear();
                    habilitarComboBox = false;
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            listaHelper.Add(new DataGridRowDetalleHelper
                            {
                                IdProducto = int.Parse(item[0].ToString()),
                                IdCategoria = byte.Parse(item[1].ToString()),
                                IdCondicion = byte.Parse(item[2].ToString()),
                                IdSublote = int.Parse(item[3].ToString()),
                                CodigoSublote = item[4].ToString(),
                                NombreProducto = item[5].ToString(),
                                Identificador = item[6].ToString(),
                                CostoUSD = double.Parse(item[7].ToString()),
                                CostoBOB = double.Parse(item[8].ToString()),
                                PrecioVentaUSD = double.Parse(item[9].ToString()),
                                PrecioVentaBOB = double.Parse(item[10].ToString()),
                                Observaciones = item[11].ToString(),
                                Estado = item[12].ToString()
                            }
                            );
                        }
                        cbxCategoria.IsEnabled = true;
                        cbxCondicion.IsEnabled = true;
                        dgvProductos.IsEnabled = true;
                        btnPrintLabel.IsEnabled = true;
                        btnSave.IsEnabled = true;
                        txtBuscar.Text = "";
                        for (int i = 0; i < cbxCategoria.Items.Count; i++)
                        {
                            cbxCategoria.SelectedIndex = i;
                            if ((cbxCategoria.SelectedItem as ComboboxItem).Valor == listaHelper[0].IdCategoria)
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < cbxCondicion.Items.Count; i++)
                        {
                            cbxCondicion.SelectedIndex = i;
                            if ((cbxCondicion.SelectedItem as ComboboxItem).Valor == listaHelper[0].IdCondicion)
                            {
                                break;
                            }
                        }
                        habilitarComboBox = true;
                    }
                    else
                    {
                        MessageBox.Show("Se ha realizado la búsqueda pero no se encontraron resultados.\nTexto Ingresado: " + txtBuscar.Text.Trim());
                        cbxCategoria.IsEnabled = false;
                        cbxCondicion.IsEnabled = false;
                        dgvProductos.IsEnabled = false;
                        btnPrintLabel.IsEnabled = false;
                        btnSave.IsEnabled = false;
                        txtBuscar.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("¡El campo de búsqueda no puede estar vacía!");
            }
        }
        private void btnPrintLabel_Click(object sender, RoutedEventArgs e)
        {
            if (listaHelper.Count > 0)
            {
                var label = DYMO.Label.Framework.Label.Open("LabelWriterCodigoQRProducto.label");
                foreach (var item in listaHelper)
                {
                    if (item.Estado == "Disponible")
                    {
                        label.SetObjectText("lblCodigoSublote", item.CodigoSublote);
                        label.SetObjectText("lblCodigoQR", item.CodigoSublote);
                        label.Print("DYMO LabelWriter 450 Turbo");
                    }
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            listaProducto.Clear();
            foreach (var item in listaHelper)
            {
                if (item.Estado == "Disponible")
                {
                    listaProducto.Add(new Producto
                    {
                        IdProducto = item.IdProducto,
                        IdCategoria = item.IdCategoria,
                        IdCondicion = item.IdCondicion,
                        NombreProducto = item.NombreProducto,
                        Identificador = item.Identificador,
                        CostoUSD = item.CostoUSD,
                        CostoBOB = item.CostoBOB,
                        PrecioVentaUSD = item.PrecioVentaUSD,
                        PrecioVentaBOB = item.PrecioVentaBOB,
                        Observaciones = item.Observaciones
                    });
                }
            }
            implProducto = new ProductoImpl();
            string mensaje = implProducto.UpdateBatchOfProductsTransaction(listaProducto);
            if (mensaje == "LOTE MODIFICADO EXITOSAMENTE.")
            {
                MessageBox.Show(mensaje);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje);
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
        public class DataGridRowDetalleHelper
        {
            public int IdProducto { get; set; }
            public byte IdCategoria { get; set; }
            public byte IdCondicion { get; set; }
            public int IdSublote { get; set; }
            public string CodigoSublote { get; set; }
            public string NombreProducto { get; set; }
            public string Identificador { get; set; }
            public double CostoUSD { get; set; }
            public double CostoBOB { get; set; }
            public double PrecioVentaUSD { get; set; }
            public double PrecioVentaBOB { get; set; }
            public string Observaciones { get; set; }
            public string Estado { get; set; }
        }
        private void btndgvImprimirEtiqueta(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Está seguro de imprimir la etiqueta con el código de producto: " + listaHelper[dgvProductos.SelectedIndex].CodigoSublote + "?", "Imprimir etiqueta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var label = DYMO.Label.Framework.Label.Open("LabelWriterCodigoQRProducto.label");
                    label.SetObjectText("lblCodigoSublote", listaHelper[dgvProductos.SelectedIndex].CodigoSublote);
                    label.SetObjectText("lblCodigoQR", listaHelper[dgvProductos.SelectedIndex].CodigoSublote);
                    label.Print("DYMO LabelWriter 450 Turbo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            Session.ExportarAPortapapeles(dgvProductos);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvProductos);
        }
    }
}
