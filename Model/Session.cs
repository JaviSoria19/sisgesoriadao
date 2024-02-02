using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace sisgesoriadao.Model
{
    public static class Session
    {
        //Cadena de conexión requerida para llamar a la base de datos.
        public static string CadenaConexionBdD { get; set; } = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
        public static string VersionApp { get; set; } = "v. 1.7.7";
        //Atributo indispensable para manejar la totalidad del sistema.
        public static byte IdUsuario { get; set; }
        //Atributo de referencia para dar a conocer al usuario que ha iniciado sesión correctamente.
        public static string NombreUsuario { get; set; }
        //Atributo requerido para los permisos del usuario
        public static byte Rol { get; set; }
        //Atributos requeridos meramente para realizar la(s) venta(s) y también para desplegarlo en la interfaz del usuario.
        public static byte Sucursal_IdSucursal { get; set; }
        public static string Sucursal_NombreSucursal { get; set; }
        public static string Sucursal_Direccion { get; set; }
        public static string Sucursal_Correo { get; set; }
        public static string Sucursal_Telefono { get; set; }
        //Atributos requeridos para regular el tipo de cambio y el límite de descuento de productos debajo del costo establecido.
        public static double Ajuste_Cambio_Dolar { get; set; }
        public static byte Ajuste_Limite_Descuento { get; set; }
        public static byte Caja_Operacion = 0;
        public static byte NumeroFormatoFecha { get; set; } = 2;
        public static byte IntervaloHora { get; set; } = 0;
        public static byte TemaPredeterminado { get; set; } = 0;
        public static int IdVentaDetalle { get; set; } = 0;
        public static int IdCliente { get; set; } = 0;
        public static int IdCaja { get; set; } = 0;
        public static int IdCotizacion { get; set; } = 0;
        public static string Producto_Historial_CodigoSublote { get; set; } = null;
        public static List<ProductoEnCola> Lista_CodigosDeProductosEnCola { get; set; } = new List<ProductoEnCola>();
        public static bool VerificarProductoEnCola(Producto producto, string operacion)
        {
            bool productoEnCola = false;
            /*Si el producto si está en cola, se devuelve verdadero.*/
            foreach (var item in Lista_CodigosDeProductosEnCola)
            {
                if (producto.CodigoSublote == item.CodigoSublote)
                {
                    productoEnCola = true;
                    break;
                }
            }
            /*Si el producto no está en cola, se añade a la lista.*/
            if (!productoEnCola)
            {
                Lista_CodigosDeProductosEnCola.Add(new ProductoEnCola
                {
                    CodigoSublote = producto.CodigoSublote,
                    NombreProducto = producto.NombreProducto,
                    Operacion = operacion
                });
            }
            return productoEnCola;
        }
        public static void RemoverProductoEnCola(string codigoSublote)
        {
            foreach (var item in Lista_CodigosDeProductosEnCola)
            {
                if (codigoSublote == item.CodigoSublote)
                {
                    Lista_CodigosDeProductosEnCola.Remove(item);
                    break;
                }
            }
        }
        public static void Mensaje_ProductoEnCola(Producto producto)
        {
            foreach (var item in Lista_CodigosDeProductosEnCola)
            {
                if (producto.CodigoSublote == item.CodigoSublote)
                {
                    MessageBox.Show("ATENCIÓN: EL PRODUCTO CON CÓDIGO " + item.CodigoSublote + " " + item.NombreProducto +
                    " NO SE PUEDE AGREGAR EN ESTE MOMENTO PORQUE SE ENCUENTRA EN COLA PARA: " + item.Operacion
                    , "PRODUCTO EN COLA", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        public static string FormatoFechaMySql(string MySqlAtributoFecha)
        {
            switch (NumeroFormatoFecha)
            {

                case 1:
                    /*DD/MM/AAAA*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%d/%m/%Y')";
                case 2:
                    /*DD/MM/AAAA HH:MM*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%d/%m/%Y %T')";
                case 3:
                    /*YYYY/MM/DD*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%Y/%m/%d')";
                case 4:
                    /*YYYY/MM/DD HH:MM*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%Y/%m/%d %T')";
                default:
                    return MySqlAtributoFecha;
            }
        }
        public static void ExportarAPortapapeles(DataGrid dataGrid)
        {
            string portapapeles = "";
            if (dataGrid.Items.Count > 0)
            {
                int columnasConTexto = 0;
                int columnaOculta = -1;
                /*ITERACION DE ENCABEZADOS*/
                foreach (var item in dataGrid.Columns)
                {
                    /*SI EL ENCABEZADO NO ESTÁ VACIO, SE COPIARÁ*/
                    if (item.Header != null)
                    {
                        /*SI LA COLUMNA NO ESTÁ OCULTA, SE COPIARÁ, CASO CONTRARIO SE GUARDARÁ EL ÍNDICE PARA NO COPIAR LOS DATOS DE ESA COLUMNA*/
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            portapapeles += item.Header.ToString().ToUpper() + ",";
                        }
                        else
                        {
                            columnaOculta = item.DisplayIndex;
                        }
                        columnasConTexto++;
                    }
                }
                /*REMUEVE EL ÚLTIMO CARACTER (COMA [,]) DE LA ITERACIÓN DE ENCABEZADOS*/
                portapapeles = portapapeles.Remove(portapapeles.Length - 1, 1);
                /*ITERACION DE DATOS DE LA TABLA*/
                foreach (DataRowView row in dataGrid.Items)
                {
                    portapapeles += "\n";
                    /*RECORRIDO DE CADA COLUMNA DE 1 SOLA FILA*/
                    for (int i = 0; i < columnasConTexto; i++)
                    {
                        /*SI LA COLUMNA ITERADA NO ES UN BOTÓN, CONTINUAR*/
                        if (row[i] != (row[i] as Button))
                        {
                            /*SI EL INDICE ITERADO NO ES IGUAL AL INDICE DE LA COLUMNA OCULTA, COPIAR*/
                            if (i != columnaOculta)
                            {
                                /*SI CONTIENE SALTOS DE LINEA, LIMPIARLOS Y COPIARLO, CASO CONTRARIO SOLO COPIARLO*/
                                if (row[i].ToString().Contains("\n"))
                                {
                                    portapapeles += row[i].ToString().Replace("\n", "") + ",";
                                }
                                else
                                {
                                    portapapeles += row[i].ToString() + ",";
                                }
                            }
                        }
                    }
                    portapapeles = portapapeles.Remove(portapapeles.Length - 1, 1);
                }
                Clipboard.SetText(portapapeles);
                MessageBox.Show("¡Se han copiado " + dataGrid.Items.Count + " registros al portapapeles!");
            }
            else
            {
                MessageBox.Show("¡Para exportar al portapapeles debe haber por lo menos 1 registro!");
            }
        }
        public static void ExportarAExcel(DataGrid dataGrid)
        {
            if (dataGrid.Items.Count > 0)
            {
                int columnasConTexto = 0;
                int columnaOculta = -1;
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Application.Workbooks.Add(true);
                int IndiceColumna = 0;
                /*ITERACION DE ENCABEZADOS*/
                foreach (var item in dataGrid.Columns)
                {
                    /*SI EL ENCABEZADO NO ESTÁ VACIO, SE EXPORTARÁ*/
                    if (item.Header != null)
                    {
                        /*SI LA COLUMNA NO ESTÁ OCULTA, SE EXPORTARÁ, CASO CONTRARIO SE GUARDARÁ EL ÍNDICE PARA NO EXPORTAR LOS DATOS DE ESA COLUMNA*/
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            IndiceColumna++;
                            excel.Cells[1, IndiceColumna] = item.Header.ToString().ToUpper();
                        }
                        else
                        {
                            columnaOculta = item.DisplayIndex;
                        }
                        columnasConTexto++;
                    }
                }
                int IndeceFila = 0;
                /*ITERACION DE DATOS DE LA TABLA*/
                foreach (DataRowView row in dataGrid.Items)
                {
                    IndeceFila++;
                    IndiceColumna = 0;
                    /*RECORRIDO DE CADA COLUMNA DE 1 SOLA FILA*/
                    for (int i = 0; i < columnasConTexto; i++)
                    {
                        /*SI LA COLUMNA ITERADA NO ES UN BOTÓN, CONTINUAR*/
                        if (row[i] != (row[i] as Button))
                        {
                            /*SI EL INDICE ITERADO NO ES IGUAL AL INDICE DE LA COLUMNA OCULTA, EXPORTAR*/
                            if (i != columnaOculta)
                            {
                                IndiceColumna++;
                                /*SI CONTIENE SALTOS DE LINEA, LIMPIARLOS Y EXPORTARLO, CASO CONTRARIO SOLO EXPORTARLO*/
                                if (row[i].ToString().Contains("\n"))
                                {
                                    excel.Cells[IndeceFila + 1, IndiceColumna] = row[i].ToString().Replace("\n", "") + ",";
                                }
                                else
                                {
                                    excel.Cells[IndeceFila + 1, IndiceColumna] = row[i].ToString();
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("¡Se ha exportado " + dataGrid.Items.Count + " registros a Excel!");
                /*DESPLEGAR AL USUARIO LA VENTANA DE EXCEL*/
                excel.Visible = true;
            }
            else
            {
                MessageBox.Show("¡Para exportar a Excel debe haber por lo menos 1 registro!");
            }
        }
        public static void ExportarAPDF(DataGrid dataGrid, string titulo)
        {
            if (dataGrid.Items.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog guardar = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = titulo + "_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm") + ".pdf",
                    Filter = "PDF(*.pdf)|*.pdf"
                };
                if (guardar.ShowDialog() == true)
                {
                    try
                    {
                        int columnasConTexto = 0;
                        int columnaOculta = -1;
                        /*ITERACION DE ENCABEZADOS*/
                        foreach (var item in dataGrid.Columns)
                        {
                            /*SI EL ENCABEZADO NO ESTÁ VACIO, SE COPIARÁ*/
                            if (item.Header != null)
                            {
                                if (item.Visibility != Visibility.Collapsed)
                                {
                                    columnasConTexto++;
                                }
                                else
                                {
                                    columnaOculta = item.DisplayIndex;
                                }
                            }
                        }
                        Document document = new Document(PageSize.A4, 25, 15, 15, 15);
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(guardar.FileName, FileMode.Create));
                        document.Open();
                        Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, 5);
                        Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA, 5, Font.BOLD);
                        PdfPTable table = new PdfPTable(columnasConTexto);
                        Array floatArray = Array.CreateInstance(typeof(float), columnasConTexto);
                        //float[] widths = new float[] { };
                        for (int i = 0; i < columnasConTexto; i++)
                            floatArray.SetValue(4f, i);
                        table.SetWidths((float[])floatArray);
                        table.WidthPercentage = 100;
                        PdfPCell cell = new PdfPCell(new Phrase("Products"))
                        {
                            Colspan = columnasConTexto
                        };

                        /*ITERACION DE ENCABEZADOS*/
                        foreach (var item in dataGrid.Columns)
                        {
                            /*SI EL ENCABEZADO NO ESTÁ VACIO, SE COPIARÁ*/
                            if (item.Header != null)
                            {
                                /*SI LA COLUMNA NO ESTÁ OCULTA, SE COPIARÁ, CASO CONTRARIO SE GUARDARÁ EL ÍNDICE PARA NO COPIAR LOS DATOS DE ESA COLUMNA*/
                                if (item.Visibility != Visibility.Collapsed)
                                {
                                    table.AddCell(new Phrase(item.Header.ToString().ToUpper(), boldFont));
                                }
                            }
                        }
                        if (columnaOculta != -1)
                        {
                            columnasConTexto++;
                        }
                        /*ITERACION DE DATOS DE LA TABLA*/
                        foreach (DataRowView row in dataGrid.Items)
                        {
                            /*RECORRIDO DE CADA COLUMNA DE 1 SOLA FILA*/
                            for (int i = 0; i < columnasConTexto; i++)
                            {
                                /*SI LA COLUMNA ITERADA NO ES UN BOTÓN, CONTINUAR*/
                                if (row[i] != (row[i] as Button))
                                {
                                    /*SI EL INDICE ITERADO NO ES IGUAL AL INDICE DE LA COLUMNA OCULTA, COPIAR*/
                                    if (i != columnaOculta)
                                    {
                                        table.AddCell(new Phrase(row[i].ToString(), font5));
                                    }
                                }
                            }
                        }
                        document.Add(table);
                        document.Close();
                        MessageBox.Show("¡Se ha exportado " + dataGrid.Items.Count + " registros a PDF!");
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("¡Para exportar a PDF debe haber por lo menos 1 registro!");
            }
        }
    }
}
