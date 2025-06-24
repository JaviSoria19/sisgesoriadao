using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Sublotes.xaml
    /// </summary>
    public partial class winProducto_Sublotes : Window
    {
        ProductoImpl implProducto;
        public winProducto_Sublotes()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }

        public class ComboboxItem
        {
            public string Texto { get; set; }
            public int Valor { get; set; }

            public override string ToString()
            {
                return Texto;
            }
            public ComboboxItem(string texto, int valor)
            {
                Texto = texto;
                Valor = valor;
            }
            public ComboboxItem()
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetNombreProovedorFromDatabase();
        }

        void cbxGetNombreProovedorFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxNombreProveedor = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implProducto = new ProductoImpl();
                dataTable = implProducto.SelectProviderNamesForComboBox();
                listcomboboxNombreProveedor = (from DataRow dr in dataTable.Rows
                                               select new ComboboxItem()
                                               {
                                                   Texto = dr["nombreProveedor"].ToString()
                                               }).ToList();
                acbtxtNombreProveedor.ItemsSource = listcomboboxNombreProveedor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btndgvEliminarUltimoPago_Click(object sender, RoutedEventArgs e)
        {
            DataRowView d = (DataRowView)dgvDatos.SelectedItem;
            int id = int.Parse(d.Row.ItemArray[0].ToString());
            string pagos = d.Row.ItemArray[4].ToString();
            if(pagos == "-")
            {
                MessageBox.Show("No se puede eliminar el último pago, ya que no existe un pago registrado para este sublote.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                dgvDatos.SelectedItem = null;
                txtPagoUSD.IsEnabled = false;
                btnAddPayment.IsEnabled = false;
                labelClear(lblSeleccion);
                txtNombreProveedor.IsEnabled = false;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                return;
            }
            if(MessageBox.Show("Está realmente segur@ de eliminar el último pago de sublote?", "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    implProducto = new ProductoImpl();
                    if (implProducto.DeleteLastPaymentSubBatch(new PagoSublote(id, 0, DateTime.Now)) > 0)
                    {
                        MessageBox.Show("Se ha eliminado el último pago del sublote correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        dgvDatos.SelectedItem = null;
                        txtPagoUSD.IsEnabled = false;
                        btnAddPayment.IsEnabled = false;
                        labelClear(lblSeleccion);
                        txtNombreProveedor.IsEnabled = false;
                        btnSave.IsEnabled = false;
                        btnCancel.IsEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SelectSublotes(acbtxtNombreProveedor.Text, 0.01);
            }
        }

        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            SelectSublotes("", 0.01);
        }
        private void SelectSublotes(string nombreProveedor, double saldo)
        {
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectSubBatchPendings(nombreProveedor, saldo).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "Registros: " + dgvDatos.Items.Count;

                labelClear(lblSeleccion);
                txtPagoUSD.IsEnabled = false;
                btnAddPayment.IsEnabled = false;
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
                DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                int id = int.Parse(d.Row.ItemArray[0].ToString());
                string sublote = d.Row.ItemArray[1].ToString();
                string nombreProveedor = d.Row.ItemArray[2].ToString();
                lblSeleccion.Content = "Se ha seleccionado el Sublote " + sublote;
                labelSuccess(lblSeleccion);
                txtPagoUSD.IsEnabled = true;
                btnAddPayment.IsEnabled = true;
                txtNombreProveedor.Text = nombreProveedor;
                txtNombreProveedor.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
        }

        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void txtPagoUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InsertPayment();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }

        void InsertPayment()
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text))
            {
                MessageBox.Show("Por favor, ingrese un monto de pago.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (dgvDatos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un sublote para registrar el pago.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                implProducto = new ProductoImpl();
                DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                int id = int.Parse(d.Row.ItemArray[0].ToString());
                double pagoUSD = double.Parse(txtPagoUSD.Text);
                PagoSublote pagoSublote = new PagoSublote(id, pagoUSD, DateTime.Now);
                
                if (implProducto.InsertPaymentSubBatch(pagoSublote) > 0)
                {
                    MessageBox.Show("Pago registrado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    labelSuccess(lblSeleccion);
                    txtPagoUSD.Text = "";
                    SelectSublotes(acbtxtNombreProveedor.Text, 0.01);
                    dgvDatos.SelectedItem = null;
                    txtPagoUSD.IsEnabled = false;
                    btnAddPayment.IsEnabled = false;
                    txtNombreProveedor.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void btnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            InsertPayment();
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
            Session.ExportarAPDF(dgvDatos, "HISTORIAL_DE_PAGOS_CLIENTE");
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectSublotes(acbtxtNombreProveedor.Text, 0);
        }

        private void acbtxtNombreProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectSublotes(acbtxtNombreProveedor.Text, 0);
                txtPagoUSD.IsEnabled = false;
                btnAddPayment.IsEnabled = false;
            }
            if (e.Key == Key.Escape)
            {
                (sender as AutoCompleteBox).Text = "";
            }
        }

        public void labelSuccess(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.Background = new SolidColorBrush(Colors.LimeGreen);
        }

        public void labelClear(Label label)
        {
            label.Foreground = new SolidColorBrush(Colors.Transparent);
            label.Background = new SolidColorBrush(Colors.Transparent);
            label.Content = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateProviderSubBatch();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            dgvDatos.SelectedItem = null;
            btnSave.IsEnabled = false;
            txtNombreProveedor.IsEnabled = false;
            btnCancel.IsEnabled = false;
            lblSeleccion.Content = "";
            labelClear(lblSeleccion);
            txtPagoUSD.IsEnabled = false;
            btnAddPayment.IsEnabled = false;
        }

        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        private void txtNombreProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                UpdateProviderSubBatch();
            }
            if (e.Key == Key.Escape)
            {
                txtNombreProveedor.Text = "";
            }
        }

        void UpdateProviderSubBatch()
        {
            if (dgvDatos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un sublote para guardar los cambios.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtNombreProveedor.Text))
            {
                MessageBox.Show("Por favor, ingrese un nombre de proveedor.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                implProducto = new ProductoImpl();
                DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                int idSublote = int.Parse(d.Row.ItemArray[0].ToString());
                string nombreProveedor = txtNombreProveedor.Text.Trim();

                if (implProducto.UpdateProviderSubBatch(nombreProveedor, idSublote) > 0)
                {
                    MessageBox.Show("Nombre de proveedor actualizado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    btnSave.IsEnabled = false;
                    txtNombreProveedor.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    SelectSublotes(acbtxtNombreProveedor.Text, 0.01);
                    txtNombreProveedor.Text = "";
                    cbxGetNombreProovedorFromDatabase();

                    dgvDatos.SelectedItem = null;
                    txtPagoUSD.IsEnabled = false;
                    btnAddPayment.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
