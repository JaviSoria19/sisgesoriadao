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
    /// Lógica de interacción para winUsuario.xaml
    /// </summary>
    public partial class winUsuario : Window
    {
        UsuarioImpl implUsuario;
        Usuario usuario;
        EmpleadoImpl implEmpleado;
        Empleado empleado;
        byte operacion;
        public winUsuario()
        {
            InitializeComponent();
        }        
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (empleado == null)
            {
                labelWarning(lblInfo);
                lblInfo.Content = "PARA AÑADIR UN NUEVO USUARIO DEBE SELECCIONAR UN EMPLEADO QUE NO TENGA UN USUARIO.";
            }
            else
            {
                labelSuccess(lblInfo);
                lblInfo.Content = "EMPLEADO SELECCIONADO: " + empleado.Nombres + " " + empleado.PrimerApellido + ",INGRESE LOS DATOS SOLICITADOS.";
                EnabledButtons();
                this.operacion = 1;
            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (usuario != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UN USUARIO DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (usuario != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        implUsuario = new UsuarioImpl();
                        int n = implUsuario.Delete(usuario);
                        if (n > 0)
                        {
                            labelSuccess(lblInfo);
                            lblInfo.Content = "REGISTRO ELIMINADO CON ÉXITO.";
                            Select();
                            SelectEmployees();
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
                lblInfo.Content = "¡PARA ELIMINAR UN EMPLEADO DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (operacion)
            {
                //INSERT
                case 1:
                    //VALIDACIÓN DE DATOS.
                    if (string.IsNullOrEmpty(txtUsuario.Text)!=true && string.IsNullOrEmpty(txtPin.Text)!=true && string.IsNullOrEmpty(txtContrasenha.Text)!=true && string.IsNullOrEmpty(txtReContrasenha.Text)!=true)
                    {
                        if (txtContrasenha.Text != txtReContrasenha.Text)
                        {
                            MessageBox.Show("Las contraseñas no coinciden, por favor intente nuevamente.");
                        }
                        else
                        {
                            usuario = new Usuario(empleado.IdEmpleado, 1, txtUsuario.Text.Trim(), txtContrasenha.Text.Trim(), byte.Parse((cbxRol.SelectedItem as ComboboxItem).Value.ToString()), txtPin.Text.Trim());
                            implUsuario = new UsuarioImpl();
                            try
                            {
                                int n = implUsuario.Insert(usuario);
                                if (n > 0)
                                {
                                    labelSuccess(lblInfo);
                                    lblInfo.Content = "REGISTRO INSERTADO CON ÉXITO.";
                                    Select();
                                    try
                                    {
                                        int n2 = implEmpleado.UpdateCreatedUser(empleado);
                                        if (n2 > 0)
                                        {
                                            SelectEmployees();
                                            empleado = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show("Segunda transacción no completada, comuníquese con el Administrador de Sistemas.");
                                    }
                                    DisabledButtons();
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                            }
                            //MessageBox.Show("ID Empleado: " + usuario.IdEmpleado + "Usuario: " + usuario.NombreUsuario + "Contraseña: " + usuario.Contrasenha + "Rol: " + usuario.Rol + "Pin: " + usuario.Pin);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
                    }
                    break;
                //UPDATE
                case 2:
                    if (string.IsNullOrEmpty(txtUsuario.Text) != true && string.IsNullOrEmpty(txtPin.Text) != true)
                    {
                        usuario.NombreUsuario = txtUsuario.Text.Trim();
                        usuario.Rol = byte.Parse((cbxRol.SelectedItem as ComboboxItem).Value.ToString());
                        usuario.Pin = txtPin.Text.Trim();
                        implUsuario = new UsuarioImpl();
                        try
                        {
                            int n = implUsuario.Update(usuario);
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
                        MessageBox.Show("Por favor rellene el usuario y el pin. (*)");
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            ComboboxItem item = new ComboboxItem();
            item.Text = "ADMINISTRADOR";
            item.Value = 1;
            cbxRol.Items.Add(item);
            ComboboxItem item2 = new ComboboxItem();
            item2.Text = "VENDEDOR";
            item2.Value = 2;
            cbxRol.Items.Add(item2);
            cbxRol.SelectedIndex = 0;
            //retorna el valor de 1:
            //MessageBox.Show((cbxRol.SelectedItem as ComboboxItem).Value.ToString());
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
            SelectEmployees();
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
                    implUsuario = new UsuarioImpl();
                    usuario = implUsuario.Get(id);
                    if (usuario != null)
                    {
                        txtUsuario.Text = usuario.NombreUsuario.Trim();
                        if (usuario.Rol == 1)
                        {
                            cbxRol.SelectedIndex = 0;
                        }
                        else
                        {
                            cbxRol.SelectedIndex = 1;
                        }
                        txtPin.Text = usuario.Pin;

                        labelSuccess(lblInfo);
                        lblInfo.Content = "USUARIO SELECCIONADO.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void dgvEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvEmpleados.SelectedItem != null && dgvEmpleados.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvEmpleados.SelectedItem;
                    byte id = byte.Parse(d.Row.ItemArray[0].ToString());
                    implEmpleado = new EmpleadoImpl();
                    empleado = implEmpleado.Get(id);
                    if (empleado != null)
                    {
                        //lblDataGridEmpleados.Content = empleado.IdEmpleado;
                        labelSuccess(lblInfo);
                        lblInfo.Content = "ATENCIÓN: UN EMPLEADO HA SIDO SELECCIONADO, AHORA PUEDE CREAR UN USUARIO.";
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
                implUsuario = new UsuarioImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implUsuario.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                dgvDatos.Columns[1].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implUsuario.Select().Rows.Count;
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
                dgvDatos.ItemsSource = implUsuario.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                dgvDatos.Columns[1].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implUsuario.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectEmployees()
        {
            try
            {
                implEmpleado = new EmpleadoImpl();
                dgvEmpleados.ItemsSource = null;
                dgvEmpleados.ItemsSource = implEmpleado.SelectEmployeesWithoutUsers().DefaultView;
                dgvEmpleados.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridEmpleados.Content = "NÚMERO DE REGISTROS: " + implEmpleado.SelectEmployeesWithoutUsers().Rows.Count;
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

            txtUsuario.IsEnabled = true;
            txtUsuario.Focus();
            txtContrasenha.IsEnabled = true;
            txtReContrasenha.IsEnabled = true;
            cbxRol.IsEnabled = true;
            txtPin.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtUsuario.IsEnabled = false;
            txtUsuario.Focus();
            txtContrasenha.IsEnabled = false;
            txtReContrasenha.IsEnabled = false;
            cbxRol.IsEnabled = false;
            txtPin.IsEnabled = false;
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
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------        
        private void txtPin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public byte Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
