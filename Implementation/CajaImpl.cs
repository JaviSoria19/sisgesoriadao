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
    public class CajaImpl : DataBase, ICaja
    {
        public int Delete(Caja o)
        {
            throw new NotImplementedException();
        }
        public Caja Get(byte Id)
        {
            throw new NotImplementedException();
        }

        public Caja GetByBranch()
        {
            Caja c = null;
            string query = @"SELECT MAX(idCaja), idSucursal, idUsuario, estado, fechaRegistro FROM caja WHERE idSucursal = @idSucursal AND estado = 1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Caja(int.Parse(dt.Rows[0][0].ToString()),   /*idCaja*/
                        byte.Parse(dt.Rows[0][1].ToString()),           /*idSucursal*/
                        byte.Parse(dt.Rows[0][2].ToString()),           /*idUsuario*/
                        byte.Parse(dt.Rows[0][3].ToString()),           /*estado*/
                        DateTime.Parse(dt.Rows[0][4].ToString()));      /*fechaRegistro*/
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }

        public int Insert(Caja o)
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
        public DataTable SelectPendingCashFromBranch()
        {
            string query = @"SELECT U.nombreUsuario AS Usuario, CONCAT('Venta: ',V.idVenta,' (',GROUP_CONCAT('- ',P.nombreProducto SEPARATOR ' '),')') AS Detalle, IF(MP.tipo = 1,'Efectivo',IF(MP.tipo = 2,'Transferencia','Tarjeta')) AS Tipo, IFNULL(MP.montoUSD,0) AS '$us', IFNULL(MP.montoBOB,0) AS 'Bs', V.fechaRegistro AS 'Fecha de Registro' FROM caja C
                            INNER JOIN detalle_caja DC ON C.idCaja = DC.idCaja
                            INNER JOIN metodo_pago MP ON DC.idMetodoPago = MP.idMetodoPago
                            INNER JOIN venta V ON MP.idVenta = V.idVenta
                            INNER JOIN detalle_venta DV ON V.idVenta = DV.idVenta
                            INNER JOIN usuario U ON V.idUsuario = U.idUsuario
                            INNER JOIN producto P ON DV.idProducto = P.idProducto
                            WHERE C.idSucursal = @idSucursal AND C.idCaja = (SELECT MAX(idCaja) FROM caja WHERE idSucursal = @idSucursal)
                            GROUP BY V.idVenta, MP.tipo
                            ORDER BY 6 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int Update(Caja c)
        {
            throw new NotImplementedException();
        }

        public string UpdateClosePendingCashTransaction(Caja c)
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
                //INSERCION DE NUEVA TRANSFERENCIA.
                command.CommandText = @"UPDATE caja SET estado = 2, idUsuario = @idUsuario WHERE idCaja = @idCaja";
                command.Parameters.AddWithValue("@idUsuario", Session.IdUsuario);
                command.Parameters.AddWithValue("@idCaja", c.IdCaja);
                command.ExecuteNonQuery();

                command.CommandText = @"INSERT INTO caja (idSucursal,idUsuario) 
                                        VALUES (@idSucursal,@idUsuario_Twice)";
                command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
                command.Parameters.AddWithValue("@idUsuario_Twice", Session.IdUsuario);
                command.ExecuteNonQuery();
                myTrans.Commit();
                return "CAJA CERRADA EXITOSAMENTE.";
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
