using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winSucursal.xaml
    /// </summary>
    public partial class winSucursal : Window
    {
        SucursalImpl implSucursal;
        Sucursal sucursal;
        byte operacion;
        public winSucursal()
        {
            InitializeComponent();
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            labelClear(lblInfo);
            EnabledButtons();
            this.operacion = 1;
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (sucursal != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UNA SUCURSAL DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sucursal != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        implSucursal = new SucursalImpl();
                        int n = implSucursal.Delete(sucursal);
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
                lblInfo.Content = "¡PARA ELIMINAR UNA SUCURSAL DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (operacion)
            {
                //INSERT
                case 1:
                    if (string.IsNullOrEmpty(txtNombreSucursal.Text) != true && string.IsNullOrEmpty(txtDireccion.Text) != true &&
                        string.IsNullOrEmpty(txtCorreo.Text) != true && string.IsNullOrEmpty(txtTelefono.Text) != true)
                    {
                        sucursal = new Sucursal(txtNombreSucursal.Text.Trim(), txtDireccion.Text.Trim(), txtCorreo.Text.Trim(), txtTelefono.Text.Trim());
                        implSucursal = new SucursalImpl();
                        try
                        {
                            int n = implSucursal.Insert(sucursal);
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
                        MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
                    }
                    break;
                //UPDATE
                case 2:
                    if (string.IsNullOrEmpty(txtNombreSucursal.Text) != true && string.IsNullOrEmpty(txtDireccion.Text) != true &&
                        string.IsNullOrEmpty(txtCorreo.Text) != true && string.IsNullOrEmpty(txtTelefono.Text) != true)
                    {
                        sucursal.NombreSucursal = txtNombreSucursal.Text.Trim();
                        sucursal.Direccion = txtDireccion.Text.Trim();
                        sucursal.Correo = txtCorreo.Text.Trim();
                        implSucursal = new SucursalImpl();
                        try
                        {
                            int n = implSucursal.Update(sucursal);
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
        void EnabledButtons()
        {
            btnInsert.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            txtNombreSucursal.IsEnabled = true;
            txtNombreSucursal.Focus();
            txtDireccion.IsEnabled = true;
            txtCorreo.IsEnabled = true;
            txtTelefono.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtNombreSucursal.IsEnabled = false;
            txtDireccion.IsEnabled = false;
            txtCorreo.IsEnabled = false;
            txtTelefono.IsEnabled = false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
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
                    byte id = byte.Parse(d.Row.ItemArray[0].ToString());
                    implSucursal = new SucursalImpl();
                    sucursal = implSucursal.Get(id);
                    if (sucursal != null)
                    {
                        txtNombreSucursal.Text = sucursal.NombreSucursal.Trim();
                        txtDireccion.Text = sucursal.Direccion.Trim();
                        txtCorreo.Text = sucursal.Correo.Trim();
                        txtTelefono.Text = sucursal.Telefono.Trim();
                        labelSuccess(lblInfo);
                        lblInfo.Content = "SUCURSAL SELECCIONADA.";
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
        private void Select()
        {
            try
            {
                implSucursal = new SucursalImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implSucursal.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implSucursal.Select().Rows.Count;
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
                dgvDatos.ItemsSource = implSucursal.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implSucursal.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void txtTelefono_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
    }
}
