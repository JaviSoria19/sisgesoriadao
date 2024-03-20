﻿using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCaja_AdminRecepcion.xaml
    /// </summary>
    public partial class winCaja_AdminRecepcion : Window
    {
        CajaImpl implCaja;
        Caja caja;
        double cajastotalUSD = 0, cajastotalBOB = 0;
        public winCaja_AdminRecepcion()
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

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            implCaja = new CajaImpl();
            try
            {
                int n = implCaja.Update(caja);
                if (n > 0)
                {
                    MessageBox.Show("Confirmación de caja exitosa.");
                    Select();
                    dgvDetalles.ItemsSource = null;
                    btnConfirm.IsEnabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
            }
        }
        private void Select()
        {
            try
            {
                cajastotalUSD = 0;
                cajastotalBOB = 0;
                txtCajasTotalUSD.Text = "Total $us.: " + cajastotalUSD.ToString();
                txtCajasTotalBOB.Text = "Total Bs.: " + cajastotalBOB.ToString();

                implCaja = new CajaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCaja.Select().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCaja.Select().Rows.Count;
                if (dgvDatos.Items.Count > 0)
                {
                    foreach (DataRowView item in dgvDatos.Items)
                    {
                        cajastotalUSD += double.Parse(item[6].ToString());
                        cajastotalBOB += double.Parse(item[7].ToString());
                    }
                }
                else
                {
                    cajastotalUSD = 0;
                    cajastotalBOB = 0;
                }
                txtCajasTotalUSD.Text = "Total $us.: " + cajastotalUSD.ToString();
                txtCajasTotalBOB.Text = "Total Bs.: " + cajastotalBOB.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                btnConfirm.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
