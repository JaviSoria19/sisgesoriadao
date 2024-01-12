using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_DeudasClientes.xaml
    /// </summary>
    public partial class winVenta_DeudasClientes : Window
    {
        VentaImpl implVenta;
        double SaldoUSD = 0, SaldoBOB = 0;
        public winVenta_DeudasClientes()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxPaymentMethod.Items.Add(new ComboboxItem("EFECTIVO", 1));
            cbxPaymentMethod.Items.Add(new ComboboxItem("TRANSFERENCIA BANCARIA", 2));
            cbxPaymentMethod.Items.Add(new ComboboxItem("TARJETA", 3));
            cbxPaymentMethod.SelectedIndex = 0;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            SelectVentasConSaldoPendiente();
        }
        private void SelectVentasConSaldoPendiente()
        {
            try
            {
                implVenta = new VentaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implVenta.SelectAllSalesWithPendingBalanceByCustomers().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "Registros: " + dgvDatos.Items.Count;

                SaldoUSD = 0;
                SaldoBOB = 0;
                foreach (DataRowView item in dgvDatos.Items)
                {
                    SaldoUSD += double.Parse(item[3].ToString());
                    SaldoBOB += double.Parse(item[4].ToString());
                }
                txtTotalSaldoUSD.Text = "Total Saldo $.: " + SaldoUSD;
                txtTotalSaldoBOB.Text = "Total Saldo Bs.: " + SaldoBOB;

                if (dgvDatos.Items.Count == 0)
                {
                    Close();
                }
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
            Session.ExportarAPDF(dgvDatos, "SALDOS_PENDIENTES");
        }

        private void btndgvModificar(object sender, RoutedEventArgs e)
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
                    throw;
                }
            }
        }
        private void btndgvImprimir(object sender, RoutedEventArgs e)
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    Session.IdVentaDetalle = int.Parse(d.Row.ItemArray[0].ToString());
                    winVenta_Detalle winVenta_Detalle = new winVenta_Detalle();
                    winVenta_Detalle.Show();
                    dgvDatos.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
        private void btnAddPaymentMethod_Click(object sender, RoutedEventArgs e)
        {
            addPaymentMethods();
        }
        void addPaymentMethods()
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                double pagoUSD = 0, pagoBOB = 0;
                byte metodoPago;
                pagoUSD = double.Parse(txtPagoUSD.Text.ToString().Trim());
                pagoBOB = double.Parse(txtPagoBOB.Text.ToString().Trim());
                metodoPago = byte.Parse((cbxPaymentMethod.SelectedItem as ComboboxItem).Valor.ToString());
                if (pagoUSD <= SaldoUSD || pagoBOB <= SaldoBOB)
                {
                    if (MessageBox.Show("Está a punto de saldar una o más ventas con saldo pendiente. ¿Está seguro de que desea continuar con el pago?", "REGISTRAR PAGOS Y ACTUALIZAR VENTAS", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        List<int> listaIDVentas = new List<int>();
                        List<double> listaSaldosUSD = new List<double>();
                        foreach (DataRowView row in dgvDatos.Items)
                        {
                            listaIDVentas.Add(int.Parse(row[0].ToString()));
                            listaSaldosUSD.Add(double.Parse(row[3].ToString()));
                        }
                        string mensaje = "";
                        int i = 0;
                        while (pagoUSD > 0)
                        {
                            if (pagoUSD >= listaSaldosUSD[i])
                            {
                                string insert = implVenta.InsertPaymentMethodTransaction(listaIDVentas[i], pagoUSD, Math.Round(pagoUSD * Session.Ajuste_Cambio_Dolar, 2), metodoPago);
                                if (insert == "INSERTMETODOPAGO_EXITOSO")
                                {
                                    mensaje += "¡Pago EXITOSO en la venta #" + listaIDVentas[i] + "!\n";
                                }
                                else
                                {
                                    mensaje += insert + "\n";
                                }
                            }
                            else
                            {
                                string insert = implVenta.InsertPaymentMethodTransaction(listaIDVentas[i], pagoUSD, Math.Round(pagoUSD * Session.Ajuste_Cambio_Dolar, 2), metodoPago);
                                if (insert == "INSERTMETODOPAGO_EXITOSO")
                                {
                                    mensaje += "Se pagó PARCIALMENTE la venta #" + listaIDVentas[i] + "\n";
                                }
                                else
                                {
                                    mensaje += insert + "\n";
                                }
                            }
                            pagoUSD -= listaSaldosUSD[i];
                            pagoUSD = Math.Round(pagoUSD, 2);
                            i++;
                        }
                        MessageBox.Show(mensaje, "RESULTADO DE LA OPERACIÓN", MessageBoxButton.OK, MessageBoxImage.Information);
                        SelectVentasConSaldoPendiente();
                    }
                }
                else
                {
                    MessageBox.Show("ATENCIÓN, LOS MONTOS INGRESADOS NO SON VÁLIDOS PORQUE SUPERAN EL SALDO TOTAL DE LA(S) VENTA(S) \n" +
                        "MONTOS INGRESADOS: " + pagoUSD + " $. | " + pagoBOB + " Bs.\n" +
                        "SALDO TOTAL:" + SaldoUSD + " $. | " + SaldoBOB + " Bs.", "RESULTADO DE LA OPERACIÓN", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los montos para realizar el(los) pago(s).");
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
        private void txtPagoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                double costoBOB = Math.Round(double.Parse(txtPagoUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtPagoBOB.Text = costoBOB.ToString();
            }
        }
        private void txtPagoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtPagoBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtPagoUSD.Text = costoUSD.ToString();
            }
        }

        private void txtPagoBOB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoBOB.Text) != true)
            {
                addPaymentMethods();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
            }
        }
        private void txtPagoUSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && string.IsNullOrEmpty(txtPagoUSD.Text) != true)
            {
                addPaymentMethods();
            }
            if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = "";
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
