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
    public class ProductoImpl : DataBase, IProducto
    {
        public int Insert(Producto p)
        {
            string query = @"INSERT INTO producto (idSucursal,idMarca,idCategoria,nombreProducto,color,numeroSerie,precio,moneda,idUsuario) 
                                            VALUES (@idSucursal,@idMarca,@idCategoria,@nombreProducto,@color,@numeroSerie,@precio,@moneda,@idUsuario)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idMarca", p.IdMarca);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@nombreProducto", p.NombreProducto);
            command.Parameters.AddWithValue("@color", p.Color);
            command.Parameters.AddWithValue("@numeroSerie", p.NumeroSerie);
            command.Parameters.AddWithValue("@precio", p.Precio);
            command.Parameters.AddWithValue("@moneda", p.Moneda);
            command.Parameters.AddWithValue("@idUsuario", p.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Producto p)
        {
            string query = @"UPDATE producto SET 
                idSucursal=@idSucursal, idMarca=@idMarca, idCategoria=@idCategoria, 
                nombreProducto=@nombreProducto, color=@color, numeroSerie=@numeroSerie,
                precio=@precio, moneda=@moneda, idUsuario=@idUsuario,
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", p.IdProducto);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idMarca", p.IdMarca);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@nombreProducto", p.NombreProducto);
            command.Parameters.AddWithValue("@color", p.Color);
            command.Parameters.AddWithValue("@numeroSerie", p.NumeroSerie);
            command.Parameters.AddWithValue("@precio", p.Precio);
            command.Parameters.AddWithValue("@moneda", p.Moneda);
            command.Parameters.AddWithValue("@idUsuario", p.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Producto p)
        {
            string query = @"UPDATE producto SET estado = 0, idUsuario = @idUsuario, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", p.IdProducto);
            command.Parameters.AddWithValue("@idUsuario", p.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Producto Get(int Id)
        {
            Producto p = null;
            string query = @"SELECT idProducto, idSucursal, idMarca, idCategoria, nombreProducto, color, numeroSerie, precio, moneda, idUsuario, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM producto 
                            WHERE idProducto=@idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    p = new Producto(int.Parse(dt.Rows[0][0].ToString()),//id.
                        byte.Parse(dt.Rows[0][1].ToString()),// id sucursal.
                        byte.Parse(dt.Rows[0][2].ToString()), //id marca.
                        byte.Parse(dt.Rows[0][3].ToString()), //id categoria.
                        dt.Rows[0][4].ToString(), //nombre.
                        dt.Rows[0][5].ToString(), //color.
                        dt.Rows[0][6].ToString(), //sn o imei.
                        double.Parse(dt.Rows[0][7].ToString()), //precio.
                        dt.Rows[0][8].ToString(), //moneda.
                        byte.Parse(dt.Rows[0][9].ToString()), //id usuario (session).

                        byte.Parse(dt.Rows[0][10].ToString()), //estado, f. registro & f. actualización.
                        DateTime.Parse(dt.Rows[0][11].ToString()),
                        dt.Rows[0][12].ToString());
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, M.nombreMarca AS Marca, P.nombreProducto AS Modelo, P.color AS Color, P.numeroSerie AS 'SN o IMEI', P.precio AS Precio, P.moneda AS Moneda, P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN marca AS M ON P.idMarca = M.idMarca
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE P.estado = 1 ORDER BY 10 ASC, 2 ASC, 3 ASC";
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, M.nombreMarca AS Marca, P.nombreProducto AS Modelo, P.color AS Color, P.numeroSerie AS 'SN o IMEI', P.precio AS Precio, P.moneda AS Moneda, P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN marca AS M ON P.idMarca = M.idMarca
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR M.nombreMarca LIKE @search OR P.nombreProducto LIKE @search OR P.color LIKE @search OR P.numeroSerie LIKE @search OR P.precio LIKE @search OR P.moneda LIKE @search)
                                AND P.estado = 1 ORDER BY 10 ASC, 2 ASC, 3 ASC;";
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
