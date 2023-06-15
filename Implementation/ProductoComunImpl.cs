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
    public class ProductoComunImpl : DataBase, IProductoComun
    {
        public int Insert(ProductoComun p)
        {
            string query = @"INSERT INTO producto_comun 
                                (nombreProductoComun,precioMinimo,precioSugerido) 
                                VALUES 
                                (@nombreProductoComun,@precioMinimo,@precioSugerido)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreProductoComun", p.NombreProductoComun);
            command.Parameters.AddWithValue("@precioMinimo", p.PrecioMinimo);
            command.Parameters.AddWithValue("@precioSugerido", p.PrecioSugerido);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(ProductoComun p)
        {
            string query = @"UPDATE producto_comun SET 
                nombreProductoComun=@nombreProductoComun, precioMinimo=@precioMinimo, precioSugerido=@precioSugerido,
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idProductoComun = @idProductoComun";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@nombreProductoComun", p.NombreProductoComun);
            command.Parameters.AddWithValue("@precioMinimo", p.PrecioMinimo);
            command.Parameters.AddWithValue("@precioSugerido", p.PrecioSugerido);
            command.Parameters.AddWithValue("@idProductoComun", p.IdProductoComun);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(ProductoComun p)
        {
            string query = @"UPDATE producto_comun SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProductoComun = @idProductoComun";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProductoComun", p.IdProductoComun);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ProductoComun Get(int Id)
        {
            ProductoComun p = null;
            string query = @"SELECT idProductoComun, nombreProductoComun, precioMinimo, precioSugerido,
                                estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM producto_comun
                                WHERE idProductoComun=@idProductoComun";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProductoComun", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    p = new ProductoComun(int.Parse(dt.Rows[0][0].ToString()),   /*idProductoComun*/
                        dt.Rows[0][1].ToString(),                           /*nombreProductoComun*/
                        double.Parse(dt.Rows[0][2].ToString()),               /*precioMinimo*/
                        double.Parse(dt.Rows[0][3].ToString()),                /*precioSugerido*/

                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()),
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return p;
        }
        public DataTable Select()
        {
            string query = @"SELECT idProductoComun AS ID, nombreProductoComun AS 'Nombre Producto', precioMinimo AS 'Precio Minimo', precioSugerido AS 'Precio Sugerido', fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') AS 'Fecha de Actualizacion' 
                            FROM producto_comun WHERE estado = 1 ORDER BY 2 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@SessionSucursal", Session.Sucursal_IdSucursal);
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
            string query = @"SELECT idProductoComun AS ID, nombreProductoComun AS 'Nombre Producto', precioMinimo AS 'Precio Minimo', precioSugerido AS 'Precio Sugerido', fechaRegistro AS 'Fecha de Registro', IFNULL(fechaActualizacion,'-') AS 'Fecha de Actualizacion' 
                            FROM producto_comun 
                            WHERE (nombreProductoComun LIKE @search OR precioMinimo LIKE @search OR precioSugerido LIKE @search) 
                            AND estado = 1 AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            ORDER BY 2 ASC";
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
