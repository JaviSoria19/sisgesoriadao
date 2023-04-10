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
            string query = @"INSERT INTO producto 
                                (idSucursal,idCategoria,idSublote,idUsuario,codigoSublote,nombreProducto,identificador,costoUSD,costoBOB,precioVentaUSD,precioVentaBOB,observaciones) 
                                VALUES 
                                (@idSucursal,@idCategoria,@idSublote,@idUsuario,@codigoSublote,@nombreProducto,@identificador,@costoUSD,@costoBOB,@precioVentaUSD,@precioVentaBOB,@observaciones)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@idSublote", p.IdSublote);
            command.Parameters.AddWithValue("@idUsuario", p.IdUsuario);

            command.Parameters.AddWithValue("@codigoSublote", p.CodigoSublote);
            command.Parameters.AddWithValue("@nombreProducto", p.NombreProducto);
            command.Parameters.AddWithValue("@identificador", p.Identificador);
            command.Parameters.AddWithValue("@costoUSD", p.CostoUSD);
            command.Parameters.AddWithValue("@costoBOB", p.CostoBOB);
            command.Parameters.AddWithValue("@precioVentaUSD", p.PrecioVentaUSD);
            command.Parameters.AddWithValue("@precioVentaBOB", p.PrecioVentaBOB);
            command.Parameters.AddWithValue("@observaciones", p.Observaciones);
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
                idSucursal=@idSucursal, idCategoria=@idCategoria, idSublote=@idSublote, idUsuario=@idUsuario,
                codigoSublote=@codigoSublote,nombreProducto=@nombreProducto, identificador=@identificador,
                costoUSD=@costoUSD, costoBOB=@costoBOB, precioVentaUSD=@precioVentaUSD, precioVentaBOB=@precioVentaBOB, observaciones=@observaciones,
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@idSublote", p.IdSublote);
            command.Parameters.AddWithValue("@idUsuario", p.IdUsuario);

            command.Parameters.AddWithValue("@codigoSublote", p.CodigoSublote);
            command.Parameters.AddWithValue("@nombreProducto", p.NombreProducto);
            command.Parameters.AddWithValue("@identificador", p.Identificador);
            command.Parameters.AddWithValue("@costoUSD", p.CostoUSD);
            command.Parameters.AddWithValue("@costoBOB", p.CostoBOB);
            command.Parameters.AddWithValue("@precioVentaUSD", p.PrecioVentaUSD);
            command.Parameters.AddWithValue("@precioVentaBOB", p.PrecioVentaBOB);
            command.Parameters.AddWithValue("@observaciones", p.Observaciones);
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
            string query = @"SELECT idProducto, idSucursal, idCategoria, idSublote, idUsuario,
                                codigoSublote, nombreProducto, identificador,
                                costoUSD, costoBOB, precioVentaUSD, precioVentaBOB, observaciones,
                                estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM producto
                                WHERE idProducto=@idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    p = new Producto(int.Parse(dt.Rows[0][0].ToString()),   /*idProducto*/
                        byte.Parse(dt.Rows[0][1].ToString()),               /*idSucursal*/
                        byte.Parse(dt.Rows[0][2].ToString()),               /*idCategoria*/
                        int.Parse(dt.Rows[0][3].ToString()),                /*idSublote*/
                        byte.Parse(dt.Rows[0][4].ToString()),               /*idUsuario*/
                        dt.Rows[0][5].ToString(),                           /*codigoSublote*/
                        dt.Rows[0][6].ToString(),                           /*nombreProducto*/
                        dt.Rows[0][7].ToString(),                           /*identificador*/
                        double.Parse(dt.Rows[0][8].ToString()),             /*costoUSD*/
                        double.Parse(dt.Rows[0][9].ToString()),             /*costoBOB*/
                        double.Parse(dt.Rows[0][10].ToString()),            /*precioVentaUSD*/
                        double.Parse(dt.Rows[0][11].ToString()),            /*precioVentaBOB*/
                        dt.Rows[0][12].ToString(),                          /*observaciones*/

                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][13].ToString()), 
                        DateTime.Parse(dt.Rows[0][14].ToString()),
                        dt.Rows[0][15].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return p;
        }
        public Producto GetByIdentifierOrCode(string CadenaBusqueda)
        {
            Producto p = null;
            string query = @"SELECT idProducto, idSucursal, idCategoria, idSublote, idUsuario,
                                codigoSublote, nombreProducto, identificador,
                                costoUSD, costoBOB, precioVentaUSD, precioVentaBOB, observaciones,
                                estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM producto
                                WHERE identificador = @search OR codigoSublote = @search";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@search", CadenaBusqueda);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    p = new Producto(int.Parse(dt.Rows[0][0].ToString()),   /*idProducto*/
                        byte.Parse(dt.Rows[0][1].ToString()),               /*idSucursal*/
                        byte.Parse(dt.Rows[0][2].ToString()),               /*idCategoria*/
                        int.Parse(dt.Rows[0][3].ToString()),                /*idSublote*/
                        byte.Parse(dt.Rows[0][4].ToString()),               /*idUsuario*/
                        dt.Rows[0][5].ToString(),                           /*codigoSublote*/
                        dt.Rows[0][6].ToString(),                           /*nombreProducto*/
                        dt.Rows[0][7].ToString(),                           /*identificador*/
                        double.Parse(dt.Rows[0][8].ToString()),             /*costoUSD*/
                        double.Parse(dt.Rows[0][9].ToString()),             /*costoBOB*/
                        double.Parse(dt.Rows[0][10].ToString()),            /*precioVentaUSD*/
                        double.Parse(dt.Rows[0][11].ToString()),            /*precioVentaBOB*/
                        dt.Rows[0][12].ToString(),                          /*observaciones*/

                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][13].ToString()), 
                        DateTime.Parse(dt.Rows[0][14].ToString()),
                        dt.Rows[0][15].ToString());
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C. USD', P.costoBOB AS 'C. Bs.', P.precioVentaUSD AS 'P. USD', P.precioVentaBOB AS 'P. BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C. USD', P.costoBOB AS 'C. Bs.', P.precioVentaUSD AS 'P. USD', P.precioVentaBOB AS 'P. BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR P.nombreProducto LIKE @search OR P.identificador LIKE @search)
                                AND P.estado = 1 AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 2 ASC, 3 ASC, 4 ASC";
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
        public DataTable SelectSoldProducts()
        {
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C. USD', P.costoBOB AS 'C. Bs.', P.precioVentaUSD AS 'P. USD', P.precioVentaBOB AS 'P. BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE P.estado = 2 ORDER BY 2 ASC, 3 ASC, 4 ASC";
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
        public DataTable SelectLikeSoldProducts(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin)
        {
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C. USD', P.costoBOB AS 'C. Bs.', P.precioVentaUSD AS 'P. USD', P.precioVentaBOB AS 'P. BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR P.nombreProducto LIKE @search OR P.identificador LIKE @search)
                                AND P.estado = 2 AND fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 2 ASC, 3 ASC, 4 ASC";
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
