using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Inventario_Cantidad.xaml
    /// </summary>
    public partial class winProducto_Inventario_Cantidad : Window
    {
        ProductoImpl implProducto;
        //Implementaciones para obtener ID's y Nombres para los Combobox
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        CondicionImpl implCondicion;
        public winProducto_Inventario_Cantidad()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetGroupConcatSucursal();
            cbxGetSucursalFromDatabase();
            cbxGetGroupConcatCategoria();
            cbxGetCategoriaFromDatabase();
            cbxGetGroupConcatCondicion();
            cbxGetCondicionFromDatabase();
            txtBuscar.Focus();
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
        void cbxGetGroupConcatCategoria()
        {
            try
            {
                implCategoria = new CategoriaImpl();
                string categoriaGroupConcatID = implCategoria.SelectGroupConcatIDForComboBox();
                if (categoriaGroupConcatID != null)
                {
                    cbxCategoria.Items.Add(new ComboboxItem("TODOS", categoriaGroupConcatID));
                }
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
        void cbxGetGroupConcatCondicion()
        {
            try
            {
                implCondicion = new CondicionImpl();
                string condicionGroupConcatID = implCondicion.SelectGroupConcatIDForComboBox();
                if (condicionGroupConcatID != null)
                {
                    cbxCondicion.Items.Add(new ComboboxItem("TODOS", condicionGroupConcatID));
                }
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
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        void SelectLike()
        {
            if (string.IsNullOrEmpty(txtMayorA.Text))
            {
                txtMayorA.Text = "1";
            }
            if (string.IsNullOrEmpty(txtMenorA.Text))
            {
                txtMenorA.Text = "999";
            }
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectLikeInventoryOnlyQuantityFilter(txtBuscar.Text, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCondicion.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, int.Parse(txtMayorA.Text), int.Parse(txtMenorA.Text)).DefaultView;
                /*dgvDatos.Columns[0].Visibility = Visibility.Collapsed;*/
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeInventoryOnlyQuantityFilter(txtBuscar.Text, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCondicion.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, int.Parse(txtMayorA.Text), int.Parse(txtMenorA.Text)).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        void CleanText()
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }

        private void txtMayorA_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
    }
}
