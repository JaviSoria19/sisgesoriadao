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
    public class UsuarioImpl : DataBase, IUsuario
    {
        public Usuario Login(string nombreUsuario, string contrasenha)
        {
            Usuario session = null;
            string query = @"SELECT idUsuario, nombreUsuario, rol FROM usuario 
                            WHERE nombreUsuario=@nombreUsuario AND contrasenha=MD5(@contrasenha) AND estado=1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            command.Parameters.AddWithValue("@contrasenha", contrasenha);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    session = new Usuario(byte.Parse(dt.Rows[0][0].ToString()),
                        dt.Rows[0][1].ToString(), byte.Parse(dt.Rows[0][2].ToString()));
                    Session.IdUsuario = session.IdUsuario;
                    Session.NombreUsuario = session.NombreUsuario;
                    Session.Rol = session.Rol;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return session;
        }
        public Usuario SelectRecoverPasswordWithPin(string nombreUsuario, string pin)
        {
            Usuario u = null;
            string query = @"SELECT idUsuario, nombreUsuario, rol FROM usuario WHERE nombreUsuario = @nombreUsuario AND pin = @pin AND estado = 1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            command.Parameters.AddWithValue("@pin", pin);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    u = new Usuario(byte.Parse(dt.Rows[0][0].ToString()),
                        dt.Rows[0][1].ToString(), byte.Parse(dt.Rows[0][2].ToString()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return u;
        }
        public int UpdateRecoverPasswordWithPin(byte IdUsuario, string NuevaContrasenha)
        {
            string query = @"UPDATE usuario SET 
                contrasenha=MD5(@contrasenha), fechaActualizacion = CURRENT_TIMESTAMP WHERE idUsuario = @idUsuario";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", IdUsuario);
            command.Parameters.AddWithValue("@contrasenha", NuevaContrasenha);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Insert(Usuario u)
        {
            string query = @"INSERT INTO usuario (idEmpleado,idAjustes,nombreUsuario,contrasenha,rol,pin) 
                            VALUES (@idEmpleado,1,@nombreUsuario,MD5(@contrasenha),@rol,@pin)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idEmpleado", u.IdEmpleado);
            command.Parameters.AddWithValue("@nombreUsuario", u.NombreUsuario);
            command.Parameters.AddWithValue("@contrasenha", u.Contrasenha);
            command.Parameters.AddWithValue("@rol", u.Rol);
            command.Parameters.AddWithValue("@pin", u.Pin);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Usuario u)
        {
            string query = @"UPDATE usuario SET 
                nombreUsuario=@nombreUsuario, rol=@rol, pin=@pin, 
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idUsuario = @idUsuario";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", u.IdUsuario);
            command.Parameters.AddWithValue("@nombreUsuario", u.NombreUsuario);
            command.Parameters.AddWithValue("@rol", u.Rol);
            command.Parameters.AddWithValue("@pin", u.Pin);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Usuario u)
        {
            string query = @"UPDATE usuario SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idUsuario = @idUsuario;
                             UPDATE empleado SET estado = 1, fechaActualizacion = CURRENT_TIMESTAMP WHERE idEmpleado = @idEmpleado";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", u.IdUsuario);
            command.Parameters.AddWithValue("@idEmpleado", u.IdEmpleado);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Usuario Get(byte Id)
        {
            Usuario u = null;
            string query = @"SELECT idUsuario, idEmpleado, idAjustes, nombreUsuario, contrasenha, rol, pin, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM usuario 
                            WHERE idUsuario=@idUsuario";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    u = new Usuario(byte.Parse(dt.Rows[0][0].ToString()),   /*idUsuario*/
                        byte.Parse(dt.Rows[0][1].ToString()),               /*idEmpleado*/
                        byte.Parse(dt.Rows[0][2].ToString()),               /*idAjustes*/
                        dt.Rows[0][3].ToString(),                           /*nombreUsuario*/
                        dt.Rows[0][4].ToString(),                           /*contrasenha*/
                        byte.Parse(dt.Rows[0][5].ToString()),               /*rol*/
                        dt.Rows[0][6].ToString(),                           /*pin*/

                        /*Estado, F. Registro & F. Actualizacion.*/
                        byte.Parse(dt.Rows[0][7].ToString()),
                        DateTime.Parse(dt.Rows[0][8].ToString()),
                        dt.Rows[0][9].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return u;
        }

        public DataTable Select()
        {
            string query = @"SELECT u.idUsuario AS 'ID U', u.idEmpleado AS 'ID E' , CONCAT(e.nombres,' ', e.primerApellido,' ',IF(e.segundoApellido='-','',IFNULL(e.segundoApellido,''))) AS Empleado, u.nombreUsuario AS 'Usuario', IF(u.rol=1, 'ADMIN', IF(u.rol=2,'VENDEDOR','TERCER ROL')) AS Rol , u.pin AS Pin, u.fechaRegistro as 'Fecha de Registro', IFNULL(u.fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM usuario AS u
                                INNER JOIN empleado AS e ON u.idEmpleado = e.idEmpleado
                                WHERE u.estado = 1 ORDER BY 5,3 ASC";
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
            string query = @"SELECT u.idUsuario AS 'ID U', u.idEmpleado AS 'ID E' , CONCAT(e.nombres,' ', e.primerApellido,' ',IFNULL(e.segundoApellido,'')) AS Empleado, u.nombreUsuario AS 'Usuario', IF(u.rol=1, 'ADMIN', IF(u.rol=2,'VENDEDOR','TERCER ROL')) AS Rol , u.pin AS Pin, u.fechaRegistro as 'Fecha de Registro', IFNULL(u.fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM usuario AS u
                                INNER JOIN empleado AS e ON u.idEmpleado = e.idEmpleado
                                WHERE (e.nombres LIKE @search OR e.primerApellido LIKE @search OR e.segundoApellido LIKE @search OR u.nombreUsuario LIKE @search) 
                                AND u.estado = 1 AND u.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 5,3 ASC";
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
            string query = @"SELECT idUsuario,nombreUsuario FROM usuario WHERE estado = 1";
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
    }
}
