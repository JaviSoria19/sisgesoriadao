using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using MySql.Data.MySqlClient;//MySql.Data
namespace sisgesoriadao.Implementation
{
    public class VentaImpl : DataBase, IVenta
    {
        public string InsertTransaction(Venta venta, List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<Categoria> ListaGarantias, List<MetodoPago> ListaMetodosPago, Cliente cliente  )
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
                command.CommandText = @"INSERT INTO venta (idCliente,idUsuario,idSucursal,totalUSD,totalBOB,saldoUSD,saldoBOB,observaciones) 
                            VALUES(@idCliente,@idUsuario,@idSucursal,@totalUSD,@totalBOB,@saldoUSD,@saldoBOB,@observaciones)";
                command.Parameters.AddWithValue("@idCliente", venta.IdCliente);
                command.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", venta.IdSucursal);
                command.Parameters.AddWithValue("@totalUSD", venta.TotalUSD);
                command.Parameters.AddWithValue("@totalBOB", venta.TotalBOB);
                command.Parameters.AddWithValue("@saldoUSD", venta.SaldoUSD);
                command.Parameters.AddWithValue("@saldoBOB", venta.SaldoBOB);
                command.Parameters.AddWithValue("@observaciones", venta.Observaciones);
                command.ExecuteNonQuery();
                //REGISTRO DE LOS PRODUCTOS, ACTUALIZACIÓN DEL ESTADO DE LOS PRODUCTOS E INSERCIÓN EN HISTORIAL.
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    command.CommandText = @"INSERT INTO detalle_venta (idVenta,idProducto,cantidad,precioUSD,precioBOB,descuento,garantia)
                            VALUES((SELECT MAX(idVenta) FROM venta),@idProducto,@cantidad,@precioUSD,@precioBOB,@descuento,@garantia)";
                    command.Parameters.AddWithValue("@idProducto", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@cantidad", 1);
                    command.Parameters.AddWithValue("@precioUSD", ListaProductos[i].PrecioVentaUSD);
                    command.Parameters.AddWithValue("@precioBOB", ListaProductos[i].PrecioVentaBOB);
                    command.Parameters.AddWithValue("@descuento", ListaDescuentosPorcentaje[i]);
                    command.Parameters.AddWithValue("@garantia", ListaGarantias[i].Garantia);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE producto SET estado = 2, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProductoTwice";
                    command.Parameters.AddWithValue("@idProductoTwice", ListaProductos[i].IdProducto);
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO historial (idProducto,detalle) VALUES
                                (@idProductoThree,@detalle)";
                    command.Parameters.AddWithValue("@idProductoThree", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@detalle", "PRODUCTO VENDIDO POR EL USUARIO: " + Session.NombreUsuario + " AL CLIENTE: " + cliente.Nombre);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
                if (ListaMetodosPago.Count > 0)
                {
                    foreach (var item in ListaMetodosPago)
                    {
                        //REGISTRO DEL DETALLE DE VENTA.
                        command.CommandText = @"INSERT INTO metodo_pago (idVenta,montoUSD,montoBOB,tipo)
                            VALUES((SELECT MAX(idVenta) FROM venta WHERE idSucursal = @Session_idSucursal),@montoUSD,@montoBOB,@tipo)";
                        command.Parameters.AddWithValue("@Session_idSucursal", Session.Sucursal_IdSucursal);
                        command.Parameters.AddWithValue("@montoUSD", item.MontoUSD);
                        command.Parameters.AddWithValue("@montoBOB", item.MontoBOB);
                        command.Parameters.AddWithValue("@tipo", item.Tipo);
                        command.ExecuteNonQuery();
                        //REGISTRO DEL DETALLE DE CAJA.
                        command.CommandText = @"INSERT INTO detalle_caja (idCaja,idMetodoPago)
                            VALUES((SELECT MAX(idCaja) FROM caja WHERE idSucursal = @Twice_Session_idSucursal),LAST_INSERT_ID())";
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
            string query = @"SELECT COUNT(idVenta) FROM venta WHERE idSucursal = @SessionIdSucursal
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
            string query = @"SELECT IFNULL(SUM(MP.montoUSD),0), IFNULL(SUM(MP.montoBOB),0) FROM caja C
                                INNER JOIN detalle_caja DC ON C.idCaja = DC.idCaja
                                INNER JOIN metodo_pago MP ON DC.idMetodoPago = MP.idMetodoPago
                                WHERE C.idSucursal = @SessionIdSucursal AND C.idCaja = (SELECT MAX(idCaja) FROM caja WHERE idSucursal = @SessionIdSucursal)";
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
            return (CajaUSD,CajaBOB);
        }

        public string GetTodayProducts(DateTime FechaHoy)
        {
            string numeroProductosdelDia = null;
            string query = @"SELECT COUNT(DV.idProducto) FROM venta V
                            INNER JOIN detalle_venta DV ON V.idVenta = DV.idVenta
                            WHERE V.idSucursal = @SessionIdSucursal
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
            string query = @"SELECT V.idVenta, U.nombreUsuario AS Usuario, CONCAT('Venta: ',V.idVenta,' (',GROUP_CONCAT('- ',P.nombreProducto SEPARATOR ' '),')') AS Detalle, saldoUSD AS 'Saldo $us', saldoBOB AS 'Saldo Bs' FROM venta V
                            INNER JOIN usuario U ON V.idUsuario = U.idUsuario
                            INNER JOIN detalle_venta DV ON V.idVenta = DV.idVenta
                            INNER JOIN producto P ON DV.idProducto = P.idProducto
                            WHERE V.saldoUSD > 1 AND V.idSucursal = @SessionIdSucursal
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
                            C.nombreCategoria AS Categoria, P.costoUSD AS 'P Costo', DV.precioUSD AS 'P Venta', (DV.precioUSD - P.costoUSD) AS Utilidad FROM venta V
                            INNER JOIN sucursal S ON S.idSucursal = V.idSucursal
                            INNER JOIN usuario U ON U.idUsuario = V.idUsuario
                            INNER JOIN detalle_venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN producto P ON P.idProducto = DV.idProducto
                            INNER JOIN categoria C ON C.idCategoria = P.idCategoria
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
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Precio USD', DV.precioBOB AS 'Precio Bs' FROM venta V
                            INNER JOIN sucursal S ON S.idSucursal = V.idSucursal
                            INNER JOIN usuario U ON U.idUsuario = V.idUsuario
                            INNER JOIN detalle_venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN producto P ON P.idProducto = DV.idProducto
                            INNER JOIN categoria C ON C.idCategoria = P.idCategoria
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
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM venta V
                            INNER JOIN cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN detalle_venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN producto P ON P.idProducto = DV.idProducto
                            INNER JOIN categoria C ON C.idCategoria = P.idCategoria
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
                            C.nombreCategoria AS Categoria, DV.precioUSD AS 'Total USD', IF(V.saldoUSD < 1, 0, V.saldoUSD) AS 'Saldo USD' FROM venta V
                            INNER JOIN cliente CL ON CL.idCliente = V.idCliente
                            INNER JOIN detalle_venta DV ON DV.idVenta = V.idVenta
                            INNER JOIN producto P ON P.idProducto = DV.idProducto
                            INNER JOIN categoria C ON C.idCategoria = P.idCategoria
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
                V.totalBOB AS Total, V.saldoBOB AS Saldo, V.observaciones AS Observaciones, DATE_FORMAT(V.fechaRegistro,'%d/%m/%Y') AS Fecha FROM venta V
                INNER JOIN cliente CL ON CL.idCliente = V.idCliente
                INNER JOIN usuario U ON U.idUsuario = V.idUsuario
                INNER JOIN empleado E ON E.idEmpleado = U.idEmpleado
                INNER JOIN sucursal S ON S.idSucursal = V.idSucursal
                INNER JOIN detalle_venta DV ON DV.idVenta = V.idVenta
                INNER JOIN producto P ON P.idProducto = DV.idProducto
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
            string query = @"SELECT montoBOB AS 'Monto Bs', DATE_FORMAT(fechaRegistro,'%d/%m/%Y') AS Fecha FROM metodo_pago
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
            string query = @"SELECT MAX(idVenta) FROM venta WHERE idSucursal = @sucursalOrigen";
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
    }
}
