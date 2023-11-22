using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class CondicionImpl : DataBase, ICondicion
    {
        public int Insert(Condicion c)
        {
            string query = @"INSERT INTO Condicion (idUsuario,nombreCondicion) 
                            VALUES (@idUsuario,@nombreCondicion)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            command.Parameters.AddWithValue("@nombreCondicion", c.NombreCondicion);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Condicion c)
        {
            string query = @"UPDATE Condicion SET 
                idUsuario=@idUsuario, nombreCondicion=@nombreCondicion, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCondicion = @idCondicion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCondicion", c.IdCondicion);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            command.Parameters.AddWithValue("@nombreCondicion", c.NombreCondicion);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Condicion c)
        {
            string query = @"UPDATE Condicion SET estado = 0, idUsuario=@idUsuario, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCondicion = @idCondicion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCondicion", c.IdCondicion);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Condicion Get(byte Id)
        {
            Condicion c = null;
            string query = @"SELECT idCondicion, idUsuario, nombreCondicion, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Condicion 
                            WHERE idCondicion=@idCondicion";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCondicion", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Condicion(byte.Parse(dt.Rows[0][0].ToString()),     /*idCondicion*/
                        byte.Parse(dt.Rows[0][1].ToString()),                   /*idUsuario*/
                        dt.Rows[0][2].ToString(),                               /*nombreCondicion*/

                        //Estado, FechaRegistro y FechaActualizacion.
                        byte.Parse(dt.Rows[0][3].ToString()),
                        DateTime.Parse(dt.Rows[0][4].ToString()),
                        dt.Rows[0][5].ToString());
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
            string query = @"SELECT C.idCondicion AS ID, C.nombreCondicion AS Condicion," + Session.FormatoFechaMySql("C.fechaRegistro") + @" AS 'Fecha de Registro', IFNULL(" + Session.FormatoFechaMySql("C.fechaActualizacion") + @",'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM Condicion AS C
                                INNER JOIN Usuario AS U ON C.idUsuario = U.idUsuario
                                WHERE C.estado = 1 ORDER BY 1 ASC";
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
            string query = @"SELECT C.idCondicion AS ID, C.nombreCondicion AS Condicion," + Session.FormatoFechaMySql("C.fechaRegistro") + @" AS 'Fecha de Registro', IFNULL(" + Session.FormatoFechaMySql("C.fechaActualizacion") + @",'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM Condicion AS C
                                INNER JOIN Usuario AS U ON C.idUsuario = U.idUsuario
                                WHERE (C.nombreCondicion LIKE @search) 
                                AND C.estado = 1 AND C.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 1 ASC";
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
        public DataTable SelectForComboBox()
        {
            string query = @"SELECT idCondicion, nombreCondicion FROM Condicion WHERE estado = 1";
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

        public string SelectGroupConcatIDForComboBox()
        {
            string groupConcatIDs = null;
            string query = @"SELECT group_concat(idCondicion) AS idCondiciones FROM Condicion WHERE estado = 1";
            MySqlCommand command = CreateBasicCommand(query);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    groupConcatIDs = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return groupConcatIDs;
        }
    }
}
