﻿using System;
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
    public class CotizacionImpl : DataBase, ICotizacion
    {
        public string InsertTransaction(List<Producto> ListaProductos)
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
                command.CommandText = @"INSERT INTO cotizacion (idUsuario,idSucursal) VALUES (@idUsuario,@idSucursal);";
                command.Parameters.AddWithValue("@idUsuario", Session.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
                command.ExecuteNonQuery();
                foreach (var producto in ListaProductos)
                {
                    //LIMPIEZA DE PARÁMETROS YA UTILIZADOS EN EL CICLO ANTERIOR PARA PROSEGUIR, CASO CONTRARIO LANZA ERROR.
                    command.Parameters.Clear();
                    //REGISTRO DEL DETALLE DE LA COTIZACION.
                    command.CommandText = @"INSERT INTO detalle_cotizacion (idCotizacion,idProducto,cotizacionUSD,cotizacionBOB) 
                                VALUES ((SELECT MAX(idCotizacion) FROM cotizacion WHERE idSucursal = @Session_idSucursal),@idProducto,@cotizacionUSD,@cotizacionBOB)";
                    command.Parameters.AddWithValue("@Session_idSucursal", Session.Sucursal_IdSucursal);
                    command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    command.Parameters.AddWithValue("@cotizacionUSD", producto.PrecioVentaUSD);
                    command.Parameters.AddWithValue("@cotizacionBOB", producto.PrecioVentaBOB);
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
        public int Delete(Cotizacion c)
        {
            throw new NotImplementedException();
        }
        public Cotizacion Get(int id)
        {
            Cotizacion c = null;
            string query = @"SELECT idCotizacion, idUsuario, idSucursal, fechaRegistro FROM cotizacion 
                            WHERE idCotizacion=@idCotizacion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCotizacion", id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cotizacion(
                        int.Parse(dt.Rows[0][0].ToString()),     /*idCotizacion*/
                        byte.Parse(dt.Rows[0][1].ToString()),                   /*idUsuario*/
                        byte.Parse(dt.Rows[0][2].ToString()),                   /*idSucursal*/
                        //FechaRegistro.
                        DateTime.Parse(dt.Rows[0][3].ToString()));
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
            string query = @"SELECT C.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, COUNT(DC.idCotizacion) AS 'Productos cotizados',C.fechaRegistro AS 'Fecha de Registro' FROM cotizacion C
                            INNER JOIN usuario U ON C.idUsuario = U.idUsuario
                            INNER JOIN sucursal S ON C.idSucursal = S.idSucursal
                            INNER JOIN detalle_cotizacion DC ON C.idCotizacion = DC.idCotizacion
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
            string query = @"SELECT C.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, U.nombreUsuario AS Usuario, COUNT(DC.idCotizacion) AS 'Productos cotizados',C.fechaRegistro AS 'Fecha de Registro' FROM cotizacion C
                            INNER JOIN usuario U ON C.idUsuario = U.idUsuario
                            INNER JOIN sucursal S ON C.idSucursal = S.idSucursal
                            INNER JOIN detalle_cotizacion DC ON C.idCotizacion = DC.idCotizacion
                            WHERE (U.nombreUsuario LIKE @search OR S.nombreSucursal LIKE @search OR C.idCotizacion LIKE @search)
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
            string query = @"SELECT DC.idCotizacion AS 'Nro', S.nombreSucursal AS Sucursal, P.nombreProducto AS Producto, DC.cotizacionUSD AS 'Cotizacion USD', DC.cotizacionBOB AS 'Cotizacion BOB', C.fechaRegistro AS 'Fecha de Registro' FROM detalle_cotizacion DC
                            INNER JOIN producto P ON DC.idProducto = P.idProducto
                            INNER JOIN cotizacion C ON DC.idCotizacion = C.idCotizacion 
                            INNER JOIN sucursal S ON C.idSucursal = S.idSucursal
                            WHERE DC.idCotizacion = @idCotizacion
                            ORDER BY 1 DESC";
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
    }
}