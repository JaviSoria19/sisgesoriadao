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
            throw new NotImplementedException();
        }
        public int Insert(Cotizacion c)
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
        public int Update(Cotizacion c)
        {
            throw new NotImplementedException();
        }
    }
}
