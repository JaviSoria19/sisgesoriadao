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
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCaja_Registros.xaml
    /// </summary>
    public partial class winCaja_Registros : Window
    {
        CajaImpl implCaja;
        Caja caja;
        public winCaja_Registros()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }

        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void Select()
        {
            try
            {
                implCaja = new CajaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCaja.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCaja.Select().Rows.Count;
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
                    int id = int.Parse(d.Row.ItemArray[0].ToString());
                    caja = implCaja.Get(id);
                    if (caja != null)
                    {
                        SelectDetalles(caja);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        void SelectDetalles(Caja caja)
        {
            try
            {
                dgvDetalles.ItemsSource = null;
                dgvDetalles.ItemsSource = implCaja.SelectDetails(caja).DefaultView;
                //dgvDetalles.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRowsDetalles.Content = "NÚMERO DE REGISTROS: " + implCaja.SelectDetails(caja).Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
