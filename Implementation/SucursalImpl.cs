using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class SucursalImpl : DataBase, ISucursal
    {
        public int Insert(Sucursal s)
        {
            string query = @"INSERT INTO Sucursal (nombreSucursal,direccion,correo,telefono) 
                            VALUES (@nombreSucursal,@direccion,@correo,@telefono)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreSucursal", s.NombreSucursal);
            command.Parameters.AddWithValue("@direccion", s.Direccion);
            command.Parameters.AddWithValue("@correo", s.Correo);
            command.Parameters.AddWithValue("@telefono", s.Telefono);
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
            string query = @"UPDATE Sucursal SET 
                nombreSucursal=@nombreSucursal, direccion=@direccion, correo=@correo, telefono=@telefono,
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idSucursal = @idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", s.IdSucursal);
            command.Parameters.AddWithValue("@nombreSucursal", s.NombreSucursal);
            command.Parameters.AddWithValue("@direccion", s.Direccion);
            command.Parameters.AddWithValue("@correo", s.Correo);
            command.Parameters.AddWithValue("@telefono", s.Telefono);
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
            string query = @"UPDATE Sucursal SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idSucursal = @idSucursal";
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
            string query = @"SELECT idSucursal, nombreSucursal, direccion, correo, telefono, estado, " + Session.FormatoFechaMySql("fechaRegistro") + @", IFNULL(fechaActualizacion,'-') FROM Sucursal 
                            WHERE idSucursal=@idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    s = new Sucursal(byte.Parse(dt.Rows[0][0].ToString()), /*idSucursal*/
                        dt.Rows[0][1].ToString(),   /*nombreSucursal*/
                        dt.Rows[0][2].ToString(),   /*direccion*/
                        dt.Rows[0][3].ToString(),   /*correo*/
                        dt.Rows[0][4].ToString(),   /*telefono*/

                        /*Estado, F. Registro & F. Actualizacion.*/
                        byte.Parse(dt.Rows[0][5].ToString()),
                        DateTime.Parse(dt.Rows[0][6].ToString()),
                        dt.Rows[0][7].ToString());
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
            string query = @"SELECT idSucursal as ID, nombreSucursal as Sucursal, direccion AS Direccion, correo AS Correo, telefono AS Telefono, " + Session.FormatoFechaMySql("fechaRegistro") + @" AS 'Fecha de Registro', IFNULL(" + Session.FormatoFechaMySql("fechaActualizacion") + @",'-') as 'Fecha de Actualizacion'
                                FROM Sucursal WHERE estado = 1 ORDER BY 2 ASC";
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
            string query = @"SELECT idSucursal, nombreSucursal FROM Sucursal WHERE estado = 1";
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
            string query = @"SELECT idSucursal as ID, nombreSucursal as Sucursal, direccion AS Direccion, correo AS Correo, telefono AS Telefono,
                                " + Session.FormatoFechaMySql("fechaRegistro") + @" AS 'Fecha de Registro', IFNULL(" + Session.FormatoFechaMySql("fechaActualizacion") + @",'-') AS 'Fecha de Actualizacion' FROM Sucursal
                                WHERE (nombreSucursal LIKE @search OR direccion LIKE @search OR correo LIKE @search OR telefono LIKE @search) 
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
            string query = @"SELECT nombreSucursal, direccion, correo, telefono FROM Sucursal 
                            WHERE idSucursal=@idSucursal";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", IdSucursal);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    Session.Sucursal_NombreSucursal = dt.Rows[0][0].ToString();
                    Session.Sucursal_Direccion = dt.Rows[0][1].ToString();
                    Session.Sucursal_Correo = dt.Rows[0][2].ToString();
                    Session.Sucursal_Telefono = dt.Rows[0][3].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string SelectGroupConcatIDForComboBox()
        {
            string groupConcatIDs = null;
            string query = @"SELECT group_concat(idSucursal) AS idSucursales FROM Sucursal WHERE estado = 1";
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
