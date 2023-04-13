﻿using System;
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
using MySql.Data.MySqlClient;//MySql.Data

using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winMainAdmin.xaml
    /// </summary>
    public partial class winMainAdmin : Window
    {
        public winMainAdmin()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = "Bienvenid@ a " + Session.Sucursal_NombreSucursal + " , " + Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está segur@ de cerrar la sesión actual?", "CERRAR SESIÓN", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                winLogin winLogin = new winLogin();
                winLogin.Show();
                this.Close();
            }
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            winAjuste winAjuste = new winAjuste();
            winAjuste.Show();
        }
        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            winEmpleado winEmpleado = new winEmpleado();
            winEmpleado.Show();
        }
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            winUsuario winUsuario = new winUsuario();
            winUsuario.Show();
        }
        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            winVenta_Insert winVenta_Insert = new winVenta_Insert();
            winVenta_Insert.Show();
        }
        private void btnCustomers_Click(object sender, RoutedEventArgs e)
        {
            winCliente winCliente = new winCliente();
            winCliente.Show();
        }
        private void btnBranches_Click(object sender, RoutedEventArgs e)
        {
            winSucursal winSucursal = new winSucursal();
            winSucursal.Show();
        }
        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto winProducto = new winProducto();
            winProducto.Show();
        }
        private void btnAddProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Insert winProducto_Insert = new winProducto_Insert();
            winProducto_Insert.Show();
        }
        private void btnBatches_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Lote winProducto_Lote = new winProducto_Lote();
            winProducto_Lote.Show();
        }
        private void btnHistoryProducts_Click(object sender, RoutedEventArgs e)
        {
            winProducto_Historial winProducto_Historial = new winProducto_Historial();
            winProducto_Historial.Show();
        }
        private void btnCategories_Click(object sender, RoutedEventArgs e)
        {
            winCategoria winCategoria = new winCategoria();
            winCategoria.Show();
        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
