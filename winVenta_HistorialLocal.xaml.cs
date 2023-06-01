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
    /// Lógica de interacción para winVenta_HistorialLocal.xaml
    /// </summary>
    public partial class winVenta_HistorialLocal : Window
    {
        VentaImpl implVenta;
        string cadenaAuxiliar = string.Empty;
        public winVenta_HistorialLocal()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
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
        }
        private void txtBuscar_IDVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLikeByID();
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
            try
            {
                DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                winVenta_Detalle winVenta_Detalle = new winVenta_Detalle();
                winVenta_Detalle.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
    }
}
