﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;//ADO.NET
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winProducto_Transferencia.xaml
    /// </summary>
    public partial class winProducto_Transferencia : Window
    {
        ProductoImpl implProducto;
        Producto producto;
        List<Producto> listaProductos = new List<Producto>();
        SucursalImpl implSucursal;
        int contador = 1;
        int pdf_contador = 1;
        public winProducto_Transferencia()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (listaProductos.Count > 0)
            {
                if (MessageBox.Show("¿Está seguro de haber ingresado todos los datos correctamente? \n Cantidad de productos ingresados al lote: " + listaProductos.Count + ". \n Presione SI para continuar.", "Confirmar lote", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    implProducto = new ProductoImpl();
                    string mensaje = implProducto.UpdateBranchMovementTransaction(listaProductos, (cbxSucursal.SelectedItem as ComboboxItem).Valor, cbxSucursal.Text);
                    if (mensaje == "PRODUCTOS TRANSFERIDOS EXITOSAMENTE.")
                    {
                        MessageBox.Show(mensaje);
                        try
                        {
                            int idTransferencia = implProducto.GetLastMovementFromBranch();
                            if (idTransferencia != 0)
                            {
                                DataTable dt = new DataTable();
                                dt = implProducto.SelectMovementsHistory_Details(idTransferencia);
                                pdf(dt);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(mensaje);
                    }
                }
            }
            else
            {
                MessageBox.Show("¡Debe ingresar como mínimo 1 producto al lote!");
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            GetProductoFromDB();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            cbxGetSucursalFromDatabase();
        }
        private void txtCodigoSublote_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodigoSublote.Text) != true)
            {
                if (e.Key == Key.Enter)
                {
                    GetProductoFromDB();
                }
            }
        }
        private void dgvProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeFromDataGridandList(dgvProductos.SelectedIndex);
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
                Header = "ID CONDICION",
                Binding = new Binding("IdCondicion")
            };
            DataGridTextColumn columna5 = new DataGridTextColumn
            {
                Header = "ID USUARIO",
                Binding = new Binding("IdUsuario")
            };
            DataGridTextColumn columna6 = new DataGridTextColumn
            {
                Header = "Codigo",
                Binding = new Binding("CodigoSublote")
            };
            DataGridTextColumn columna7 = new DataGridTextColumn
            {
                Header = "Producto",
                Binding = new Binding("NombreProducto")
            };
            DataGridTextColumn columna8 = new DataGridTextColumn
            {
                Header = "SN/IMEI",
                Binding = new Binding("Identificador")
            };
            DataGridTextColumn columna9 = new DataGridTextColumn
            {
                Header = "C. $.",
                Binding = new Binding("CostoUSD")
            };
            DataGridTextColumn columna10 = new DataGridTextColumn
            {
                Header = "C. Bs.",
                Binding = new Binding("CostoBOB")
            };
            DataGridTextColumn columna11 = new DataGridTextColumn
            {
                Header = "P. $.",
                Binding = new Binding("PrecioVentaUSD")
            };
            DataGridTextColumn columna12 = new DataGridTextColumn
            {
                Header = "P. Bs.",
                Binding = new Binding("PrecioVentaBOB")
            };
            DataGridTextColumn columna13 = new DataGridTextColumn
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
            dgvProductos.Columns.Add(columna13);

            dgvProductos.Columns[0].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[1].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[2].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[3].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[4].Visibility = Visibility.Collapsed;

            dgvProductos.Columns[8].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[9].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[10].Visibility = Visibility.Collapsed;
            dgvProductos.Columns[11].Visibility = Visibility.Collapsed;
        }
        void GetProductoFromDB()
        {
            if (string.IsNullOrEmpty(txtCodigoSublote.Text) != true)
            {
                if (cbxSucursal.SelectedItem.ToString() != Session.Sucursal_NombreSucursal)
                {
                    try
                    {
                        implProducto = new ProductoImpl();
                        producto = implProducto.GetByCode(txtCodigoSublote.Text);
                        if (producto != null)
                        {
                            if (producto.IdSucursal == Session.Sucursal_IdSucursal)
                            {
                                if (producto.Estado == 1)
                                {
                                    addToDataGrid_andList(producto);
                                }
                                else if (producto.Estado == 0)//P. Eliminado
                                {
                                    MessageBox.Show("Lo siento, este producto ha sido ELIMINADO y no está disponible para transferencia");
                                }
                                else if (producto.Estado == 2)//P. Vendido
                                {
                                    MessageBox.Show("Lo siento, este producto ya ha sido VENDIDO y no está disponible para transferencia");
                                }
                                else if (producto.Estado == 3)//P. En espera
                                {
                                    MessageBox.Show("Este producto está actualmente en espera de ser aceptado en otra sucursal, por favor espere hasta que esté disponible de nuevo.");
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
                    MessageBox.Show("¡No puede seleccionar la misma sucursal en la que usted se encuentra!");
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
            }
        }
        void addToDataGrid_andList(Producto producto)
        {
            bool validoParaInsercion = true;
            for (int i = 0; i < listaProductos.Count; i++)
            {
                if (producto.CodigoSublote == listaProductos[i].CodigoSublote)
                {
                    MessageBox.Show("¡El producto ingresado ya se encuentra en la tabla!");
                    validoParaInsercion = false;
                    break;
                }
            }
            if (validoParaInsercion == true)
            {
                dgvProductos.Items.Add(producto);
                listaProductos.Add(producto);
                txtCodigoSublote.Text = "";
                txtCodigoSublote.Focus();
                contador++;
                if (contador != 1)
                {
                    cbxSucursal.IsEnabled = false;
                }
            }
        }
        void removeFromDataGridandList(int posicion)
        {
            if (dgvProductos.SelectedItem != null && dgvProductos.Items.Count > 0)
            {
                if (dgvProductos.Items.IsEmpty != true && listaProductos != null)
                {
                    if (MessageBox.Show("Está realmente segur@ de remover este producto de la lista?", "Remover", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        dgvProductos.Items.RemoveAt(posicion);
                        listaProductos.RemoveAt(posicion);
                        contador--;
                        if (contador == 1)
                        {
                            cbxSucursal.IsEnabled = true;
                        }
                    }
                }
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
                                            Valor = Convert.ToByte(dr["idSucursal"]),
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
        void pdf(DataTable dataTable)
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Transferencia_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            string paginahtml_texto = Properties.Resources.PlantillaReporteTransferencia.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@NOMBRESUCURSAL", Session.Sucursal_NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAREGISTRO", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHAIMPRESION", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", Session.NombreUsuario);
            string filas = string.Empty;
            foreach (DataRow item in dataTable.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + pdf_contador + "</td>";
                filas += "<td>" + item[1].ToString() + "</td>";
                filas += "<td>" + item[2].ToString() + "</td>";
                filas += "<td>" + item[3].ToString() + "</td>";
                filas += "<td>" + item[4].ToString() + "</td>";
                filas += "<td>" + item[5].ToString() + "</td>";
                filas += "</tr>";
                pdf_contador++;
            }
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);


            if (guardar.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(guardar.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Phrase(""));
                    using (StringReader sr = new StringReader(paginahtml_texto))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    pdfDoc.Close();
                    stream.Close();
                }
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
    }
}
