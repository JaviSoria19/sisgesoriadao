using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class ClienteImpl : DataBase, ICliente
    {
        public int Insert(Cliente c)
        {
            string query = @"INSERT INTO Cliente (nombre,numeroCelular,numeroCI) 
                            VALUES (@nombre,@numeroCelular,@numeroCI)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombre", c.Nombre);
            command.Parameters.AddWithValue("@numeroCelular", c.NumeroCelular);
            command.Parameters.AddWithValue("@numeroCI", c.NumeroCI);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Cliente c)
        {
            string query = @"UPDATE Cliente SET 
                nombre=@nombre, numeroCelular=@numeroCelular,
                numeroCI=@numeroCI, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCliente = @idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", c.IdCliente);
            command.Parameters.AddWithValue("@nombre", c.Nombre);
            command.Parameters.AddWithValue("@numeroCelular", c.NumeroCelular);
            command.Parameters.AddWithValue("@numeroCI", c.NumeroCI);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Cliente c)
        {
            string query = @"UPDATE Cliente SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCliente = @idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", c.IdCliente);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Cliente Get(int Id)
        {
            Cliente c = null;
            string query = @"SELECT idCliente, nombre, numeroCelular, numeroCI , estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Cliente 
                            WHERE idCliente=@idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cliente(int.Parse(dt.Rows[0][0].ToString()),   /*idCliente*/
                        dt.Rows[0][1].ToString(),                           /*nombre*/
                        dt.Rows[0][2].ToString(),                           /*numeroCelular*/
                        dt.Rows[0][3].ToString(),                           /*numeroCI*/

                        /*Estado, Fecha de Registro, Fecha de Actualización.*/
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()),
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }
        public Cliente GetByCIorCelular(string CadenaBusqueda)
        {
            Cliente c = null;
            string query = @"SELECT idCliente, nombre, numeroCelular, numeroCI , estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Cliente 
                                WHERE (numeroCelular = @search OR numeroCI = @search) AND estado = 1;";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@search", CadenaBusqueda);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cliente(int.Parse(dt.Rows[0][0].ToString()),   /*idCliente*/
                        dt.Rows[0][1].ToString(),                           /*nombre*/
                        dt.Rows[0][2].ToString(),                           /*numeroCelular*/
                        dt.Rows[0][3].ToString(),                           /*numeroCI*/

                        /*Estado, Fecha de Registro, Fecha de Actualización.*/
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()),
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return c;
        }
        public DataTable Select()
        {
            string query = @"SELECT idCliente as ID, nombre as Nombre, numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' 
                                FROM Cliente WHERE estado = 1 ORDER BY 5 DESC";
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
        public DataTable SelectLike(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin)
        {
            string query = @"SELECT idCliente as ID, nombre as Nombre, numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion'
                                FROM Cliente WHERE (nombre LIKE @search OR numeroCelular LIKE @search OR numeroCI LIKE @search) 
                                AND estado = 1 AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 6 DESC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@search", "%" + CadenaBusqueda + "%");
            command.Parameters.AddWithValue("@FechaInicio", FechaInicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@FechaFin", FechaFin.ToString("yyyy-MM-dd") + " 23:59:59");
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelectCustomerNamesForComboBox()
        {
            string query = @"SELECT idCliente, nombre FROM Cliente";
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

        public Cliente GetFromSale(int idVenta)
        {
            Cliente c = null;
            string query = @"SELECT C.idCliente, C.nombre, C.numeroCelular, C.numeroCI , C.estado, C.fechaRegistro, IFNULL(C.fechaActualizacion,'-') FROM Cliente C
                             INNER JOIN Venta V ON V.idCliente = C.idCliente
                             WHERE V.idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idVenta", Session.IdVentaDetalle);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cliente(int.Parse(dt.Rows[0][0].ToString()),   /*idCliente*/
                        dt.Rows[0][1].ToString(),                           /*nombre*/
                        dt.Rows[0][2].ToString(),                           /*numeroCelular*/
                        dt.Rows[0][3].ToString(),                           /*numeroCI*/

                        /*Estado, Fecha de Registro, Fecha de Actualización.*/
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()),
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }

        public int UpdateSaleCustomer(Cliente c, int idVenta)
        {
            string query = @"UPDATE Venta SET idCliente = @idCliente, fechaActualizacion = CURRENT_TIMESTAMP WHERE idVenta = @idVenta";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", c.IdCliente);
            command.Parameters.AddWithValue("@idVenta", idVenta);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
