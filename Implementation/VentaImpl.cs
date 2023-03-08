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
        string connectionString = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
        public string InsertTransaction(List<Producto> ListaProductos, Venta venta)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
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
                command.CommandText = @"INSERT INTO venta (idCliente,idUsuario,total,metodoPago,moneda,idSucursal) 
                            VALUES(@idCliente, @idUsuario, @total, @metodoPago, @moneda, @idSucursal)";
                command.Parameters.AddWithValue("@idCliente", venta.IdCliente);
                command.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                command.Parameters.AddWithValue("@total", venta.Total);
                command.Parameters.AddWithValue("@metodoPago", venta.MetodoPago);
                command.Parameters.AddWithValue("@moneda", venta.Moneda);
                command.Parameters.AddWithValue("@idSucursal", venta.IdSucursal);
                command.ExecuteNonQuery();

                foreach (var item in ListaProductos)
                {
                    //REGISTRO DEL DETALLE DE VENTA.
                    command.CommandText = @"INSERT INTO detalle_venta (idVenta,idProducto,precio,descuento,cantidad)
                            VALUES((SELECT MAX(idVenta) FROM venta), " + item.IdProducto + ", " + item.Precio + ", (SELECT precio - " + item.Precio + " FROM producto WHERE idProducto = " + item.IdProducto + ") , 1)";
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE producto SET estado = 2, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = " + item.IdProducto;
                    command.ExecuteNonQuery();
                }
                //command.CommandText = "Insert into mytable (id, desc) VALUES (101, 'Description')";
                //command.ExecuteNonQuery();
                myTrans.Commit();
                return "LA VENTA SE REGISTRÓ CON ÉXITO.";
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
    }
}
