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
    public class SucursalImpl : DataBase, ISucursal
    {
        public int Insert(Sucursal s)
        {
            string query = @"INSERT INTO sucursal (nombreSucursal,direccion,correo) 
                            VALUES (@nombreSucursal,@direccion,@correo)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreSucursal", s.NombreSucursal);
            command.Parameters.AddWithValue("@direccion", s.Direccion);
            command.Parameters.AddWithValue("@correo", s.Correo);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Sucursal s)
        {
            string query = @"UPDATE sucursal SET 
                nombreSucursal=@nombreSucursal, direccion=@direccion, correo=@correo, fechaActualizacion = CURRENT_TIMESTAMP WHERE idSucursal = @idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", s.IdSucursal);
            command.Parameters.AddWithValue("@nombreSucursal", s.NombreSucursal);
            command.Parameters.AddWithValue("@direccion", s.Direccion);
            command.Parameters.AddWithValue("@correo", s.Correo);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Sucursal s)
        {
            string query = @"UPDATE sucursal SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idSucursal = @idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", s.IdSucursal);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Sucursal Get(byte Id)
        {
            Sucursal s = null;
            string query = @"SELECT idSucursal, nombreSucursal, direccion, correo , estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM sucursal 
                            WHERE idSucursal=@idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    s = new Sucursal(byte.Parse(dt.Rows[0][0].ToString()),
                        dt.Rows[0][1].ToString(),
                        dt.Rows[0][2].ToString(),
                        dt.Rows[0][3].ToString(),
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()), 
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return s;
        }
        public DataTable Select()
        {
            string query = @"SELECT idSucursal as ID, nombreSucursal as Sucursal, direccion AS Direccion, correo AS Correo, fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM sucursal WHERE estado = 1 ORDER BY 5 DESC";
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
            string query = @"SELECT idSucursal, nombreSucursal FROM sucursal WHERE estado = 1";
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
            string query = @"SELECT idSucursal as ID, nombreSucursal as Sucursal, direccion AS Direccion, correo AS Correo, 
                                fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') as 'Fecha de Actualizacion' FROM sucursal
                                WHERE (nombreSucursal LIKE @search OR direccion LIKE @search OR correo LIKE @search) 
                                AND estado = 1 AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 5 DESC";
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

        public void GetBranchForSession(byte IdSucursal)
        {
            string query = @"SELECT nombreSucursal, direccion, correo FROM sucursal 
                            WHERE idSucursal=@idSucursal AND estado = 1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    Session.Sucursal_NombreSucursal = dt.Rows[0][0].ToString();
                    Session.Sucursal_Direccion = dt.Rows[0][1].ToString();
                    Session.Sucursal_correo = dt.Rows[0][2].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
