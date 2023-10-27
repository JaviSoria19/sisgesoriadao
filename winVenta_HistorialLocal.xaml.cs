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

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_HistorialLocal.xaml
    /// </summary>
    public partial class winVenta_HistorialLocal : Window
    {
        VentaImpl implVenta;
        string cadenaAuxiliar = string.Empty;
        ClienteImpl implCliente;
        public winVenta_HistorialLocal()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            if (Session.Rol != 1)
            {
                btnDeletedSales.IsEnabled = false;
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
        private void btnSearchByID_Click(object sender, RoutedEventArgs e)
        {
            SelectLikeByID();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Now;
            dtpFechaInicio.SelectedDate = DateTime.Now;
        }
        private void txtBuscar_Producto_o_Codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLike();
            }
            if (e.Key == Key.Escape)
            {
                if (sender == (sender as TextBox))
                {
                    (sender as TextBox).Text = "";
                }
                if (sender == (sender as AutoCompleteBox))
                {
                    (sender as AutoCompleteBox).Text = "";
                }
            }
        }
        private void txtBuscar_IDVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLikeByID();
            }
            if (e.Key == Key.Escape)
            {
                txtBuscar_IDVenta.Text = "";
                txtBuscar_IDVenta.Focus();
            }
        }
        private void txtBuscar_IDVenta_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
        private void SelectLike()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                if (string.IsNullOrEmpty(txtBuscar_Producto_o_Codigo.Text.Trim()) == false && string.IsNullOrEmpty(txtBuscar_Cliente_o_CI.Text.Trim()) == true)
                {
                    cadenaAuxiliar = txtBuscar_Producto_o_Codigo.Text.Trim();
                    dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasLocales(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, txtBuscar_Producto_o_Codigo.Text.Trim(), cadenaAuxiliar).DefaultView;
                }
                else if (string.IsNullOrEmpty(txtBuscar_Producto_o_Codigo.Text.Trim()) == true && string.IsNullOrEmpty(txtBuscar_Cliente_o_CI.Text.Trim()) == false)
                {
                    cadenaAuxiliar = txtBuscar_Cliente_o_CI.Text.Trim();
                    dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasLocales(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, cadenaAuxiliar, txtBuscar_Cliente_o_CI.Text.Trim()).DefaultView;
                }
                else
                {
                    dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasLocales(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, txtBuscar_Producto_o_Codigo.Text.Trim(), txtBuscar_Cliente_o_CI.Text.Trim()).DefaultView;
                }
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + dgvDatos.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectLikeByID()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasLocalesByID(int.Parse(txtBuscar_IDVenta.Text.Trim())).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + dgvDatos.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_Update winVenta_Update = new winVenta_Update();
                    winVenta_Update.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void btnDeletedSales_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Eliminados winVenta_Eliminados = new winVenta_Eliminados();
            winVenta_Eliminados.Show();
        }

        private void txtBuscar_Cliente_o_CI_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ComboboxItem> listcomboboxCliente = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCliente = new ClienteImpl();
                dataTable = implCliente.SelectCustomerNamesForComboBox();
                listcomboboxCliente = (from DataRow dr in dataTable.Rows
                                       select new ComboboxItem()
                                       {
                                           Valor = Convert.ToInt32(dr["idCliente"]),
                                           Texto = dr["nombre"].ToString()
                                       }).ToList();
                txtBuscar_Cliente_o_CI.ItemsSource = listcomboboxCliente;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            public ComboboxItem(string texto, byte valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }
    }
}
