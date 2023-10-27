﻿using sisgesoriadao.Implementation;
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
    /// Lógica de interacción para winCliente.xaml
    /// </summary>
    public partial class winCliente : Window
    {
        ClienteImpl implCliente;
        Cliente cliente;
        byte operacion;
        public winCliente()
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
            if (cliente != null)
            {
                labelClear(lblInfo);
                EnabledButtons();
                this.operacion = 2;
            }
            else
            {
                labelWarning(lblInfo);
                lblInfo.Content = "¡PARA MODIFICAR UN CLIENTE DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cliente != null)
            {
                labelClear(lblInfo);
                if (MessageBox.Show("Está realmente segur@ de eliminar el registro?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        implCliente = new ClienteImpl();
                        int n = implCliente.Delete(cliente);
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
                lblInfo.Content = "¡PARA ELIMINAR UN CLIENTE DEBE SELECCIONAR UN REGISTRO!";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (operacion)
            {
                //INSERT
                case 1:
                    if (string.IsNullOrEmpty(txtNombre.Text) != true && string.IsNullOrEmpty(txtNumeroCelular.Text) != true && string.IsNullOrEmpty(txtNumeroCI.Text) != true)
                    {
                        cliente = new Cliente(txtNombre.Text.Trim(), txtNumeroCelular.Text.Trim(), txtNumeroCI.Text.Trim());
                        implCliente = new ClienteImpl();
                        try
                        {
                            int n = implCliente.Insert(cliente);
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
                    if (string.IsNullOrEmpty(txtNombre.Text) != true && string.IsNullOrEmpty(txtNumeroCelular.Text) != true && string.IsNullOrEmpty(txtNumeroCI.Text) != true)
                    {
                        cliente.Nombre = txtNombre.Text.Trim();
                        cliente.NumeroCelular = txtNumeroCelular.Text.Trim();
                        cliente.NumeroCI = txtNumeroCI.Text.Trim();
                        implCliente = new ClienteImpl();
                        try
                        {
                            int n = implCliente.Update(cliente);
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
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
            if (e.Key == Key.Escape)
            {
                CleanText();
            }
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    int id = int.Parse(d.Row.ItemArray[0].ToString());
                    implCliente = new ClienteImpl();
                    cliente = implCliente.Get(id);
                    if (cliente != null)
                    {
                        txtNombre.Text = cliente.Nombre.Trim();
                        txtNumeroCelular.Text = cliente.NumeroCelular.Trim();
                        txtNumeroCI.Text = cliente.NumeroCI.Trim();

                        labelSuccess(lblInfo);
                        lblInfo.Content = "CLIENTE SELECCIONADO.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
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
                implCliente = new ClienteImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCliente.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCliente.Select().Rows.Count;
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
                dgvDatos.ItemsSource = implCliente.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implCliente.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
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

            txtNombre.IsEnabled = true;
            txtNombre.Focus();
            txtNumeroCelular.IsEnabled = true;
            txtNumeroCI.IsEnabled = true;
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

        private void txtNumeroCI_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        void CleanText()
        {
            txtBuscar.Text = "";
            txtBuscar.Focus();
        }
    }
}
