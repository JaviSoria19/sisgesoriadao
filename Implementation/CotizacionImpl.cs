using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class CotizacionImpl : DataBase, ICotizacion
    {
        public string InsertTransaction(List<Producto> ListaProductos, Cotizacion Cotizacion)
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
                //AÑADIENDO NUEVA COTIZACION.
                command.CommandText = @"INSERT INTO Cotizacion (idUsuario,idSucursal,nombreCliente,nombreEmpresa,nit,direccion,correo,telefono,tiempoEntrega)
                                        VALUES (@idUsuario,@idSucursal,@nombreCliente,@nombreEmpresa,@nit,@direccion,@correo,@telefono,@tiempoEntrega)";
                command.Parameters.AddWithValue("@idUsuario", Session.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
                command.Parameters.AddWithValue("@nombreCliente", Cotizacion.NombreCliente);
                command.Parameters.AddWithValue("@nombreEmpresa", Cotizacion.NombreEmpresa);
                command.Parameters.AddWithValue("@nit", Cotizacion.Nit);
                command.Parameters.AddWithValue("@direccion", Cotizacion.Direccion);
                command.Parameters.AddWithValue("@correo", Cotizacion.Correo);
                command.Parameters.AddWithValue("@telefono", Cotizacion.Telefono);
                command.Parameters.AddWithValue("@tiempoEntrega", Cotizacion.TiempoEntrega);
                command.ExecuteNonQuery();
                foreach (var Producto in ListaProductos)
                {
                    //LIMPIEZA DE PARÁMETROS YA UTILIZADOS EN EL CICLO ANTERIOR PARA PROSEGUIR, CASO CONTRARIO LANZA ERROR.
                    command.Parameters.Clear();
                    //REGISTRO DEL DETALLE DE LA COTIZACION.
                    command.CommandText = @"INSERT INTO Detalle_Cotizacion (idCotizacion,idProducto,CotizacionUSD,CotizacionBOB) 
                                VALUES ((SELECT MAX(idCotizacion) FROM Cotizacion WHERE idSucursal = @Session_idSucursal),@idProducto,@CotizacionUSD,@CotizacionBOB)";
                    command.Parameters.AddWithValue("@Session_idSucursal", Session.Sucursal_IdSucursal);
                    command.Parameters.AddWithValue("@idProducto", Producto.IdProducto);
                    command.Parameters.AddWithValue("@CotizacionUSD", Producto.PrecioVentaUSD);
                    command.Parameters.AddWithValue("@CotizacionBOB", Producto.PrecioVentaBOB);
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                return "COTIZACION REGISTRADA EXITOSAMENTE.";
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
        public string UpdateTransaction(List<Producto> ListaProductos, Cotizacion cotizacion)
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
                //AÑADIENDO NUEVA COTIZACION.
                command.CommandText = @"UPDATE Cotizacion SET nombreCliente = @nombreCliente, nombreEmpresa = @nombreEmpresa, nit = @nit, direccion = @direccion, correo = @correo, telefono = @telefono, tiempoEntrega = @tiempoEntrega
                    WHERE idCotizacion = @idCotizacion";
                command.Parameters.AddWithValue("@nombreCliente", cotizacion.NombreCliente);
                command.Parameters.AddWithValue("@nombreEmpresa", cotizacion.NombreEmpresa);
                command.Parameters.AddWithValue("@nit", cotizacion.Nit);
                command.Parameters.AddWithValue("@direccion", cotizacion.Direccion);
                command.Parameters.AddWithValue("@correo", cotizacion.Correo);
                command.Parameters.AddWithValue("@telefono", cotizacion.Telefono);
                command.Parameters.AddWithValue("@tiempoEntrega", cotizacion.TiempoEntrega);
                command.Parameters.AddWithValue("@idCotizacion", cotizacion.IdCotizacion);
                command.ExecuteNonQuery();
                foreach (var Producto in ListaProductos)
                {
                    //LIMPIEZA DE PARÁMETROS YA UTILIZADOS EN EL CICLO ANTERIOR PARA PROSEGUIR, CASO CONTRARIO LANZA ERROR.
                    command.Parameters.Clear();
                    //REGISTRO DEL DETALLE DE LA COTIZACION.
                    command.CommandText = @"UPDATE Detalle_Cotizacion SET cotizacionUSD = @cotizacionUSD, cotizacionBOB = @cotizacionBOB
                        WHERE idCotizacion = @idCotizacion AND idProducto = @idProducto";
                    command.Parameters.AddWithValue("@idCotizacion", cotizacion.IdCotizacion);
                    command.Parameters.AddWithValue("@idProducto", Producto.IdProducto);
                    command.Parameters.AddWithValue("@cotizacionUSD", Producto.PrecioVentaUSD);
                    command.Parameters.AddWithValue("@cotizacionBOB", Producto.PrecioVentaBOB);
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                return "COTIZACION MODIFICADA EXITOSAMENTE.";
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
        public int Delete(Cotizacion c)
        {
            string query = @"UPDATE Cotizacion SET estado = 0 WHERE idCotizacion = @idCotizacion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", c.IdCotizacion);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Cotizacion Get(int id)
        {
            Cotizacion c = null;
            string query = @"SELECT idCotizacion, idUsuario, idSucursal, nombreCliente, nombreEmpresa, nit, direccion, correo, telefono, tiempoEntrega, fechaRegistro FROM Cotizacion 
                            WHERE idCotizacion=@idCotizacion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cotizacion(
                        int.Parse(dt.Rows[0][0].ToString()),            /*idCotizacion*/
                        byte.Parse(dt.Rows[0][1].ToString()),           /*idUsuario*/
                        byte.Parse(dt.Rows[0][2].ToString()),           /*idSucursal*/
                        dt.Rows[0][3].ToString(),           /*nombreCliente*/
                        dt.Rows[0][4].ToString(),           /*nombreEmpresa*/
                        dt.Rows[0][5].ToString(),           /*nit*/
                        dt.Rows[0][6].ToString(),           /*direccion*/
                        dt.Rows[0][7].ToString(),           /*correo*/
                        dt.Rows[0][8].ToString(),           /*telefono*/
                        //tiempoEntrega,fechaRegistro
                        DateTime.Parse(dt.Rows[0][9].ToString()),
                        DateTime.Parse(dt.Rows[0][10].ToString()));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }
        public int Insert(Cotizacion c)
        {
            throw new NotImplementedException();
        }
        public DataTable Select()
        {
            string query = @"SELECT C.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, C.nombreCliente AS Cliente, C.nombreEmpresa AS Empresa, C.nit AS NIT, C.direccion AS Direccion, C.correo AS Correo, C.telefono AS Telefono, COUNT(DC.idCotizacion) AS 'Productos cotizados',C.fechaRegistro AS 'Fecha de Registro', C.tiempoEntrega AS 'Fecha de Entrega' FROM Cotizacion C
                            INNER JOIN Usuario U ON C.idUsuario = U.idUsuario
                            INNER JOIN Sucursal S ON C.idSucursal = S.idSucursal
                            INNER JOIN Detalle_Cotizacion DC ON C.idCotizacion = DC.idCotizacion
                            WHERE C.estado = 1
                            GROUP BY C.idCotizacion
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable SelectLike(string CadenaBusqueda, DateTime fechaInicio, DateTime fechaFin)
        {
            string query = @"SELECT C.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, C.nombreCliente AS Cliente, C.nombreEmpresa AS Empresa, C.nit AS NIT, C.direccion AS Direccion, C.correo AS Correo, C.telefono AS Telefono, COUNT(DC.idCotizacion) AS 'Productos cotizados',C.fechaRegistro AS 'Fecha de Registro', C.tiempoEntrega AS 'Fecha de Entrega' FROM Cotizacion C
                            INNER JOIN Usuario U ON C.idUsuario = U.idUsuario
                            INNER JOIN Sucursal S ON C.idSucursal = S.idSucursal
                            INNER JOIN Detalle_Cotizacion DC ON C.idCotizacion = DC.idCotizacion
                            WHERE C.estado = 1 AND (U.nombreUsuario LIKE @search OR S.nombreSucursal LIKE @search OR C.idCotizacion LIKE @search OR C.nombreCliente LIKE @search OR C.nombreEmpresa LIKE @search OR C.nit LIKE @search OR C.telefono LIKE @search)
                            AND C.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY C.idCotizacion
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@search", "%" + CadenaBusqueda + "%");
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
        public DataTable SelectDetails(int idCotizacion)
        {
            string query = @"SELECT DC.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, P.nombreProducto AS Producto, DC.CotizacionUSD AS 'Cotizacion USD', DC.CotizacionBOB AS 'Cotizacion BOB', " + Session.FormatoFechaMySql("C.fechaRegistro") + @" AS 'Fecha de Registro', P.idProducto AS IDProducto, P.costoUSD AS 'Costo USD', P.precioVentaUSD AS 'Precio USD', P.precioVentaBOB AS 'Precio BOB' FROM Detalle_Cotizacion DC
                            INNER JOIN Producto P ON DC.idProducto = P.idProducto
                            INNER JOIN Cotizacion C ON DC.idCotizacion = C.idCotizacion 
                            INNER JOIN Sucursal S ON C.idSucursal = S.idSucursal
                            WHERE DC.idCotizacion = @idCotizacion
                            ORDER BY 3 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", idCotizacion);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable SelectDetails2(int idCotizacion)
        {
            string query = @"SELECT C.idCotizacion, S.nombreSucursal, S.direccion, S.telefono, S.correo, C.nombreCliente, C.nombreEmpresa, C.nit, C.direccion, C.correo, C.telefono, DATE_FORMAT(C.tiempoEntrega,'%d/%m/%Y'), " + Session.FormatoFechaMySql("C.fechaRegistro") + @" FROM Cotizacion C
                            INNER JOIN Sucursal S ON S.idSucursal = C.idSucursal
                            WHERE C.idCotizacion = @idCotizacion;";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", idCotizacion);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update(Cotizacion c)
        {
            throw new NotImplementedException();
        }

        public Cotizacion GetLastFromBranch()
        {
            Cotizacion c = null;
            string query = @"SELECT idCotizacion, idUsuario, idSucursal, nombreCliente, nombreEmpresa, nit, direccion, correo, telefono, tiempoEntrega, fechaRegistro FROM Cotizacion 
                            WHERE idSucursal = @idSucursal AND idCotizacion = (SELECT MAX(idCotizacion) FROM Cotizacion WHERE idSucursal = @idSucursal) LIMIT 1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cotizacion(
                        int.Parse(dt.Rows[0][0].ToString()),            /*idCotizacion*/
                        byte.Parse(dt.Rows[0][1].ToString()),           /*idUsuario*/
                        byte.Parse(dt.Rows[0][2].ToString()),           /*idSucursal*/
                        dt.Rows[0][3].ToString(),           /*nombreCliente*/
                        dt.Rows[0][4].ToString(),           /*nombreEmpresa*/
                        dt.Rows[0][5].ToString(),           /*nit*/
                        dt.Rows[0][6].ToString(),           /*direccion*/
                        dt.Rows[0][7].ToString(),           /*correo*/
                        dt.Rows[0][8].ToString(),           /*telefono*/
                        //tiempoEntrega,fechaRegistro
                        DateTime.Parse(dt.Rows[0][9].ToString()),
                        DateTime.Parse(dt.Rows[0][10].ToString()));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }
        public byte GetEstado(int idCotizacion)
        {
            byte estado = 0;
            string query = @"SELECT estado FROM Cotizacion WHERE idCotizacion = @idCotizacion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", idCotizacion);
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

        public string DeleteAfterQuotationProductTransaction(Cotizacion cotizacion, int IdProducto)
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
                //ELIMINANDO EL PRODUCTO DE LA VENTA.
                command.CommandText = @"DELETE FROM Detalle_Cotizacion WHERE idCotizacion = @idCotizacion AND idProducto = @idProducto";
                command.Parameters.AddWithValue("@idCotizacion", cotizacion.IdCotizacion);
                command.Parameters.AddWithValue("@idProducto", IdProducto);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                myTrans.Commit();
                return "DELETEPRODUCTO_EXITOSO";
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
