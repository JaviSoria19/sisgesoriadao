using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para winProducto_Insert.xaml
    /// </summary>
    public partial class winProducto_Insert : Window
    {
        CategoriaImpl implCategoria;
        ProductoImpl implProducto;
        List<Producto> listaproductos = new List<Producto>();
        int contador=1;
        string codigoSublote;
        int idSublote = 0;
        
        public winProducto_Insert()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            txtCambioDolar.Text = Session.Ajuste_Cambio_Dolar.ToString();
            cbxGetCategoriaFromDatabase();
            GetCodigoSubLoteFromDatabase();
            GetIDSubLoteFromDatabase();
            txtSucursal.Text = "Sucursal: " + Session.Sucursal_NombreSucursal;
            txtObservaciones.Text = "-";
        }
        private void dgvProductos_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn columna1 = new DataGridTextColumn
            {
                Header = "ID SUCURSAL",
                Binding = new Binding("IdSucursal")
            };
            DataGridTextColumn columna2 = new DataGridTextColumn
            {
                Header = "ID CATEGORIA",
                Binding = new Binding("IdCategoria")
            };
            DataGridTextColumn columna3 = new DataGridTextColumn
            {
                Header = "ID SUBLOTE",
                Binding = new Binding("IdSublote")
            };
            DataGridTextColumn columna4 = new DataGridTextColumn
            {
                Header = "ID USUARIO",
                Binding = new Binding("IdUsuario")
            };
            DataGridTextColumn columna5 = new DataGridTextColumn
            {
                Header = "Codigo",
                Binding = new Binding("CodigoSublote")
            };
            DataGridTextColumn columna6 = new DataGridTextColumn
            {
                Header = "Producto",
                Binding = new Binding("NombreProducto")
            };
            DataGridTextColumn columna7 = new DataGridTextColumn
            {
                Header = "SN/IMEI",
                Binding = new Binding("Identificador")
            };
            DataGridTextColumn columna8 = new DataGridTextColumn
            {
                Header = "C. $.",
                Binding = new Binding("CostoUSD")
            };
            DataGridTextColumn columna9 = new DataGridTextColumn
            {
                Header = "C. Bs.",
                Binding = new Binding("CostoBOB")
            };
            DataGridTextColumn columna10 = new DataGridTextColumn
            {
                Header = "P. $.",
                Binding = new Binding("PrecioVentaUSD")
            };
            DataGridTextColumn columna11 = new DataGridTextColumn
            {
                Header = "P. Bs.",
                Binding = new Binding("PrecioVentaBOB")
            };
            DataGridTextColumn columna12 = new DataGridTextColumn
            {
                Header = "Obs.",
                Binding = new Binding("Observaciones")
            };
            dgvProductos.Columns.Add(columna1);
            dgvProductos.Columns.Add(columna2);
            dgvProductos.Columns.Add(columna3);
            dgvProductos.Columns.Add(columna4);
            dgvProductos.Columns.Add(columna5);
            dgvProductos.Columns.Add(columna6);
            dgvProductos.Columns.Add(columna7);
            dgvProductos.Columns.Add(columna8);
            dgvProductos.Columns.Add(columna9);
            dgvProductos.Columns.Add(columna10);
            dgvProductos.Columns.Add(columna11);
            dgvProductos.Columns.Add(columna12);

            dgvProductos.Columns[0].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[1].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[2].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[3].Visibility = Visibility.Collapsed;
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            winProducto winProducto = new winProducto();
            winProducto.Show();
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addToDataGrid_andList();
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dgvProductos != null && listaproductos != null)
            {
                dgvProductos.Items.RemoveAt(contador-2);
                listaproductos.RemoveAt(contador-2);
                contador--;
                txtCodigoSublote.Text = codigoSublote + "-" + contador;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            implProducto = new ProductoImpl();
            string mensaje = implProducto.InsertTransaction(listaproductos,1/*<---REMOVER ESTE 1*/);
            if (mensaje == "LA VENTA SE REGISTRÓ CON ÉXITO.")
            {
                MessageBox.Show(mensaje + "\n IMPRIMIENDO LAS ETIQUETAS...");
                //insertar código para imprimir etiquetas DYMO
            }
            else
            {
                MessageBox.Show(mensaje);
            }
        }
        private void txtCostoUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCostoUSD.Text)!=true)
            {
                double costoBOB = Math.Round(double.Parse(txtCostoUSD.Text) * Session.Ajuste_Cambio_Dolar,2);
                txtCostoBOB.Text = costoBOB.ToString();
            }
        }
        private void txtCostoBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCostoBOB.Text) != true)
            {
                double costoUSD = Math.Round(double.Parse(txtCostoBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);                
                txtCostoUSD.Text = costoUSD.ToString();
            }
        }
        private void txtPrecioUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrecioUSD.Text) != true)
            {
                double precioBOB = Math.Round(double.Parse(txtPrecioUSD.Text) * Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioBOB.Text = precioBOB.ToString();
            }
        }
        private void txtPrecioBOB_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrecioBOB.Text) != true)
            {
                double precioUSD = Math.Round(double.Parse(txtPrecioBOB.Text) / Session.Ajuste_Cambio_Dolar, 2);
                txtPrecioUSD.Text = precioUSD.ToString();
            }
        }
        private void txtIdentificador_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addToDataGrid_andList();
            }
        }
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        void addToDataGrid_andList()
        {
            if (string.IsNullOrEmpty(txtNombreProducto.Text) != true && string.IsNullOrEmpty(txtIdentificador.Text) != true &&
                string.IsNullOrEmpty(txtCostoUSD.Text) != true && string.IsNullOrEmpty(txtCostoBOB.Text) != true &&
                string.IsNullOrEmpty(txtPrecioUSD.Text) != true && string.IsNullOrEmpty(txtPrecioBOB.Text) != true)
            {
                if (double.Parse(txtPrecioUSD.Text) > double.Parse(txtCostoUSD.Text))
                {
                    dgvProductos.Items.Add(new Producto
                    {
                        IdSucursal = Session.Sucursal_IdSucursal,
                        IdCategoria = byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString()),
                        IdSublote = idSublote,
                        IdUsuario = Session.IdUsuario,
                        CodigoSublote = txtCodigoSublote.Text,
                        NombreProducto = txtNombreProducto.Text,
                        Identificador = txtIdentificador.Text,
                        CostoUSD = double.Parse(txtCostoUSD.Text),
                        CostoBOB = double.Parse(txtCostoBOB.Text),
                        PrecioVentaUSD = double.Parse(txtPrecioUSD.Text),
                        PrecioVentaBOB = double.Parse(txtPrecioBOB.Text),
                        Observaciones = txtObservaciones.Text
                    });

                    listaproductos.Add(new Producto(
                        Session.Sucursal_IdSucursal,
                        byte.Parse((cbxCategoria.SelectedItem as ComboboxItem).Valor.ToString()),
                        idSublote,
                        Session.IdUsuario,
                        txtCodigoSublote.Text,
                        txtNombreProducto.Text,
                        txtIdentificador.Text,
                        double.Parse(txtCostoUSD.Text),
                        double.Parse(txtCostoBOB.Text),
                        double.Parse(txtPrecioUSD.Text),
                        double.Parse(txtPrecioBOB.Text),
                        txtObservaciones.Text
                        ));

                    contador++;
                    txtCodigoSublote.Text = codigoSublote + "-" + contador;
                    txtIdentificador.Text = "";
                    txtIdentificador.Focus();
                }
                else
                {
                    MessageBox.Show("¡El precio de venta no puede ser menor al costo del producto!");
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
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
                                             Valor = Convert.ToByte(dr["idCategoria"]),
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
        void GetCodigoSubLoteFromDatabase()
        {
            try
            {
                implProducto = new ProductoImpl();
                codigoSublote = implProducto.GetCodeFormatToInsertProducts();
                txtCodigoSublote.Text = codigoSublote + "-" + contador;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GetIDSubLoteFromDatabase()
        {
            try
            {
                implProducto = new ProductoImpl();
                idSublote = implProducto.GetSubBatchToInsertProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UN PUNTO.)<---------
        private static readonly Regex _regex = new Regex("[^0-9,-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
    }
}
