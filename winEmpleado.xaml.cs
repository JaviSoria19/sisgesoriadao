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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;//ADO.NET
using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class winEmpleado : Window
    {
        EmpleadoImpl implEmpleado;
        Empleado empleado;
        byte operacion;
        public winEmpleado()
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
        }
        private void Select()
        {
            try
            {
                implEmpleado = new EmpleadoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implEmpleado.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implEmpleado.Select().Rows.Count;
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
            if (empleado != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UN EMPLEADO DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (empleado!=null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?","Eliminar",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
                {
                    try
                    {
                        implEmpleado = new EmpleadoImpl();
                        int n = implEmpleado.Delete(empleado);
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
                    if (string.IsNullOrEmpty(txtNombre.Text)!=true && string.IsNullOrEmpty(txtPrimerApellido.Text) != true &&
                        string.IsNullOrEmpty(txtNumeroCelular.Text) != true && string.IsNullOrEmpty(txtNumeroCI.Text) != true
                        )
                    {
                        empleado = new Empleado(txtNombre.Text.Trim(), txtPrimerApellido.Text.Trim(), txtSegundoApellido.Text.Trim(), txtNumeroCelular.Text.Trim(), txtNumeroCI.Text.Trim());
                        implEmpleado = new EmpleadoImpl();
                        try
                        {
                            int n = implEmpleado.Insert(empleado);
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
                    //VALIDACIÓN DE DATOS.
                    if (string.IsNullOrEmpty(txtNombre.Text) != true && string.IsNullOrEmpty(txtPrimerApellido.Text) != true &&
                        string.IsNullOrEmpty(txtNumeroCelular.Text) != true && string.IsNullOrEmpty(txtNumeroCI.Text) != true
                        )
                    {
                        empleado.Nombres = txtNombre.Text.Trim();
                        empleado.PrimerApellido = txtPrimerApellido.Text.Trim();
                        empleado.SegundoApellido = txtSegundoApellido.Text.Trim();
                        empleado.NumeroCelular = txtNumeroCelular.Text.Trim();
                        empleado.NumeroCI = txtNumeroCI.Text.Trim();
                        implEmpleado = new EmpleadoImpl();
                        try
                        {
                            int n = implEmpleado.Update(empleado);
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
            if (txtBuscar.Text==null || txtBuscar.Text=="")
            {
                Select();
            }
            else
            {
                SelectLike();
            }
        }
        private void SelectLike()
        {
            try
            {
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implEmpleado.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implEmpleado.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            txtNombre.IsEnabled = true;
            txtNombre.Focus();
            txtNumeroCelular.IsEnabled = true;
            txtNumeroCI.IsEnabled = true;
            txtPrimerApellido.IsEnabled = true;
            txtSegundoApellido.IsEnabled = true;
        }
        void DisabledButtons()
        {
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;

            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            txtNombre.IsEnabled = false;
            txtNumeroCelular.IsEnabled = false;
            txtNumeroCI.IsEnabled = false;
            txtPrimerApellido.IsEnabled = false;
            txtSegundoApellido.IsEnabled = false;
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem!=null && dgvDatos.Items.Count>0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    byte id = byte.Parse(d.Row.ItemArray[0].ToString());
                    implEmpleado = new EmpleadoImpl();
                    empleado = implEmpleado.Get(id);
                    if (empleado!=null)
                    {
                        txtNombre.Text = empleado.Nombres.Trim();
                        txtPrimerApellido.Text = empleado.PrimerApellido.Trim();
                        txtSegundoApellido.Text = empleado.SegundoApellido.Trim();
                        txtNumeroCelular.Text = empleado.NumeroCelular.Trim();
                        txtNumeroCI.Text = empleado.NumeroCI.Trim();

                        labelSuccess(lblInfo);
                        lblInfo.Content = "EMPLEADO SELECCIONADO.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        public void TextBoxUppercase(object sender, KeyEventArgs e)
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
    }
}
