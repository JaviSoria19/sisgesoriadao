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
    /// Lógica de interacción para winProductoComun.xaml
    /// </summary>
    public partial class winProductoComun : Window
    {
        ProductoComunImpl implProductoComun;
        ProductoComun productoComun;
        byte operacion;
        public winProductoComun()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }
        private void Select()
        {
            try
            {
                implProductoComun = new ProductoComunImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProductoComun.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProductoComun.Select().Rows.Count;
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
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProductoComun.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProductoComun.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
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
            if (productoComun != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UN PRODUCTO COMUN DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (productoComun != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        implProductoComun = new ProductoComunImpl();
                        int n = implProductoComun.Delete(productoComun);
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
                lblInfo.Content = "¡PARA ELIMINAR UN PRODUCTO COMUN DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (operacion)
            {
                //INSERT
                case 1:
                    if (string.IsNullOrEmpty(txtProductoComun.Text) != true && string.IsNullOrEmpty(txtPrecioMinimo.Text) != true && string.IsNullOrEmpty(txtPrecioSugerido.Text) != true)
                    {
                        if (double.Parse(txtPrecioMinimo.Text.Trim()) <= double.Parse(txtPrecioSugerido.Text.Trim()))
                        {
                            productoComun = new ProductoComun(txtProductoComun.Text.Trim(), double.Parse(txtPrecioMinimo.Text.Trim()), double.Parse(txtPrecioSugerido.Text.Trim()));
                            implProductoComun = new ProductoComunImpl();
                            try
                            {
                                int n = implProductoComun.Insert(productoComun);
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
                        }
                        else
                        {
                            MessageBox.Show("El precio de venta no puede ser inferior al precio mínimo!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
                    }
                    break;
                //UPDATE
                case 2:
                    if (string.IsNullOrEmpty(txtProductoComun.Text) != true && string.IsNullOrEmpty(txtPrecioMinimo.Text) != true && string.IsNullOrEmpty(txtPrecioSugerido.Text) != true)
                    {
                        productoComun.NombreProductoComun = txtProductoComun.Text.Trim();
                        productoComun.PrecioMinimo = double.Parse(txtPrecioMinimo.Text.Trim());
                        productoComun.PrecioSugerido = double.Parse(txtPrecioSugerido.Text.Trim());
                        implProductoComun = new ProductoComunImpl();
                        try
                        {
                            int n = implProductoComun.Update(productoComun);
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
                    else
                    {
                        MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
                    }
                    break;
                default:
                    break;
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DisabledButtons();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == null || txtBuscar.Text == "")
            {
                Select();
            }
            else
            {
                SelectLike();
            }
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                    implProductoComun = new ProductoComunImpl();
                    productoComun = implProductoComun.Get(id);
                    if (productoComun != null)
                    {
                        txtProductoComun.Text = productoComun.NombreProductoComun.Trim();
                        txtPrecioMinimo.Text = productoComun.PrecioMinimo.ToString().Trim();
                        txtPrecioSugerido.Text = productoComun.PrecioSugerido.ToString().Trim();
                        labelSuccess(lblInfo);
                        lblInfo.Content = "PRODUCTO COMUN SELECCIONADO.";
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
                if (string.IsNullOrEmpty(txtBuscar.Text))
                {
                    Select();
                }
                else
                {
                    SelectLike();
                }
            }
        }
        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }
        void EnabledButtons()
        {
            btnInsert.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            txtProductoComun.IsEnabled = true;
            txtProductoComun.Focus();
            txtPrecioMinimo.IsEnabled = true;
            txtPrecioSugerido.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtProductoComun.IsEnabled = false;
            txtPrecioMinimo.IsEnabled = false;
            txtPrecioSugerido.IsEnabled = false;
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
        private void txtPrecios_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        private void btnAddSale_Click(object sender, RoutedEventArgs e)
        {
            winProductoComun_Venta_Insert winProductoComun_Venta_Insert = new winProductoComun_Venta_Insert();
            winProductoComun_Venta_Insert.Show();
        }

        private void btnViewReport_Click(object sender, RoutedEventArgs e)
        {
            winProductoComun_Venta_Historial winProductoComun_Venta_Historial = new winProductoComun_Venta_Historial();
            winProductoComun_Venta_Historial.Show();
        }
    }
}
