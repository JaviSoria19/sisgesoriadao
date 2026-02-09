using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Linq;
using System.Windows;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winTransferencia.xaml
    /// </summary>
    public partial class winTransferencia : Window
    {
        ProductoImpl implProducto;
        SucursalImpl implSucursal;
        public winTransferencia()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxSelectSucursalFromDatabase();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void Select()
        {
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectMovementsHistory().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.SelectMovementsHistory().Rows.Count;
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
                dgvDatos.ItemsSource = implProducto.SelectLikeMovementsHistory(cbxSucursalOrigen.SelectedItem.ToString(), cbxSucursalDestino.SelectedItem.ToString(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implProducto.SelectLikeMovementsHistory(cbxSucursalOrigen.SelectedItem.ToString(), cbxSucursalDestino.SelectedItem.ToString(), dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Select();
            cbxSucursalOrigen.SelectedIndex = 0;
            cbxSucursalDestino.SelectedIndex = 0;
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        void cbxSelectSucursalFromDatabase()
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
                                            Valor = Convert.ToByte(dr["idSucursal"]),
                                            Texto = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursalOrigen.Items.Add(item);
                    cbxSucursalDestino.Items.Add(item);
                }
                cbxSucursalOrigen.SelectedIndex = 0;
                cbxSucursalDestino.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            public ComboboxItem()
            {

            }
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDatos);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }
        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPDF(dgvDatos, "TRANSFERENCIAS_HISTORIAL");
        }
    }
}
