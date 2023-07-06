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
    public class EmpleadoImpl : DataBase, IEmpleado
    {
        public int Insert(Empleado e)
        {
            string query = @"INSERT INTO Empleado (nombres,primerApellido,segundoApellido,numeroCelular,numeroCI) 
                            VALUES (@nombres,@primerApellido,@segundoApellido,@numeroCelular,@numeroCI)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombres", e.Nombres);
            command.Parameters.AddWithValue("@primerApellido", e.PrimerApellido);
            command.Parameters.AddWithValue("@segundoApellido", e.SegundoApellido);
            command.Parameters.AddWithValue("@numeroCelular", e.NumeroCelular);
            command.Parameters.AddWithValue("@numeroCI", e.NumeroCI);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Empleado e)
        {
            string query = @"UPDATE Empleado SET 
                nombres=@nombres, primerApellido=@primerApellido,
                segundoApellido=@segundoApellido, numeroCelular=@numeroCelular,
                numeroCI=@numeroCI, fechaActualizacion = CURRENT_TIMESTAMP WHERE idEmpleado = @idEmpleado";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idEmpleado", e.IdEmpleado);
            command.Parameters.AddWithValue("@nombres", e.Nombres);
            command.Parameters.AddWithValue("@primerApellido", e.PrimerApellido);
            command.Parameters.AddWithValue("@segundoApellido", e.SegundoApellido);
            command.Parameters.AddWithValue("@numeroCelular", e.NumeroCelular);
            command.Parameters.AddWithValue("@numeroCI", e.NumeroCI);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Empleado e)
        {
            string query = @"UPDATE Empleado SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idEmpleado = @idEmpleado";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idEmpleado", e.IdEmpleado);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Empleado Get(byte Id)
        {
            Empleado e = null;
            string query = @"SELECT idEmpleado, nombres, primerApellido, IFNULL(segundoApellido,'-'), numeroCelular, numeroCI , estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Empleado 
                            WHERE idEmpleado=@idEmpleado";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idEmpleado", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count>0)
                {
                    e = new Empleado(byte.Parse(dt.Rows[0][0].ToString()),
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
            return e;
        }
        public DataTable Select()
        {
            string query = @"SELECT idEmpleado as ID, nombres as Nombres, CONCAT(primerApellido,' ',IF(segundoApellido='-','',IFNULL(segundoApellido,''))) AS Apellidos,
                            numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM Empleado WHERE estado IN (1,2) ORDER BY 6 DESC";
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
            string query = @"SELECT idEmpleado as ID, nombres as Nombres, CONCAT(primerApellido,' ',IF(segundoApellido='-','',IFNULL(segundoApellido,''))) AS Apellidos,
                                numeroCelular AS Celular, numeroCI AS Carnet, fechaRegistro AS 'Fecha de Registro',
                                IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM bdventacelular.Empleado 
                                WHERE (nombres LIKE @search OR primerApellido LIKE @search OR segundoApellido LIKE @search OR numeroCelular LIKE @search OR numeroCI LIKE @search) 
                                AND estado IN (1,2) AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
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

        public DataTable SelectEmployeesWithoutUsers()
        {
            string query = @"SELECT idEmpleado as ID, CONCAT(nombres, ' ',primerApellido,' ',IF(segundoApellido='-','',IFNULL(segundoApellido,''))) AS Nombre FROM Empleado WHERE estado=1 ORDER BY 1 DESC";
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

        public int UpdateCreatedUser(Empleado e)
        {
            string query = @"UPDATE Empleado SET estado = 2 WHERE idEmpleado = @idEmpleado";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idEmpleado", e.IdEmpleado);
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
