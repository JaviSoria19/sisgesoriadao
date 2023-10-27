using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCotizacion.xaml
    /// </summary>
    public partial class winCotizacion : Window
    {
        CotizacionImpl implCotizacion;
        public winCotizacion()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
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
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdCotizacion = int.Parse(d.Row.ItemArray[0].ToString());
                    winCotizacion_Detalle winCotizacion_Detalle = new winCotizacion_Detalle();
                    winCotizacion_Detalle.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Select()
        {
            try
            {
                implCotizacion = new CotizacionImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCotizacion.Select().DefaultView;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCotizacion.Select().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SelectLike()
        {
            if (string.IsNullOrEmpty(txtBuscar.Text) != true)
            {
                try
                {
                    implCotizacion = new CotizacionImpl();
                    dgvDatos.ItemsSource = null;
                    dgvDatos.ItemsSource = implCotizacion.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                    lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implCotizacion.SelectLike(txtBuscar.Text.Trim(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnQuotationAdd_Click(object sender, RoutedEventArgs e)
        {
            winCotizacion_Insert winCotizacion_Insert = new winCotizacion_Insert();
            winCotizacion_Insert.Show();
            this.Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdCotizacion = int.Parse(d.Row.ItemArray[0].ToString());
                    winCotizacion_Detalle winCotizacion_Detalle = new winCotizacion_Detalle();
                    winCotizacion_Detalle.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
