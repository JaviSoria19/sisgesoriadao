using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using MySql.Data.MySqlClient;
namespace sisgesoriadao.Implementation
{
    public class MarcaImpl : DataBase, IMarca
    {
        public int Insert(Marca m)
        {
            string query = @"INSERT INTO marca (nombreMarca,idUsuario) 
                            VALUES (@nombreMarca,@idUsuario)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreMarca", m.NombreMarca);
            command.Parameters.AddWithValue("@idUsuario", m.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Marca m)
        {
            string query = @"UPDATE marca SET 
                nombreMarca=@nombreMarca, idUsuario=@idUsuario, fechaActualizacion = CURRENT_TIMESTAMP WHERE idMarca = @idMarca";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idMarca", m.IdMarca);
            command.Parameters.AddWithValue("@nombreMarca", m.NombreMarca);
            command.Parameters.AddWithValue("@idUsuario", m.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Marca m)
        {
            string query = @"UPDATE marca SET estado = 0, idUsuario=@idUsuario, fechaActualizacion = CURRENT_TIMESTAMP WHERE idMarca = @idMarca";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idMarca", m.IdMarca);
            command.Parameters.AddWithValue("@idUsuario", m.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Marca Get(byte Id)
        {
            Marca m = null;
            string query = @"SELECT idMarca, nombreMarca, idUsuario, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM marca 
                            WHERE idMarca=@idMarca";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idMarca", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    m = new Marca(byte.Parse(dt.Rows[0][0].ToString()),
                        dt.Rows[0][1].ToString(),
                        byte.Parse(dt.Rows[0][2].ToString()),

                        byte.Parse(dt.Rows[0][3].ToString()),
                        DateTime.Parse(dt.Rows[0][4].ToString()),
                        dt.Rows[0][5].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return m;
        }        
        public DataTable Select()
        {
            string query = @"SELECT M.idMarca AS ID, M.nombreMarca AS Marca, M.fechaRegistro AS 'Fecha de Registro', IFNULL(M.fechaActualizacion,'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM marca AS M
                                INNER JOIN usuario AS U ON M.idUsuario = U.idUsuario
                                WHERE M.estado = 1 ORDER BY 3 DESC, 2 ASC";
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
        public DataTable SelectForComboBox()
        {
            string query = @"SELECT idMarca, nombreMarca FROM marca WHERE estado = 1";
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
            string query = @"SELECT M.idMarca AS ID, M.nombreMarca AS Marca, M.fechaRegistro AS 'Fecha de Registro', IFNULL(M.fechaActualizacion,'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM marca AS M
                                INNER JOIN usuario AS U ON M.idUsuario = U.idUsuario
                                WHERE (M.nombreMarca LIKE @search) 
                                AND M.estado = 1 AND M.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 3 DESC, 2 ASC";
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
