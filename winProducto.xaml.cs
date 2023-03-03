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
using MySql.Data.MySqlClient;
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
        public winProducto()
        {
            InitializeComponent();
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetCategoriaFromDatabase();
            cbxGetMarcaFromDatabase();
            cbxGetSucursalFromDatabase();
            cbxMoneda.Items.Add("BS");
            cbxMoneda.Items.Add("USD");
            cbxMoneda.SelectedIndex = 0;
        }
        private void Select()
        {
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.Select().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            labelClear(lblInfo);
            EnabledButtons();
            this.operacion = 1;
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
                //INSERT
                case 1:
                    producto = new Producto(
                        byte.Parse((cbxSucursal.SelectedItem as ComboboxItem).Valor.ToString()),
                        byte.Parse((cbxMarca.SelectedItem as ComboboxItem).Valor.ToString()),
                        byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString()),
                        txtNombreProducto.Text.Trim(),
                        txtColor.Text.Trim(),
                        txtNumeroSerie.Text.Trim(),
                        double.Parse(txtPrecio.Text.Trim()),
                        cbxMoneda.Text,
                        Session.IdUsuario
                        );
                    implProducto = new ProductoImpl();
                    try
                    {
                        int n = implProducto.Insert(producto);
                        if (n > 0)
                        {
                            labelSuccess(lblInfo);
                            lblInfo.Content = "REGISTRO INSERTADO CON ÉXITO.";
                            Select();
                            DisabledButtons();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                    }
                    break;
                //UPDATE
                case 2:
                    producto.IdSucursal = byte.Parse((cbxSucursal.SelectedItem as ComboboxItem).Valor.ToString());
                    producto.IdMarca = byte.Parse((cbxMarca.SelectedItem as ComboboxItem).Valor.ToString());
                    producto.IdCategoria = byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString());
                    producto.NombreProducto = txtNombreProducto.Text.Trim();
                    producto.Color = txtColor.Text.Trim();
                    producto.NumeroSerie = txtNumeroSerie.Text.Trim();
                    producto.Precio = double.Parse(txtPrecio.Text.Trim());
                    producto.Moneda = cbxMoneda.Text;
                    producto.IdUsuario = Session.IdUsuario;
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
                    break;
                default:
                    break;
            }
            //La siguiente línea de código comentada fue una prueba para obtener y aprender como funciona los valores del combobox.
            //MessageBox.Show("VALOR INTERNO: " + (cbxMarca.SelectedItem as ComboboxItem).Valor.ToString() + " ¿SELECTED ITEM?: " + cbxMarca.SelectedItem + " ¿SELECTED INDEX?: " + cbxMarca.SelectedIndex + "REGISTROS : " + cbxMarca.Items.Count);
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == null || txtBuscar.Text == "")
            {
                Select();
            }
            else
            {
                try
                {
                    dgvDatos.ItemsSource = null;
                    dgvDatos.ItemsSource = implProducto.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                    dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                    lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DisabledButtons();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            winMainAdmin winMainAdmin = new winMainAdmin();
            winMainAdmin.Show();
            this.Close();
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
            txtColor.IsEnabled = true;
            txtNumeroSerie.IsEnabled = true;
            txtPrecio.IsEnabled = true;
            cbxMoneda.IsEnabled = true;

            cbxSucursal.IsEnabled = true;
            cbxMarca.IsEnabled = true;
            cbxCategoria.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtNombreProducto.IsEnabled = false;
            txtColor.IsEnabled = false;
            txtNumeroSerie.IsEnabled = false;
            txtPrecio.IsEnabled = false;
            cbxMoneda.IsEnabled = false;

            cbxSucursal.IsEnabled = false;
            cbxMarca.IsEnabled = false;
            cbxCategoria.IsEnabled = false;
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
                        txtNombreProducto.Text = producto.NombreProducto.Trim();
                        txtColor.Text = producto.Color.Trim();
                        txtNumeroSerie.Text = producto.NumeroSerie.Trim();
                        txtPrecio.Text = producto.Precio.ToString();
                        for (int i = 0; i < cbxMarca.Items.Count; i++)
                        {
                            cbxMarca.SelectedIndex = i;
                            if ((cbxMarca.SelectedItem as ComboboxItem).Valor == producto.IdMarca)
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < cbxSucursal.Items.Count; i++)
                        {
                            cbxSucursal.SelectedIndex = i;
                            if ((cbxSucursal.SelectedItem as ComboboxItem).Valor == producto.IdSucursal)
                            {
                                break;
                            }
                        }
                        for (int i = 0; i < cbxCategoria.Items.Count; i++)
                        {
                            cbxCategoria.SelectedIndex = i;
                            if ((cbxCategoria.SelectedItem as ComboboxItem).Valor == producto.IdCategoria)
                            {
                                break;
                            }
                        }
                        if (producto.Moneda == "BS")
                        {
                            cbxMoneda.SelectedIndex = 0;
                        }
                        else
                        {
                            cbxMoneda.SelectedIndex = 1;
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
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public byte Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, byte valor)
            {
                Texto = texto;
                Valor = valor;
            }
        }
        void cbxGetSucursalFromDatabase()
        {
            string connectionString = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT idSucursal, nombreSucursal FROM sucursal WHERE estado = 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader mySqldr;
            try
            {
                connection.Open();
                mySqldr = cmd.ExecuteReader();
                while (mySqldr.Read())
                {
                    cbxSucursal.Items.Add(new ComboboxItem(mySqldr.GetString("nombreSucursal"),mySqldr.GetByte("idSucursal")));
                }
                cbxSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        void cbxGetMarcaFromDatabase()
        {
            string connectionString = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT idMarca, nombreMarca FROM marca WHERE estado = 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader mySqldr;
            try
            {
                connection.Open();
                mySqldr = cmd.ExecuteReader();
                while (mySqldr.Read())
                {
                    cbxMarca.Items.Add(new ComboboxItem(mySqldr.GetString("nombreMarca"), mySqldr.GetByte("idMarca")));
                }
                cbxMarca.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        void cbxGetCategoriaFromDatabase()
        {
            string connectionString = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT idCategoria, nombreCategoria FROM categoria WHERE estado = 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader mySqldr;
            try
            {
                connection.Open();
                mySqldr = cmd.ExecuteReader();
                while (mySqldr.Read())
                {
                    cbxCategoria.Items.Add(new ComboboxItem(mySqldr.GetString("nombreCategoria"), mySqldr.GetByte("idCategoria")));
                }
                cbxCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
