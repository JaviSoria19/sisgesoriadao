using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_Cantidad.xaml
    /// </summary>
    public partial class winVenta_Cantidad : Window
    {
        VentaImpl implVenta;
        SucursalImpl implSucursal;
        CategoriaImpl implCategoria;
        UsuarioImpl implUsuario;
        public winVenta_Cantidad()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
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
            txtBuscar_Producto_o_Codigo.Focus();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectLike();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Now;
            dtpFechaInicio.SelectedDate = DateTime.Now;
        }
        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            CleanText();
        }
        private void txtBuscar_Producto_o_Codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectLike();
            }
            if (e.Key == Key.Escape)
            {
                CleanText();
            }
        }
        void CleanText()
        {
            txtBuscar_Producto_o_Codigo.Text = "";
            txtBuscar_Producto_o_Codigo.Focus();
        }
        void SelectLike()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectLikeReporteVentasGlobalesCantidad(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor, txtBuscar_Producto_o_Codigo.Text.Trim()).DefaultView;
                //dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "REGISTROS ENCONTRADOS: " + implVenta.SelectLikeReporteVentasGlobalesCantidad(dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date, (cbxSucursal.SelectedItem as ComboboxItem).Valor, (cbxCategoria.SelectedItem as ComboboxItem).Valor, (cbxUsuario.SelectedItem as ComboboxItem).Valor, txtBuscar_Producto_o_Codigo.Text.Trim()).Rows.Count;
                int totalCantidad = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    totalCantidad += int.Parse(item[4].ToString());
                }
                txtTotalCantidad.Text = "TOTAL DE PRODUCTOS VENDIDOS: " + totalCantidad;
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
            Session.ExportarAPDF(dgvDatos, "VENTAS_CANTIDADES");
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
