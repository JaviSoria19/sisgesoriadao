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
    public class ClienteImpl : DataBase, ICliente
    {
        public int Insert(Cliente c)
        {
            string query = @"INSERT INTO cliente (nombres,primerApellido,segundoApellido,numeroCelular,numeroCI) 
                            VALUES (@nombres,@primerApellido,@segundoApellido,@numeroCelular,@numeroCI)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombres", c.Nombres);
            command.Parameters.AddWithValue("@primerApellido", c.PrimerApellido);
            command.Parameters.AddWithValue("@segundoApellido", c.SegundoApellido);
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
            string query = @"UPDATE cliente SET 
                nombres=@nombres, primerApellido=@primerApellido,
                segundoApellido=@segundoApellido, numeroCelular=@numeroCelular,
                numeroCI=@numeroCI, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCliente = @idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", c.IdCliente);
            command.Parameters.AddWithValue("@nombres", c.Nombres);
            command.Parameters.AddWithValue("@primerApellido", c.PrimerApellido);
            command.Parameters.AddWithValue("@segundoApellido", c.SegundoApellido);
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
            string query = @"UPDATE cliente SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCliente = @idCliente";
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
            string query = @"SELECT idCliente, nombres, primerApellido, IFNULL(segundoApellido,'-'), numeroCelular, numeroCI , estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM cliente 
                            WHERE idCliente=@idCliente";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCliente", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Cliente(byte.Parse(dt.Rows[0][0].ToString()),
                        dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString(),
                        dt.Rows[0][3].ToString(), dt.Rows[0][4].ToString(),
                        dt.Rows[0][5].ToString(), byte.Parse(dt.Rows[0][6].ToString()),
                        DateTime.Parse(dt.Rows[0][7].ToString()), dt.Rows[0][8].ToString());
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
            string query = @"SELECT idCliente as ID, nombres as Nombres, CONCAT(primerApellido,' ',IF(segundoApellido='-','',IFNULL(segundoApellido,''))) AS Apellidos,
                            numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM cliente WHERE estado = 1 ORDER BY 6 DESC";
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
            string query = @"SELECT idCliente as ID, nombres as Nombres, CONCAT(primerApellido,' ',IF(segundoApellido='-','',IFNULL(segundoApellido,''))) AS Apellidos,
                                numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro',
                                IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM cliente 
                                WHERE (nombres LIKE @search OR primerApellido LIKE @search OR segundoApellido LIKE @search OR numeroCelular LIKE @search OR numeroCI LIKE @search) 
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
    }
}
