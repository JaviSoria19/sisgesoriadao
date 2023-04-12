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
        public string InsertTransaction(List<Producto> ListaProductos, int idLote)
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlTransaction myTrans;
            myTrans = connection.BeginTransaction();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = myTrans;
            try
            {
                //AÑADIENDO NUEVO SUBLOTE PARA EL SIGUIENTE LOTE.
                command.CommandText = @"INSERT INTO sublote (idLote) VALUES (@idLote);";
                command.Parameters.AddWithValue("@idLote", idLote);
                command.ExecuteNonQuery();
                foreach (var producto in ListaProductos)
                {
                    //LIMPIEZA DE PARÁMETROS YA UTILIZADOS EN EL CICLO ANTERIOR PARA PROSEGUIR, CASO CONTRARIO LANZA ERROR.
                    command.Parameters.Clear();
                    //REGISTRO DEL LOTE.
                    command.CommandText = @"INSERT INTO producto 
                                (idSucursal,idCategoria,idSublote,idUsuario,codigoSublote,nombreProducto,identificador,costoUSD,costoBOB,precioVentaUSD,precioVentaBOB,observaciones) 
                                VALUES 
                                (@idSucursal,@idCategoria,@idSublote,@idUsuario,@codigoSublote,@nombreProducto,@identificador,@costoUSD,@costoBOB,@precioVentaUSD,@precioVentaBOB,@observaciones)";
                    command.Parameters.AddWithValue("@idSucursal", producto.IdSucursal);
                    command.Parameters.AddWithValue("@idCategoria", producto.IdCategoria);
                    command.Parameters.AddWithValue("@idSublote", producto.IdSublote);
                    command.Parameters.AddWithValue("@idUsuario", producto.IdUsuario);
                    command.Parameters.AddWithValue("@codigoSublote", producto.CodigoSublote);
                    command.Parameters.AddWithValue("@nombreProducto", producto.NombreProducto);
                    command.Parameters.AddWithValue("@identificador", producto.Identificador);
                    command.Parameters.AddWithValue("@costoUSD", producto.CostoUSD);
                    command.Parameters.AddWithValue("@costoBOB", producto.CostoBOB);
                    command.Parameters.AddWithValue("@precioVentaUSD", producto.PrecioVentaUSD);
                    command.Parameters.AddWithValue("@precioVentaBOB", producto.PrecioVentaBOB);
                    command.Parameters.AddWithValue("@observaciones", producto.Observaciones);
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO historial (idProducto,detalle) VALUES
                                ((SELECT MAX(idProducto) FROM producto),'PRODUCTO INGRESADO POR EL USUARIO " + Session.NombreUsuario + " EN SUCURSAL " + Session.Sucursal_NombreSucursal + "')";
                    command.ExecuteNonQuery();
                }
                //command.CommandText = "Insert into mytable (id, desc) VALUES (101, 'Description')";
                //command.ExecuteNonQuery();
                myTrans.Commit();
                return "LOTE REGISTRADO EXITOSAMENTE.";
            }
            catch (Exception e)
            {
                try
                {
                    myTrans.Rollback();
                }
                catch (MySqlException ex)
                {
                    if (myTrans.Connection != null)
                    {
                        return "Una excepción del tipo " + ex.GetType() + " se encontró mientras se estaba intentando revertir la transacción.";
                    }
                }
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE P.estado = 1 ORDER BY 2 ASC, 5 ASC, 4 ASC";
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR P.nombreProducto LIKE @search OR P.identificador LIKE @search)
                                AND P.estado = 1 AND P.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 2 ASC, 5 ASC, 4 ASC";
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
                                WHERE P.estado = 2 ORDER BY 2 ASC, 5 ASC, 4 ASC";
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
                                AND P.estado = 2 AND P.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 2 ASC, 5 ASC, 4 ASC";
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
        public DataTable SelectBatchForComboBox()
        {
            string query = @"SELECT idLote,codigoLote FROM lote WHERE estado = 1 ORDER BY idLote DESC";
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
        public string GetCodeFormatToInsertProducts(int IdLote)
        {
            string codigoSublote = null;
            string query = @"SELECT CONCAT(L.codigoLote, L.idLote,'-',COUNT(S.idSublote)) AS Codigo FROM lote AS L
                                INNER JOIN sublote AS S ON L.idLote = S.idLote 
                                WHERE L.idLote = @idLote";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idLote", IdLote);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    codigoSublote = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return codigoSublote;
        }

        public int GetSubBatchToInsertProducts(int idLote)
        {
            int idSublote = 0;
            string query = @"SELECT MAX(idSublote) FROM sublote WHERE idLote = @idLote";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idLote", idLote);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    idSublote = int.Parse(dt.Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return idSublote;
        }
    }
}
