using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class VentaImpl : DataBase, IVenta
    {
        public string InsertTransaction(Venta Venta, List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<byte> ListaGarantias, List<MetodoPago> ListaMetodosPago, Cliente Cliente)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {
                //REGISTRO DE LA VENTA.
                command.CommandText = @"INSERT INTO Venta (idCliente,idUsuario,idSucursal,totalUSD,totalBOB,saldoUSD,saldoBOB,observaciones) 
                            VALUES(@idCliente,@idUsuario,@idSucursal,@totalUSD,@totalBOB,@saldoUSD,@saldoBOB,@observaciones)";
                command.Parameters.AddWithValue("@idCliente", Venta.IdCliente);
                command.Parameters.AddWithValue("@idUsuario", Venta.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", Venta.IdSucursal);
                command.Parameters.AddWithValue("@totalUSD", Venta.TotalUSD);
                command.Parameters.AddWithValue("@totalBOB", Venta.TotalBOB);
                command.Parameters.AddWithValue("@saldoUSD", Venta.SaldoUSD);
                command.Parameters.AddWithValue("@saldoBOB", Venta.SaldoBOB);
                command.Parameters.AddWithValue("@observaciones", Venta.Observaciones);
                command.ExecuteNonQuery();
                //REGISTRO DE LOS PRODUCTOS, ACTUALIZACIÓN DEL ESTADO DE LOS PRODUCTOS E INSERCIÓN EN HISTORIAL.
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    command.CommandText = @"INSERT INTO Detalle_Venta (idVenta,idProducto,cantidad,precioUSD,precioBOB,descuento,garantia)
                            VALUES((SELECT MAX(idVenta) FROM Venta),@idProducto,@cantidad,@precioUSD,@precioBOB,@descuento,@garantia)";
                    command.Parameters.AddWithValue("@idProducto", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@cantidad", 1);
                    command.Parameters.AddWithValue("@precioUSD", ListaProductos[i].PrecioVentaUSD);
                    command.Parameters.AddWithValue("@precioBOB", ListaProductos[i].PrecioVentaBOB);
                    command.Parameters.AddWithValue("@descuento", ListaDescuentosPorcentaje[i]);
                    command.Parameters.AddWithValue("@garantia", ListaGarantias[i]);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE Producto SET estado = 2, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProductoTwice";
                    command.Parameters.AddWithValue("@idProductoTwice", ListaProductos[i].IdProducto);
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO Historial (idProducto,detalle) VALUES
                                (@idProductoThree,@detalle)";
                    command.Parameters.AddWithValue("@idProductoThree", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@detalle", "PRODUCTO VENDIDO POR EL USUARIO: " + Session.NombreUsuario + " AL CLIENTE: " + Cliente.Nombre);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
                if (ListaMetodosPago.Count > 0)
                {
                    foreach (var item in ListaMetodosPago)
                    {
                        //REGISTRO DEL DETALLE DE VENTA.
                        command.CommandText = @"INSERT INTO Metodo_Pago (idVenta,montoUSD,montoBOB,tipo)
                            VALUES((SELECT MAX(idVenta) FROM Venta WHERE idSucursal = @Session_idSucursal),@montoUSD,@montoBOB,@tipo)";
                        command.Parameters.AddWithValue("@Session_idSucursal", Session.Sucursal_IdSucursal);
                        command.Parameters.AddWithValue("@montoUSD", item.MontoUSD);
                        command.Parameters.AddWithValue("@montoBOB", item.MontoBOB);
                        command.Parameters.AddWithValue("@tipo", item.Tipo);
                        command.ExecuteNonQuery();
                        //REGISTRO DEL DETALLE DE CAJA.
                        command.CommandText = @"INSERT INTO Detalle_Caja (idCaja,idMetodoPago)
                            VALUES((SELECT MAX(idCaja) FROM Caja WHERE idSucursal = @Twice_Session_idSucursal),LAST_INSERT_ID())";
                        command.Parameters.AddWithValue("@Twice_Session_idSucursal", Session.Sucursal_IdSucursal);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
                myTrans.Commit();
                return "VENTA_EXITOSA";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public int Update(Venta o)
        {
            throw new NotImplementedException();
        }
        public int Delete(Venta o)
        {
            throw new NotImplementedException();
        }

        public int Insert(Venta o)
        {
            throw new NotImplementedException();
        }
        public DataTable Select()
        {
            throw new NotImplementedException();
        }
        public DataTable SelectLike(string CadenaBusqueda, DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }

        public string GetTodaySales(DateTime FechaHoy)
        {
            string numeroVentasdelDia = null;
            string query = @"SELECT COUNT(idVenta) FROM Venta WHERE idSucursal = @SessionIdSucursal AND estado = 1
                                AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@FechaInicio", FechaHoy.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", FechaHoy.ToString("yyyy-MM-dd") + " 23:59:59");
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    numeroVentasdelDia = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return numeroVentasdelDia;
        }

        public (double, double) GetCashAmounts()
        {
            double CajaUSD = 0, CajaBOB = 0;
            string query = @"SELECT IFNULL(SUM(MP.montoUSD),0), IFNULL(SUM(MP.montoBOB),0) FROM Caja C
                                INNER JOIN Detalle_Caja DC ON C.idCaja = DC.idCaja
                                INNER JOIN Metodo_Pago MP ON DC.idMetodoPago = MP.idMetodoPago
                                INNER JOIN Venta V ON MP.idVenta = V.idVenta
                                WHERE C.idSucursal = @SessionIdSucursal AND V.estado = 1 AND C.idCaja = (SELECT MAX(idCaja) FROM Caja WHERE idSucursal = @SessionIdSucursal)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    CajaUSD = double.Parse(dt.Rows[0][0].ToString());
                    CajaBOB = double.Parse(dt.Rows[0][1].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return (CajaUSD, CajaBOB);
        }

        public string GetTodayProducts(DateTime FechaHoy)
        {
            string numeroProductosdelDia = null;
            string query = @"SELECT COUNT(DV.idProducto) FROM Venta V
                            INNER JOIN Detalle_Venta DV ON V.idVenta = DV.idVenta
                            WHERE V.idSucursal = @SessionIdSucursal AND V.estado = 1
                            AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@FechaInicio", FechaHoy.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", FechaHoy.ToString("yyyy-MM-dd") + " 23:59:59");
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    numeroProductosdelDia = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return numeroProductosdelDia;
        }

        public DataTable SelectSalesWithPendingBalanceFromBranch()
        {
            string query = @"SELECT V.idVenta, U.nombreUsuario AS Usuario, CONCAT('Venta: ',V.idVenta,' (',GROUP_CONCAT('- ',P.nombreProducto SEPARATOR ' '),')') AS Detalle, saldoUSD AS 'Saldo $us', saldoBOB AS 'Saldo Bs' FROM Venta V
                            INNER JOIN Usuario U ON V.idUsuario = U.idUsuario
                            INNER JOIN Detalle_Venta DV ON V.idVenta = DV.idVenta
                            INNER JOIN Producto P ON DV.idProducto = P.idProducto
                            WHERE V.saldoUSD > 1 AND V.idSucursal = @SessionIdSucursal AND V.estado = 1
                            GROUP BY V.idVenta ORDER BY 1 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable SelectLikeReporteUtilidades(DateTime fechaInicio, DateTime fechaFin, string idSucursales, string idCategorias, string idUsuarios)
        {
            string query = @"SELECT V.idVenta AS 'ID', V.fechaRegistro AS Fecha, S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, 
                            V.idVenta AS 'Nro Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, P.costoUSD AS 'P Costo', DV.precioUSD AS 'P Venta', (DV.precioUSD - P.costoUSD) AS Utilidad FROM Venta V
                            INNER JOIN Sucursal S ON S.idSucursal = V.idSucursal
                            INNER JOIN Usuario U ON U.idUsuario = V.idUsuario
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE V.estado = 1 AND V.saldoUSD < 1 AND V.idSucursal IN (" + idSucursales + ") AND P.idCategoria IN (" + idCategorias + ") AND V.idUsuario IN (" + idUsuarios + @")
                            AND V.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd") + " 23:59:59");
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectLikeReporteVentasGlobales(DateTime fechaInicio, DateTime fechaFin, string idSucursales, string idCategorias, string idUsuarios)
        {
            string query = @"SELECT V.fechaRegistro AS Fecha, S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, 
                            V.idVenta AS 'Nro Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Precio USD', DV.precioBOB AS 'Precio Bs' FROM Venta V
                            INNER JOIN Sucursal S ON S.idSucursal = V.idSucursal
                            INNER JOIN Usuario U ON U.idUsuario = V.idUsuario
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE V.estado = 1 AND V.idSucursal IN (" + idSucursales + ") AND P.idCategoria IN (" + idCategorias + ") AND V.idUsuario IN (" + idUsuarios + @")
                            AND V.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd") + " 23:59:59");
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectLikeReporteVentasLocales(DateTime fechaInicio, DateTime fechaFin, string productoOCodigo, string clienteoCI)
        {
            string query = @"SELECT V.idVenta AS 'ID', V.fechaRegistro AS Fecha, CL.nombre AS Cliente, V.idVenta AS 'Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM Venta V
                            INNER JOIN Cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE (P.nombreProducto LIKE @productocodigoproducto OR P.codigoSublote LIKE @productocodigoproducto OR CL.nombre LIKE @clienteoci OR CL.numeroCI LIKE @clienteoci)
                            AND V.estado = 1 AND V.idSucursal = @SessionSucursal
                            AND V.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionSucursal", Session.Sucursal_IdSucursal);
            command.Parameters.AddWithValue("@productocodigoproducto", "%" + productoOCodigo + "%");
            command.Parameters.AddWithValue("@clienteoci", "%" + clienteoCI + "%");
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd") + " 23:59:59");
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectLikeReporteVentasLocalesByID(int idVenta)
        {
            string query = @"SELECT V.idVenta AS 'ID', V.fechaRegistro AS Fecha, CL.nombre AS Cliente, V.idVenta AS 'Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM Venta V
                            INNER JOIN Cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE V.estado = 1 AND V.idSucursal = @SessionSucursal AND V.idVenta = @idVenta
                            GROUP BY P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionSucursal", Session.Sucursal_IdSucursal);
            command.Parameters.AddWithValue("@idVenta", idVenta);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectSaleDetails1()
        {
            string query = @"SELECT V.idVenta AS ID, 
                S.nombreSucursal AS Sucursal, S.direccion AS Direccion, S.telefono AS Telefono, S.correo AS Correo,
                CL.nombre AS Cliente, CL.numeroCelular AS Celular, CL.numeroCI AS CI,
                U.nombreUsuario AS Usuario, E.numeroCelular AS 'Celular Usuario',
                CONCAT(P.codigoSublote,' ',P.nombreProducto) AS Producto, P.identificador AS Detalle, DV.garantia AS Garantia, DV.cantidad AS Cantidad, P.precioVentaBOB AS Precio, DV.descuento AS 'Descuento Porcentaje', (P.precioVentaBOB - DV.precioBOB) AS 'Descuento Bs', DV.precioBOB AS 'Total Producto',
                V.totalBOB AS Total, V.saldoBOB AS Saldo, V.observaciones AS Observaciones, DATE_FORMAT(V.fechaRegistro,'%d/%m/%Y') AS Fecha,
                V.totalUSD AS Total2, V.saldoUSD AS Saldo2, P.idProducto AS IDProducto
                FROM Venta V
                INNER JOIN Cliente CL ON CL.idCliente = V.idCliente
                INNER JOIN Usuario U ON U.idUsuario = V.idUsuario
                INNER JOIN Empleado E ON E.idEmpleado = U.idEmpleado
                INNER JOIN Sucursal S ON S.idSucursal = V.idSucursal
                INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                INNER JOIN Producto P ON P.idProducto = DV.idProducto
                WHERE V.idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idVenta", Session.IdVentaDetalle);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable SelectSaleDetails2()
        {
            string query = @"SELECT montoBOB AS 'Monto Bs', DATE_FORMAT(fechaRegistro,'%d/%m/%Y') AS Fecha FROM Metodo_Pago
                WHERE idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idVenta", Session.IdVentaDetalle);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetIDAfterInsert()
        {
            int idVenta = 0;
            string query = @"SELECT MAX(idVenta) FROM Venta WHERE idSucursal = @sucursalOrigen";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@sucursalOrigen", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    idVenta = int.Parse(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return idVenta;
        }

        public DataTable SelectPaymentMethodsFromSale(int IdVenta)
        {
            string query = @"SELECT idMetodoPago AS ID, montoUSD AS 'Monto USD', montoBOB AS 'Monto Bs', IF(Tipo = 1, 'EFECTIVO',IF(Tipo = 2, 'TRANSFERENCIA BANCARIA', 'TARJETA')) AS 'Metodo Pago', DATE_FORMAT(fechaRegistro,'%d/%m/%Y') AS Fecha FROM Metodo_Pago
                            WHERE idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idVenta", IdVenta);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string InsertPaymentMethodTransaction(int IdVenta, double PagoUSD, double PagoBOB, byte MetodoPago)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {
                //REGISTRO DEL METODO DE PAGO.
                command.CommandText = @"INSERT INTO Metodo_Pago (idVenta,montoUSD,montoBOB,tipo)
                            VALUES(@idVenta,@montoUSD,@montoBOB,@tipo)";
                command.Parameters.AddWithValue("@idVenta", IdVenta);
                command.Parameters.AddWithValue("@montoUSD", PagoUSD);
                command.Parameters.AddWithValue("@montoBOB", PagoBOB);
                command.Parameters.AddWithValue("@tipo", MetodoPago);
                command.ExecuteNonQuery();

                //UPDATE DEL SALDO DE LA VENTA.
                command.Parameters.Clear();
                command.CommandText = @"UPDATE Venta SET saldoUSD = saldoUSD - @montoUSD, saldoBOB = saldoBOB - @montoBOB
                                        WHERE idVenta = @idVenta";
                command.Parameters.AddWithValue("@montoUSD", PagoUSD);
                command.Parameters.AddWithValue("@montoBOB", PagoBOB);
                command.Parameters.AddWithValue("@idVenta", IdVenta);
                command.ExecuteNonQuery();

                //REGISTRO DEL MP EN LA CAJA.
                command.Parameters.Clear();
                command.CommandText = @"INSERT INTO Detalle_Caja (idCaja,idMetodoPago)
                            VALUES((SELECT MAX(idCaja) FROM Caja WHERE idSucursal = @Twice_Session_idSucursal),LAST_INSERT_ID())";
                command.Parameters.AddWithValue("@Twice_Session_idSucursal", Session.Sucursal_IdSucursal);
                command.ExecuteNonQuery();

                myTrans.Commit();
                return "INSERTMETODOPAGO_EXITOSO";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public string DeletePaymentMethodTransaction(int IdVenta, int IdMetodoPago, double MontoUSD, double MontoBOB)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {

                //ELIMINANDO EL MP DE LA CAJA.
                command.CommandText = @"DELETE FROM Detalle_Caja WHERE idMetodoPago = @idMetodoPago";
                command.Parameters.AddWithValue("@idMetodoPago", IdMetodoPago);
                command.ExecuteNonQuery();
                //ELIMINACION DEL METODO DE PAGO.
                command.CommandText = @"DELETE FROM Metodo_Pago WHERE idMetodoPago = @idMetodoPago_twice";
                command.Parameters.AddWithValue("@idMetodoPago_twice", IdMetodoPago);
                command.ExecuteNonQuery();
                //UPDATE DEL SALDO DE LA VENTA.
                command.CommandText = @"UPDATE Venta SET saldoUSD = saldoUSD + @montoUSD, saldoBOB = saldoBOB + @montoBOB
                                        WHERE idVenta = @idVenta";
                command.Parameters.AddWithValue("@montoUSD", MontoUSD);
                command.Parameters.AddWithValue("@montoBOB", MontoBOB);
                command.Parameters.AddWithValue("@idVenta", IdVenta);
                command.ExecuteNonQuery();

                myTrans.Commit();
                return "DELETEMETODOPAGO_EXITOSO";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public byte GetEstado(int IdVenta)
        {
            byte estado = 0;
            string query = @"SELECT estado FROM Venta WHERE idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idVenta", IdVenta);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    estado = byte.Parse(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return estado;
        }
        public string DeleteSaleTransaction(int IdVenta, string Observacion, List<int> ListaIDProductos)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {
                //ELIMINANDO EL MP DE LA CAJA.
                command.CommandText = @"UPDATE Producto P
                                        INNER JOIN Detalle_Venta DV ON DV.idProducto = P.idProducto
                                        SET P.estado = 1 WHERE DV.idVenta = @idVenta";
                command.Parameters.AddWithValue("@idVenta", IdVenta);
                command.ExecuteNonQuery();
                //ELIMINACION DEL METODO DE PAGO.
                command.CommandText = @"UPDATE Venta SET observaciones = @observaciones, estado = 0 WHERE idVenta = @idVentaTwice";
                command.Parameters.AddWithValue("@observaciones", Observacion);
                command.Parameters.AddWithValue("@idVentaTwice", IdVenta);
                command.ExecuteNonQuery();
                foreach (var item in ListaIDProductos)
                {
                    command.CommandText = @"INSERT INTO Historial (idProducto,detalle) VALUES
                                (@idProducto,@detalle)";
                    command.Parameters.AddWithValue("@idProducto", item);
                    command.Parameters.AddWithValue("@detalle", "EL PRODUCTO RETORNÓ A SISTEMA POR ELIMINACIÓN DE LA VENTA, MOTIVO: " + Observacion);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                myTrans.Commit();
                return "DELETEVENTA_EXITOSO";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public DataTable SelectLikeReporteVentasLocalesDELETED(DateTime fechaInicio, DateTime fechaFin, string productoOCodigo, string clienteoCI)
        {
            string query = @"SELECT V.idVenta AS 'ID', V.fechaRegistro AS Fecha, CL.nombre AS Cliente, V.idVenta AS 'Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM Venta V
                            INNER JOIN Cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE (P.nombreProducto LIKE @productocodigoproducto OR P.codigoSublote LIKE @productocodigoproducto OR CL.nombre LIKE @clienteoci OR CL.numeroCI LIKE @clienteoci)
                            AND V.estado = 0 AND V.idSucursal = @SessionSucursal
                            AND V.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY V.idVenta, P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionSucursal", Session.Sucursal_IdSucursal);
            command.Parameters.AddWithValue("@productocodigoproducto", "%" + productoOCodigo + "%");
            command.Parameters.AddWithValue("@clienteoci", "%" + clienteoCI + "%");
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd") + " 23:59:59");
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectLikeReporteVentasLocalesByIDDELETED(int idVenta)
        {
            string query = @"SELECT V.idVenta AS 'ID', V.fechaRegistro AS Fecha, CL.nombre AS Cliente, V.idVenta AS 'Venta', P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS Identificador,
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM Venta V
                            INNER JOIN Cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN Detalle_Venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN Producto P ON P.idProducto = DV.idProducto
                            INNER JOIN Categoria C ON C.idCategoria = P.idCategoria
                            WHERE V.estado = 0 AND V.idSucursal = @SessionSucursal AND V.idVenta = @idVenta
                            GROUP BY P.idProducto
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionSucursal", Session.Sucursal_IdSucursal);
            command.Parameters.AddWithValue("@idVenta", idVenta);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectSalesWithPendingBalanceByCustomers()
        {
            string query = @"SELECT V.idCliente AS ID, C.nombre AS Nombre, C.numeroCelular AS Celular, SUM(V.saldoUSD) AS 'Saldo Total' FROM Venta V
                                INNER JOIN Cliente C ON C.idCliente = V.idCliente
                                WHERE V.saldoUSD > 10 AND V.estado = 1 AND V.idSucursal = @SessionIdSucursal
                                GROUP BY V.idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable SelectAllSalesWithPendingBalanceByCustomers()
        {
            string query = @"SELECT V.idVenta, U.nombreUsuario AS Usuario, CONCAT('Venta: ',V.idVenta,' (',GROUP_CONCAT('- ',P.nombreProducto SEPARATOR ' '),')') AS Detalle, saldoUSD AS 'Saldo $us', saldoBOB AS 'Saldo Bs' FROM Venta V
                            INNER JOIN Usuario U ON V.idUsuario = U.idUsuario
                            INNER JOIN Detalle_Venta DV ON V.idVenta = DV.idVenta
                            INNER JOIN Producto P ON DV.idProducto = P.idProducto
                            WHERE V.saldoUSD > 1 AND V.idSucursal = @SessionIdSucursal AND V.estado = 1 AND V.idCliente = @SessionIdCliente
                            GROUP BY V.idVenta ORDER BY 1 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionIdSucursal", Session.Sucursal_IdSucursal);
            command.Parameters.AddWithValue("@SessionIdCliente", Session.IdCliente);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string UpdateSaleProductsTransaction(Venta venta, List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<byte> ListaGarantias)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    command.CommandText = @"UPDATE Detalle_Venta SET precioUSD = @precioUSD, precioBOB = @precioBOB, descuento = @descuento, garantia = @garantia
                    WHERE idVenta = @idVenta AND idProducto = @idProducto";
                    command.Parameters.AddWithValue("@precioUSD", ListaProductos[i].PrecioVentaUSD);
                    command.Parameters.AddWithValue("@precioBOB", ListaProductos[i].PrecioVentaBOB);
                    command.Parameters.AddWithValue("@descuento", ListaDescuentosPorcentaje[i]);
                    command.Parameters.AddWithValue("@garantia", ListaGarantias[i]);
                    command.Parameters.AddWithValue("@idVenta", venta.IdVenta);
                    command.Parameters.AddWithValue("@idProducto", ListaProductos[i].IdProducto);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                command.CommandText = @"UPDATE Venta SET totalUSD = @totalUSD, totalBOB = @totalBOB, saldoUSD = @saldoUSD, saldoBOB = @saldoBOB,
                    fechaActualizacion = CURRENT_TIMESTAMP WHERE idVenta = @idVenta";
                command.Parameters.AddWithValue("@totalUSD", venta.TotalUSD);
                command.Parameters.AddWithValue("@totalBOB", venta.TotalBOB);
                command.Parameters.AddWithValue("@saldoUSD", venta.SaldoUSD);
                command.Parameters.AddWithValue("@saldoBOB", venta.SaldoBOB);
                command.Parameters.AddWithValue("@idVenta", venta.IdVenta);
                command.ExecuteNonQuery();
                myTrans.Commit();
                return "UPDATEPRODUCTOS_EXITOSO";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
