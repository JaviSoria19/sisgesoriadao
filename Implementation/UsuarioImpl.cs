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
        public int Insert(Usuario o)
        {
            throw new NotImplementedException();
        }
        public int Update(Usuario o)
        {
            throw new NotImplementedException();
        }
        public int Delete(Usuario o)
        {
            throw new NotImplementedException();
        }
        public Usuario Get(byte Id)
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
