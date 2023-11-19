using iTextSharp.text;
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
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winCaja_Registros.xaml
    /// </summary>
    public partial class winCaja_Registros : Window
    {
        CajaImpl implCaja;
        Caja caja;
        UsuarioImpl implUsuario;
        Usuario usuario;
        SucursalImpl implSucursal;
        Sucursal sucursal;
        double cajastotalUSD = 0, cajastotalBOB = 0;
        public winCaja_Registros()
        {
            InitializeComponent();
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                implUsuario = new UsuarioImpl();
                usuario = implUsuario.Get(caja.IdUsuario);
                if (usuario != null)
                {
                    implSucursal = new SucursalImpl();
                    sucursal = implSucursal.Get(caja.IdSucursal);
                    if (sucursal != null)
                    {
                        pdf();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            addCbxTipo();
            cbxGetUsuarioFromDatabase();
        }
        private void dgvDatos_Loaded(object sender, RoutedEventArgs e)
        {
            Select();
        }
        private void dtpFechaFin_Loaded(object sender, RoutedEventArgs e)
        {
            dtpFechaFin.SelectedDate = DateTime.Today;
            dtpFechaInicio.SelectedDate = new DateTime(2023, 01, 01);
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
        private void Select()
        {
            try
            {
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
                btnPrintPDF.IsEnabled = true;
                btnPrint.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void pdf()
        {
            Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog();
            guardar.FileName = "Caja_" + caja.FechaRegistro.ToString("yyyy_MM_dd__HH_mm") + ".pdf";
            guardar.Filter = "PDF(*.pdf)|*.pdf";

            DateTime fechaCierreConvertida = new DateTime();
            fechaCierreConvertida = DateTime.Parse(caja.FechaActualizacion);

            string paginahtml_texto = Properties.Resources.PlantillaReporteCaja.ToString();
            paginahtml_texto = paginahtml_texto.Replace("@USUARIO", usuario.NombreUsuario);
            paginahtml_texto = paginahtml_texto.Replace("@SUCURSAL", sucursal.NombreSucursal);
            paginahtml_texto = paginahtml_texto.Replace("@FECHAAPERTURA", caja.FechaRegistro.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHACIERRE", fechaCierreConvertida.ToString("dd/MM/yyyy HH:mm"));
            paginahtml_texto = paginahtml_texto.Replace("@FECHASISTEMA", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            string filas = string.Empty;
            double totalUSD = 0;
            double totalBOB = 0;

            double efectivoUSD = 0, efectivoBOB = 0, transferenciaUSD = 0, transferenciaBOB = 0, tarjetaUSD = 0, tarjetaBOB = 0;
            foreach (DataRowView item in dgvDetalles.Items)
            {
                filas += "<tr>";
                filas += "<td>" + item[0].ToString() + "</td>";//usuario
                filas += "<td>" + item[5].ToString() + "</td>";//fecha
                filas += "<td>" + item[1].ToString() + "</td>";//detalle
                filas += "<td>" + item[2].ToString() + "</td>";//tipo
                filas += "<td>" + item[3].ToString() + "</td>";//usd
                filas += "<td>" + item[4].ToString() + "</td>";//bob
                filas += "</tr>";
                totalUSD += double.Parse(item[3].ToString());
                totalBOB += double.Parse(item[4].ToString());
                if (item[2].ToString() == "Efectivo")
                {
                    efectivoUSD += double.Parse(item[3].ToString());
                    efectivoBOB += double.Parse(item[4].ToString());
                }
                else if (item[2].ToString() == "Transferencia")
                {
                    transferenciaUSD += double.Parse(item[3].ToString());
                    transferenciaBOB += double.Parse(item[4].ToString());
                }
                else
                {
                    tarjetaUSD += double.Parse(item[3].ToString());
                    tarjetaBOB += double.Parse(item[4].ToString());
                }
            }
            paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas);
            paginahtml_texto = paginahtml_texto.Replace("@TOTALUSD", "$us. " + totalUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TOTALBOB", "Bs. " + totalBOB.ToString());

            paginahtml_texto = paginahtml_texto.Replace("@EFECTIVO_TOTALUSD", "$us. " + efectivoUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@EFECTIVO_TOTALBOB", "Bs. " + efectivoBOB.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TRANSFERENCIA_TOTALUSD", "$us. " + transferenciaUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TRANSFERENCIA_TOTALBOB", "Bs. " + transferenciaBOB.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TARJETA_TOTALUSD", "$us. " + tarjetaUSD.ToString());
            paginahtml_texto = paginahtml_texto.Replace("@TARJETA_TOTALBOB", "Bs. " + tarjetaBOB.ToString());


            if (guardar.ShowDialog() == true)
            {
                try
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void addCbxTipo()
        {
            cbxTipoCaja.Items.Add(new ComboboxItem("Todos", 0));
            cbxTipoCaja.Items.Add(new ComboboxItem("Cerrados", 2));
            cbxTipoCaja.Items.Add(new ComboboxItem("Recepcionados", 3));
            cbxTipoCaja.SelectedIndex = 0;
        }
        private void cbxGetUsuarioFromDatabase()
        {
            try
            {
                cbxUsuario.Items.Add(new ComboboxItem("Todos", 0));
                List<ComboboxItem> listcomboboxUsuario = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implUsuario = new UsuarioImpl();
                dataTable = implUsuario.SelectForComboBox();
                listcomboboxUsuario = (from DataRow dr in dataTable.Rows
                                       select new ComboboxItem()
                                       {
                                           Texto = dr["nombreUsuario"].ToString(),
                                           Valor = Convert.ToByte(dr["idUsuario"].ToString())
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
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cbxTipoCaja.SelectedIndex == 0 && cbxUsuario.SelectedIndex == 0)
            {//TODAS LAS CAJAS (2,3) Y TODOS LOS USUARIOS
                string cadenaCajas = "";
                for (int i = 1; i < cbxTipoCaja.Items.Count; i++)
                {
                    cbxTipoCaja.SelectedIndex = i;
                    cadenaCajas += (cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString() + ",";
                }
                cadenaCajas = cadenaCajas.Remove(cadenaCajas.Length - 1);
                cbxTipoCaja.SelectedIndex = 0;
                //--------------------------------------------------
                string cadenaIdUsuarios = "";
                for (int i = 1; i < cbxUsuario.Items.Count; i++)
                {
                    cbxUsuario.SelectedIndex = i;
                    cadenaIdUsuarios += (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString() + ",";
                }
                cadenaIdUsuarios = cadenaIdUsuarios.Remove(cadenaIdUsuarios.Length - 1);
                cbxUsuario.SelectedIndex = 0;
                //--------------------------------------------------
                SelectLike(cadenaCajas, cadenaIdUsuarios);
                //MessageBox.Show("Todas las cajas y todos los usuarios:" + cadenaCajas + "|" + cadenaIdUsuarios);
            }
            else if (cbxTipoCaja.SelectedIndex == 0 && cbxUsuario.SelectedIndex != 0)
            {//TODAS LAS CAJAS PERO CON UN USUARIO ESPECÍFICO
                string cadenaCajas = "";
                for (int i = 1; i < cbxTipoCaja.Items.Count; i++)
                {
                    cbxTipoCaja.SelectedIndex = i;
                    cadenaCajas += (cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString() + ",";
                }
                cadenaCajas = cadenaCajas.Remove(cadenaCajas.Length - 1);
                cbxTipoCaja.SelectedIndex = 0;
                //--------------------------------------------------
                SelectLike(cadenaCajas, (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString());
                //MessageBox.Show("Todas las cajas y un usuario específico:" + cadenaCajas + "|" + (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString());
            }
            else if (cbxTipoCaja.SelectedIndex != 0 && cbxUsuario.SelectedIndex == 0)
            {//CAJA ESPECÍFICA PERO CON TODOS LOS USUARIOS
                string cadenaIdUsuarios = "";
                for (int i = 1; i < cbxUsuario.Items.Count; i++)
                {
                    cbxUsuario.SelectedIndex = i;
                    cadenaIdUsuarios += (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString() + ",";
                }
                cadenaIdUsuarios = cadenaIdUsuarios.Remove(cadenaIdUsuarios.Length - 1);
                cbxUsuario.SelectedIndex = 0;
                //--------------------------------------------------
                SelectLike((cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString(), cadenaIdUsuarios);
                //MessageBox.Show("Caja específica y todos los usuarios:" + (cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString() + "|" + cadenaIdUsuarios);
            }
            else
            {//CAJA ESPECÍFICA Y USUARIO ESPECÍFICO
                SelectLike((cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString(), (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString());
                //MessageBox.Show("Caja específica y un usuario específic:" + (cbxTipoCaja.SelectedItem as ComboboxItem).Valor.ToString() + "|" + (cbxUsuario.SelectedItem as ComboboxItem).Valor.ToString());
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgvDatos.Items.IsEmpty != true)
                {
                    Session.IdCaja = caja.IdCaja;
                    Session.Caja_Operacion = 3;
                    winCaja_Detalle winCaja_Detalle = new winCaja_Detalle();
                    winCaja_Detalle.Show();
                }
                else
                {
                    MessageBox.Show("¡No puede imprimir el reporte de la caja actual si ésta encuentra vacía!");
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
        private void btnCopy2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPortapapeles(dgvDetalles);
        }
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDatos);
        }
        private void btnExcel2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAExcel(dgvDetalles);
        }
        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPDF(dgvDatos, "REGISTROS_CAJAS");
        }

        private void btnPDF2_Click(object sender, RoutedEventArgs e)
        {
            Session.ExportarAPDF(dgvDetalles, "CAJA_DETALLE");
        }

        private void SelectLike(string tipoCajas, string idUsuarios)
        {
            try
            {
                implCaja = new CajaImpl();
                dgvDatos.ItemsSource = null;
                dgvDatos.ItemsSource = implCaja.SelectLikeByCashTypeAndUsers(tipoCajas, idUsuarios, dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).DefaultView;
                dgvDatos.Columns[0].Visibility = Visibility.Collapsed;
                lblDataGridRows.Content = "NÚMERO DE REGISTROS: " + implCaja.SelectLikeByCashTypeAndUsers(tipoCajas, idUsuarios, dtpFechaInicio.SelectedDate.Value.Date, dtpFechaFin.SelectedDate.Value.Date).Rows.Count;
                cajastotalUSD = 0;
                cajastotalBOB = 0;
                if (dgvDatos.Items.Count > 0)
                {
                    foreach (DataRowView item in dgvDatos.Items)
                    {
                        cajastotalUSD += double.Parse(item[6].ToString());
                        cajastotalBOB += double.Parse(item[7].ToString());
                    }
                }
                txtCajasTotalUSD.Text = "Total $us.: " + cajastotalUSD.ToString();
                txtCajasTotalBOB.Text = "Total Bs.: " + cajastotalBOB.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
