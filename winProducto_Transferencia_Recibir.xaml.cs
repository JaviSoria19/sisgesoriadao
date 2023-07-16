using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;//ADO.NET
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Transferencia_Recibir.xaml
    /// </summary>
    public partial class winProducto_Transferencia_Recibir : Window
    {
        ProductoImpl implProducto;
        Producto producto;
        public winProducto_Transferencia_Recibir()
        {
            InitializeComponent();
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ConfirmByCode();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
        }
        private void txtCodigoSublote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmByCode();
            }
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dgvDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfirmByDataGridView();
        }
        private void Select()
        {
            try
            {
                implProducto = new ProductoImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implProducto.SelectPendingProducts().DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implProducto.SelectPendingProducts().Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ConfirmByCode()
        {
            if (string.IsNullOrEmpty(txtCodigoSublote.Text) != true)
            {
                try
                {
                    implProducto = new ProductoImpl();
                    producto = implProducto.GetByCode(txtCodigoSublote.Text);
                    if (producto != null)
                    {
                        if (producto.IdSucursal == Session.Sucursal_IdSucursal)
                        {
                            if (producto.Estado == 3)//P. En espera.
                            {
                                try
                                {
                                    int n = implProducto.UpdatePendingProduct(producto.IdProducto);
                                    if (n > 0)
                                    {
                                        txtCodigoSublote.Text = "";
                                        txtCodigoSublote.Focus();
                                        Select();
                                    }
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                                }
                            }
                            else if (producto.Estado == 0)//P. Eliminado
                            {
                                MessageBox.Show("Lo siento, este producto ha sido ELIMINADO y no está disponible para transferencia");
                            }
                            else if (producto.Estado == 2)//P. Vendido
                            {
                                MessageBox.Show("Lo siento, este producto ya ha sido VENDIDO y no está disponible para transferencia");
                            }
                            else if (producto.Estado == 1)//P. Disponible
                            {
                                MessageBox.Show("El producto seleccionado con el código " + producto.CodigoSublote + " está disponible en esta sucursal y no se encuentra en espera de transferencia.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Lo siento, el producto con el código " + producto.CodigoSublote + " se encuentra actualmente en otra sucursal y no está disponible para transferencia en este momento. Por favor, intente de nuevo más tarde o consulte con la sucursal correspondiente.\nSugerencia: Revise el historial del producto para saber la sucursal específica.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene el campo obligatorio. (*)");
            }
        }
        void ConfirmByDataGridView()
        {
            if (dgvDatos.SelectedItem != null && dgvDatos.Items.Count > 0)
            {
                try
                {
                    DataRowView d = (DataRowView)dgvDatos.SelectedItem;
                    int id = int.Parse(d.Row.ItemArray[0].ToString());
                    implProducto = new ProductoImpl();
                    producto = implProducto.Get(id);
                    if (producto != null)
                    {
                        if (MessageBox.Show("¿Confirma haber recibido el producto con código " + producto.CodigoSublote + "?", "Confirmar transferencia.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                int n = implProducto.UpdatePendingProduct(producto.IdProducto);
                                if (n > 0)
                                {
                                    txtCodigoSublote.Text = "";
                                    txtCodigoSublote.Focus();
                                    Select();
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }
        }
    }
}
