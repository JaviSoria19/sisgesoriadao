using MessagingToolkit.QRCode.Codec;
using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winVenta_Detalle.xaml
    /// </summary>
    public partial class winVenta_Detalle : Window
    {
        VentaImpl implVenta;
        string clipboardTexto = "";
        int idVenta = 0;

        public winVenta_Detalle()
        {
            InitializeComponent();
        }

        // ════════════════════════════════════════════════════════════
        //  CARGA DE VENTANA
        // ════════════════════════════════════════════════════════════

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            SelectDetalle();
            idVenta = Session.IdVentaDetalle;
            if (Session.Rol != 1)
            {
                btnModifySale.IsEnabled = false;
            }
        }

        // ════════════════════════════════════════════════════════════
        //  BOTONES EXISTENTES
        // ════════════════════════════════════════════════════════════

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPrintPDF.IsEnabled = false;
                btnPrint.IsEnabled = false;
                System.Windows.FrameworkElement fe = ZonaImpresionGrid as System.Windows.FrameworkElement;
                if (fe == null) return;

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    Transform originalScale = fe.LayoutTransform;
                    System.Printing.PrintCapabilities capabilities =
                        pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                    double scale = Math.Min(
                        capabilities.PageImageableArea.ExtentWidth / fe.ActualWidth,
                        capabilities.PageImageableArea.ExtentHeight / fe.ActualHeight);

                    fe.LayoutTransform = new ScaleTransform(scale, scale);

                    System.Windows.Size sz = new System.Windows.Size(
                        capabilities.PageImageableArea.ExtentWidth,
                        capabilities.PageImageableArea.ExtentHeight);

                    fe.Measure(sz);
                    fe.Arrange(new System.Windows.Rect(
                        new System.Windows.Point(
                            capabilities.PageImageableArea.OriginWidth,
                            capabilities.PageImageableArea.OriginHeight), sz));

                    pd.PrintVisual(ZonaImpresionGrid, "My Print");
                    fe.LayoutTransform = originalScale;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                btnPrintPDF.IsEnabled = true;
                btnPrint.IsEnabled = true;
                Focus();
            }
        }

        private void btnPrintPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPrintPDF.IsEnabled = false;
                btnPrint.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(ZonaImpresionGrid, "RECIBO");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                btnPrintPDF.IsEnabled = true;
                btnPrint.IsEnabled = true;
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("¡Se ha copiado la descripción de los Productos e IMEI's o S/N en el portapapeles!");
            Clipboard.SetText(clipboardTexto);
        }

        private void btnModifySale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Session.IdVentaDetalle = idVenta;
                winVenta_Update winVenta_Update = new winVenta_Update();
                winVenta_Update.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        // ════════════════════════════════════════════════════════════
        //  NUEVO — BOTÓN IMPRIMIR ROLLO
        // ════════════════════════════════════════════════════════════

        private void btnPrintRollo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnPrintRollo.IsEnabled = false;

                // Ancho en píxeles del rollo:
                //   58 mm → 211 px  |  80 mm → 302 px  (a 96 dpi)
                // Cambia este valor según el ancho de tu impresora.
                const double TICKET_WIDTH = 302;

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() != true) return;

                // Configurar el ticket de impresión para rollo
                var pt = pd.PrintTicket;
                pt.PageMediaSize = new System.Printing.PageMediaSize(
                    System.Printing.PageMediaSizeName.Unknown,
                    TICKET_WIDTH,
                    2800    // alto generoso; la térmica corta al terminar el contenido
                );
                pt.PageOrientation = System.Printing.PageOrientation.Portrait;
                pd.PrintTicket = pt;

                // Construir y medir el visual del ticket
                StackPanel rollo = ConstruirTicketRollo(TICKET_WIDTH);

                // Envolver en un Border que actúa como contenedor estricto
                Border contenedor = new Border
                {
                    Width = TICKET_WIDTH,
                    Child = rollo,
                    ClipToBounds = true,   // corta cualquier cosa que se desborde
                    Background = System.Windows.Media.Brushes.White,
                    Padding = new Thickness(0)
                };

                contenedor.Measure(new System.Windows.Size(TICKET_WIDTH, double.PositiveInfinity));
                contenedor.Arrange(new System.Windows.Rect(
                    new System.Windows.Size(TICKET_WIDTH, contenedor.DesiredSize.Height)));
                contenedor.UpdateLayout();

                pd.PrintVisual(contenedor, "Ticket Rollo - " + txtIdVenta.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir en rollo: " + ex.Message);
            }
            finally
            {
                btnPrintRollo.IsEnabled = true;
            }
        }

        // ════════════════════════════════════════════════════════════
        //  NUEVO — CONSTRUCTOR DEL TICKET EN ROLLO
        // ════════════════════════════════════════════════════════════

        private StackPanel ConstruirTicketRollo(double ticketWidth)
        {
            const double MARGIN = 16;
            const double FS = 11;
            const double FS_SM = 9.5;
            const double FS_LG = 14;

            var ticket = new StackPanel
            {
                Width = ticketWidth - (MARGIN * 2), // ancho real descontando márgenes
                MaxWidth = ticketWidth - (MARGIN * 2),
                Orientation = Orientation.Vertical,
                Margin = new Thickness(MARGIN),
                Background = System.Windows.Media.Brushes.White
            };

            // ── ENCABEZADO ───────────────────────────────────────────
            ticket.Children.Add(TB(txtSucursal_nombre.Text, FS_LG, bold: true, center: true));
            ticket.Children.Add(TB(txtSucursal_direccion.Text, FS_SM, center: true));
            ticket.Children.Add(TB(txtSucursal_telefono.Text, FS_SM, center: true));
            ticket.Children.Add(TB(txtSucursal_correo.Text, FS_SM, center: true));
            ticket.Children.Add(Sep());

            // ── TIPO DE DOCUMENTO Y NÚMERO ───────────────────────────
            ticket.Children.Add(TB(txtTitulo.Text, FS_LG, bold: true, center: true));
            ticket.Children.Add(TB(txtIdVenta.Text, FS, bold: true, center: true));
            ticket.Children.Add(TB(txtVenta_fecha.Text, FS_SM));
            ticket.Children.Add(Sep());

            // ── DATOS DEL CLIENTE ────────────────────────────────────
            ticket.Children.Add(TB(txtCliente_nombre.Text, FS_SM, bold: true));
            ticket.Children.Add(TB(txtCliente_celular.Text, FS_SM));
            ticket.Children.Add(TB(txtCliente_ci.Text, FS_SM));
            ticket.Children.Add(Sep());

            // ── CABECERA TABLA DE PRODUCTOS ──────────────────────────
            ticket.Children.Add(CabeceraProductos(FS_SM));
            ticket.Children.Add(LineaDivisora());

            // ── ITEMS ────────────────────────────────────────────────
            // Leemos los mismos TextBlock ya cargados por SelectDetalle()
            // y los dividimos línea por línea.
            string[] descripciones = txtProducto_Descripcion.Text.Split('\n');
            string[] detalles = txtProducto_Detalle.Text.Split('\n');
            string[] garantias = txtProducto_Garantia.Text.Split('\n');
            string[] cantidades = txtProducto_Cantidad.Text.Split('\n');
            string[] precios = txtProducto_Precio.Text.Split('\n');
            string[] totales = txtProducto_TotalBOB.Text.Split('\n');

            // Filtramos líneas que son solo espacios (los "\n \n" generan blancos)
            var descFilt = Array.FindAll(descripciones, s => s.Trim().Length > 1);
            var detFilt = Array.FindAll(detalles, s => s.Trim().Length > 0);
            var garFilt = Array.FindAll(garantias, s => s.Trim().Length > 0);
            var cantFilt = Array.FindAll(cantidades, s => s.Trim().Length > 0);
            var precFilt = Array.FindAll(precios, s => s.Trim().Length > 0);
            var totFilt = Array.FindAll(totales, s => s.Trim().Length > 0);

            int count = Math.Min(descFilt.Length, cantFilt.Length);
            for (int i = 0; i < count; i++)
            {
                ticket.Children.Add(FilaProducto(
                    desc: descFilt[i].Trim(),
                    detalle: i < detFilt.Length ? detFilt[i].Trim() : "",
                    garantia: i < garFilt.Length ? garFilt[i].Trim() : "",
                    cant: cantFilt[i].Trim(),
                    /*precio: i < precFilt.Length ? precFilt[i].Trim() : "",*/
                    total: i < totFilt.Length ? totFilt[i].Trim() : "",
                    fs: FS_SM
                ));
            }

            ticket.Children.Add(Sep());

            // ── TOTALES ──────────────────────────────────────────────
            ticket.Children.Add(FilaDoble("TOTAL:", txtVenta_Total.Text, FS_LG, bold: true));
            ticket.Children.Add(FilaDoble("Adelanto:", txtVenta_Adelanto.Text, FS));
            ticket.Children.Add(FilaDoble("Saldo:", txtVenta_Saldo.Text, FS));
            ticket.Children.Add(Sep());

            // ── PAGOS ────────────────────────────────────────────────
            ticket.Children.Add(TB(txtSubtituloPagos.Text, FS, bold: true));
            ticket.Children.Add(TB(txtPagos.Text, FS_SM));
            ticket.Children.Add(Sep());

            // ── OBSERVACIONES ────────────────────────────────────────
            ticket.Children.Add(TB(txtObservaciones.Text, FS_SM, wrap: true));
            ticket.Children.Add(Sep());

            // ── QR (solo en venta minorista) ─────────────────────────
            if (imgQR.Visibility == Visibility.Visible && imgQR.Source != null)
            {
                var qr = new System.Windows.Controls.Image
                {
                    Source = imgQR.Source,
                    Width = 90,
                    Height = 90,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 4)
                };
                ticket.Children.Add(qr);
            }

            // ── PIE ──────────────────────────────────────────────────
            ticket.Children.Add(TB("¡GRACIAS POR SU PREFERENCIA!", FS, bold: true, center: true));
            ticket.Children.Add(TB("La garantía de los celulares y tablet's es de 1 año.", FS_SM, bold: false, center: true));
            ticket.Children.Add(new Border { Height = 28 }); // espacio antes del corte

            return ticket;
        }

        // ════════════════════════════════════════════════════════════
        //  HELPERS VISUALES PARA EL TICKET
        // ════════════════════════════════════════════════════════════

        /// <summary>TextBlock configurable.</summary>
        private System.Windows.Controls.TextBlock TB(
            string texto,
            double fs,
            bool bold = false,
            bool center = false,
            bool wrap = true)
        {
            return new System.Windows.Controls.TextBlock
            {
                Text = texto,
                FontSize = fs,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                TextAlignment = center ? TextAlignment.Center : TextAlignment.Left,
                HorizontalAlignment = center
                    ? HorizontalAlignment.Center
                    : HorizontalAlignment.Left,
                TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
                Margin = new Thickness(0, 1, 0, 1)
            };
        }

        /// <summary>Fila con etiqueta a la izquierda y valor a la derecha.</summary>
        private Grid FilaDoble(string izq, string der, double fs, bool bold = false)
        {
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(1, GridUnitType.Star) });

            var fw = bold ? FontWeights.Bold : FontWeights.Normal;

            var tbIzq = new System.Windows.Controls.TextBlock
            {
                Text = izq,
                FontSize = fs,
                FontWeight = fw,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var tbDer = new System.Windows.Controls.TextBlock
            {
                Text = der,
                FontSize = fs,
                FontWeight = fw,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(tbIzq, 0);
            Grid.SetColumn(tbDer, 1);
            g.Children.Add(tbIzq);
            g.Children.Add(tbDer);
            g.Margin = new Thickness(0, 1, 0, 1);
            return g;
        }

        /// <summary>Encabezado de la tabla de productos.</summary>
        private Grid CabeceraProductos(double fs)
        {
            var g = new Grid();
            // Desc | Detalle | Garantia | Cant | Precio | Total
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            string[] cols = { "Descripción", "Detalle", /*"Garantía", "Cant.", "Precio",*/ "Total" };
            HorizontalAlignment[] aligns =
            {
                HorizontalAlignment.Left,   HorizontalAlignment.Left,
                /*HorizontalAlignment.Center, HorizontalAlignment.Center,
                HorizontalAlignment.Right,*/ HorizontalAlignment.Right
            };

            for (int i = 0; i < cols.Length; i++)
            {
                var tb = new System.Windows.Controls.TextBlock
                {
                    Text = cols[i],
                    FontSize = fs,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = aligns[i]
                };
                Grid.SetColumn(tb, i);
                g.Children.Add(tb);
            }
            return g;
        }

        /// <summary>Fila de un producto en el ticket.</summary>
        private Grid FilaProducto(
            string desc, string detalle, string garantia,
            string cant, /*string precio,*/ string total,
            double fs)
        {
            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.5, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.7, GridUnitType.Star) });
            //g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.3, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.Margin = new Thickness(1, 1, 1, 2);

            var tbDesc = new System.Windows.Controls.TextBlock
            {
                Text = desc,
                FontSize = fs,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var tbDet = new System.Windows.Controls.TextBlock
            {
                Text = detalle,
                FontSize = fs,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            /*var tbGar = new System.Windows.Controls.TextBlock
            {
                Text = garantia,
                FontSize = fs,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var tbCant = new System.Windows.Controls.TextBlock
            {
                Text = cant,
                FontSize = fs,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var tbPrec = new System.Windows.Controls.TextBlock
            {
                Text = precio,
                FontSize = fs,
                HorizontalAlignment = HorizontalAlignment.Right
            };*/
            var tbTot = new System.Windows.Controls.TextBlock
            {
                Text = total,
                FontSize = fs,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(tbDesc, 0); Grid.SetColumn(tbDet, 1);
            /*Grid.SetColumn(tbGar, 2); Grid.SetColumn(tbCant, 3);
            Grid.SetColumn(tbPrec, 4);*/ Grid.SetColumn(tbTot, 2);

            g.Children.Add(tbDesc); g.Children.Add(tbDet);
            /*g.Children.Add(tbGar); g.Children.Add(tbCant);
            g.Children.Add(tbPrec);*/
            g.Children.Add(tbTot);
            return g;
        }

        /// <summary>Línea separadora sólida negra.</summary>
        private Border Sep() => new Border
        {
            BorderThickness = new Thickness(0, 1, 0, 0),
            BorderBrush = System.Windows.Media.Brushes.Black,
            Margin = new Thickness(0, 4, 0, 4)
        };

        /// <summary>Línea divisora delgada gris (entre cabecera e ítems).</summary>
        private Border LineaDivisora() => new Border
        {
            BorderThickness = new Thickness(0, 1, 0, 0),
            BorderBrush = System.Windows.Media.Brushes.Gray,
            Margin = new Thickness(0, 2, 0, 2)
        };

        // ════════════════════════════════════════════════════════════
        //  CARGA DE DATOS
        // ════════════════════════════════════════════════════════════

        void SelectDetalle()
        {
            if (Session.IdVentaDetalle == 0) return;

            try
            {
                implVenta = new VentaImpl();

                DataTable dtProductos = implVenta.SelectSaleDetails1();
                DataTable dtPagos = implVenta.SelectSaleDetails2();

                if (dtProductos.Rows.Count == 0) return;

                DataRow cabecera = dtProductos.Rows[0];
                bool esVentaMayor = byte.Parse(cabecera["esVentaPorMayor"].ToString()) == 1;

                CargarEncabezado(cabecera, esVentaMayor);
                CargarProductos(dtProductos, esVentaMayor);
                CargarPagos(dtPagos, esVentaMayor);
                GenerarQR(clipboardTexto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ────────────────────────────────────────────────────────────

        private void CargarEncabezado(DataRow cabecera, bool esVentaMayor)
        {
            int idVentaLocal = int.Parse(cabecera["ID"].ToString());

            // Modo venta por mayor: ajustar UI
            if (esVentaMayor)
            {
                txtTitulo.Text = "NOTA DE VENTA";
                txtTitulo.Foreground = new SolidColorBrush(System.Windows.Media.Colors.DarkBlue);
                thTotal.Text = "TOTAL $.";
                txtSubtituloPagos.Text = "PAGOS ($.)";

                shapeFirmaResponsable.Visibility = Visibility.Hidden;
                txtFirmaResponsable.Visibility = Visibility.Hidden;
                shapeFirmaCliente.Visibility = Visibility.Hidden;
                txtFirmaCliente.Visibility = Visibility.Hidden;
                txtDisclaimer.Visibility = Visibility.Hidden;
                txtThanks.Visibility = Visibility.Hidden;
                imgQR.Visibility = Visibility.Hidden;
            }

            // Datos generales
            txtIdVenta.Text = "Nro.: " + idVentaLocal.ToString("D5");
            txtSucursal_nombre.Text = cabecera["Sucursal"].ToString();
            txtSucursal_direccion.Text = cabecera["Direccion"].ToString();
            txtSucursal_telefono.Text = cabecera["Telefono"].ToString();
            txtSucursal_correo.Text = cabecera["Correo"].ToString();

            txtCliente_nombre.Text = "Cliente: " + cabecera["Cliente"].ToString();
            txtCliente_celular.Text = "Telefono/Celular: " + cabecera["Celular"].ToString();
            txtCliente_ci.Text = "C.I.: " + cabecera["CI"].ToString();

            txtObservaciones.Text = "Observaciones: " + cabecera["Observaciones"].ToString()
                + " | Ejecutivo de ventas: " + cabecera["Usuario"].ToString()
                + " - Cel.: " + cabecera["CelularUsuario"].ToString();

            txtVenta_fecha.Text = "Fecha: " + cabecera["Fecha"].ToString();

            // Totales según moneda
            double total = double.Parse(cabecera[esVentaMayor ? "TotalUSD" : "TotalBOB"].ToString());
            double saldo = double.Parse(cabecera[esVentaMayor ? "SaldoUSD" : "SaldoBOB"].ToString());
            double adelanto = Math.Round(total - saldo, 2);
            string moneda = esVentaMayor ? "$." : "Bs.";

            txtVenta_Total.Text = $"{total} {moneda}";
            txtVenta_Adelanto.Text = $"{adelanto} {moneda}";
            txtVenta_Saldo.Text = $"{saldo} {moneda}";
        }

        // ────────────────────────────────────────────────────────────

        private void CargarProductos(DataTable dtProductos, bool esVentaMayor)
        {
            txtProducto_Descripcion.Text = "";
            txtProducto_Detalle.Text = "";
            txtProducto_Garantia.Text = "";
            txtProducto_Cantidad.Text = "";
            txtProducto_Precio.Text = "";
            txtProducto_DescuentoPorcentaje.Text = "";
            txtProducto_DescuentoBOB.Text = "";
            txtProducto_TotalBOB.Text = "";
            clipboardTexto = "";

            foreach (DataRow fila in dtProductos.Rows)
            {
                string descripcion = fila["Producto"].ToString();
                string detalle = fila["Detalle"].ToString();
                string garantia = esVentaMayor ? "N/A" : fila["Garantia"].ToString() + " Meses";
                string totalFila = esVentaMayor ? fila["precioUSD"].ToString() : fila["TotalProducto"].ToString();

                // Descripción larga no necesita línea extra
                txtProducto_Descripcion.Text += descripcion.Length > 45 ? descripcion + "\n" : descripcion + "\n \n";
                txtProducto_Detalle.Text += detalle + "\n \n";
                txtProducto_Garantia.Text += garantia + "\n \n";
                txtProducto_Cantidad.Text += fila["Cantidad"].ToString() + "\n \n";
                txtProducto_Precio.Text += fila["Precio"].ToString() + "\n \n";
                txtProducto_DescuentoPorcentaje.Text += fila["DescuentoPorcentaje"].ToString() + "\n \n";
                txtProducto_DescuentoBOB.Text += fila["DescuentoBs"].ToString() + "\n \n";
                txtProducto_TotalBOB.Text += totalFila + "\n \n";

                clipboardTexto += $"{descripcion} {detalle}\n";
            }

            clipboardTexto = clipboardTexto.Trim();
        }

        // ────────────────────────────────────────────────────────────

        private void CargarPagos(DataTable dtPagos, bool esVentaMayor)
        {
            if (dtPagos.Rows.Count == 0)
            {
                txtPagos.Text = "-";
                return;
            }

            txtPagos.Text = "";
            foreach (DataRow pago in dtPagos.Rows)
            {
                string monto = esVentaMayor ? pago["MontoUSD"].ToString() : pago["MontoBOB"].ToString();
                txtPagos.Text += $"{pago["Fecha"]}    {monto}\n";
            }
        }

        // ────────────────────────────────────────────────────────────

        private void GenerarQR(string contenido)
        {
            QRCodeEncoder encoder = new QRCodeEncoder { QRCodeScale = 8 };
            Bitmap bitmap = encoder.Encode(contenido);

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                imgQR.Source = bitmapImage;
            }
        }
    }
}
