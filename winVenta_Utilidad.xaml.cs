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
    /// Lógica de interacción para winVenta.xaml
    /// </summary>
    public partial class winVenta_Utilidad : Window
    {
        VentaImpl implVenta;
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        UsuarioImpl implUsuario;
        double totalCosto = 0, totalVenta = 0, totalUtilidad = 0;
        public winVenta_Utilidad()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetGroupConcatSucursal();
            cbxGetSucursalFromDatabase();
            cbxGetGroupConcatCategoria();
            cbxGetCategoriaFromDatabase();
            cbxGetGroupConcatUsuarios();
            cbxGetUsuarioFromDatabase();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
        }
        private void SelectLike()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectLikeReporteUtilidades(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implVenta.SelectLikeReporteUtilidades(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor).Rows.Count;
                totalCosto = 0;
                totalVenta = 0;
                totalUtilidad = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    totalCosto += double.Parse(item[9].ToString());
                    totalVenta += double.Parse(item[10].ToString());
                    totalUtilidad += double.Parse(item[11].ToString());
                }
                txtTotalCosto.Text = "Total P. Costo: $us. " + totalCosto.ToString();
                txtTotalVenta.Text = "Total P. Venta: $us. " + totalVenta.ToString();
                txtTotalUtilidad.Text = "Total Utilidad: $us. " + totalUtilidad.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatSucursal()
        {
            try
            {
                implSucursal = new SucursalImpl();
                string sucursalGroupConcatID = implSucursal.SelectGroupConcatIDForComboBox();
                if (sucursalGroupConcatID != null)
                {
                    cbxSucursal.Items.Add(new ComboboxItem("TODOS", sucursalGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetSucursalFromDatabase()
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
                                            Valor = dr["idSucursal"].ToString(),
                                            Texto = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursal.Items.Add(item);
                }
                cbxSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatCategoria()
        {
            try
            {
                implCategoria = new CategoriaImpl();
                string categoriaGroupConcatID = implCategoria.SelectGroupConcatIDForComboBox();
                if (categoriaGroupConcatID != null)
                {
                    cbxCategoria.Items.Add(new ComboboxItem("TODOS", categoriaGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetCategoriaFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxCategoria = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implCategoria = new CategoriaImpl();
                dataTable = implCategoria.SelectForComboBox();
                listcomboboxCategoria = (from DataRow dr in dataTable.Rows
                                         select new ComboboxItem()
                                         {
                                             Valor = dr["idCategoria"].ToString(),
                                             Texto = dr["nombreCategoria"].ToString()
                                         }).ToList();
                foreach (var item in listcomboboxCategoria)
                {
                    cbxCategoria.Items.Add(item);
                }
                cbxCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetGroupConcatUsuarios()
        {
            try
            {
                implUsuario = new UsuarioImpl();
                string usuarioGroupConcatID = implUsuario.SelectGroupConcatIDForComboBox();
                if (usuarioGroupConcatID != null)
                {
                    cbxUsuario.Items.Add(new ComboboxItem("TODOS", usuarioGroupConcatID));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void cbxGetUsuarioFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxUsuario = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implUsuario = new UsuarioImpl();
                dataTable = implUsuario.SelectForComboBox();
                listcomboboxUsuario = (from DataRow dr in dataTable.Rows
                                       select new ComboboxItem()
                                       {
                                           Valor = dr["idUsuario"].ToString(),
                                           Texto = dr["nombreUsuario"].ToString()
                                       }).ToList();
                foreach (var item in listcomboboxUsuario)
                {
                    cbxUsuario.Items.Add(item);
                }
                cbxUsuario.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class ComboboxItem
        {
            public string Texto { get; set; }
            public string Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, string valor)
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
