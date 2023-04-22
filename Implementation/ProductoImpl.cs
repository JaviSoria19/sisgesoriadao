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
                                (idSucursal,idCategoria,idSublote,idCondicion,idUsuario,codigoSublote,nombreProducto,identificador,costoUSD,costoBOB,precioVentaUSD,precioVentaBOB,observaciones) 
                                VALUES 
                                (@idSucursal,@idCategoria,@idSublote,@idCondicion,@idUsuario,@codigoSublote,@nombreProducto,@identificador,@costoUSD,@costoBOB,@precioVentaUSD,@precioVentaBOB,@observaciones)";
                    command.Parameters.AddWithValue("@idSucursal", producto.IdSucursal);
                    command.Parameters.AddWithValue("@idCategoria", producto.IdCategoria);
                    command.Parameters.AddWithValue("@idSublote", producto.IdSublote);
                    command.Parameters.AddWithValue("@idCondicion", producto.IdCondicion);
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
                                ((SELECT MAX(idProducto) FROM producto),'PRODUCTO INGRESADO POR EL USUARIO: " + Session.NombreUsuario + ", EN SUCURSAL: " + Session.Sucursal_NombreSucursal + "')";
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
                                (idSucursal,idCategoria,idSublote,idCondicion,idUsuario,codigoSublote,nombreProducto,identificador,costoUSD,costoBOB,precioVentaUSD,precioVentaBOB,observaciones) 
                                VALUES 
                                (@idSucursal,@idCategoria,@idSublote,@idCondicion,@idUsuario,@codigoSublote,@nombreProducto,@identificador,@costoUSD,@costoBOB,@precioVentaUSD,@precioVentaBOB,@observaciones)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@idSublote", p.IdSublote);
            command.Parameters.AddWithValue("@idCondicion", p.IdCondicion);
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
                idSucursal=@idSucursal, idCategoria=@idCategoria, idSublote=@idSublote, idCondicion=@idCondicion, idUsuario=@idUsuario,
                codigoSublote=@codigoSublote,nombreProducto=@nombreProducto, identificador=@identificador,
                costoUSD=@costoUSD, costoBOB=@costoBOB, precioVentaUSD=@precioVentaUSD, precioVentaBOB=@precioVentaBOB, observaciones=@observaciones,
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProducto";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", p.IdProducto);
            command.Parameters.AddWithValue("@idSucursal", p.IdSucursal);
            command.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            command.Parameters.AddWithValue("@idSublote", p.IdSublote);
            command.Parameters.AddWithValue("@idCondicion", p.IdCondicion);
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
            string query = @"SELECT idProducto, idSucursal, idCategoria, idSublote, idCondicion, idUsuario,
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
                        byte.Parse(dt.Rows[0][4].ToString()),               /*idCondicion*/
                        byte.Parse(dt.Rows[0][5].ToString()),               /*idUsuario*/
                        dt.Rows[0][6].ToString(),                           /*codigoSublote*/
                        dt.Rows[0][7].ToString(),                           /*nombreProducto*/
                        dt.Rows[0][8].ToString(),                           /*identificador*/
                        double.Parse(dt.Rows[0][9].ToString()),             /*costoUSD*/
                        double.Parse(dt.Rows[0][10].ToString()),            /*costoBOB*/
                        double.Parse(dt.Rows[0][11].ToString()),            /*precioVentaUSD*/
                        double.Parse(dt.Rows[0][12].ToString()),            /*precioVentaBOB*/
                        dt.Rows[0][13].ToString(),                          /*observaciones*/

                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][14].ToString()), 
                        DateTime.Parse(dt.Rows[0][15].ToString()),
                        dt.Rows[0][16].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return p;
        }
        public Producto GetByCode(string CadenaBusqueda)
        {
            Producto p = null;
            string query = @"SELECT idProducto, idSucursal, idCategoria, idSublote, idCondicion, idUsuario,
                                codigoSublote, nombreProducto, identificador,
                                costoUSD, costoBOB, precioVentaUSD, precioVentaBOB, observaciones,
                                estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM producto
                                WHERE codigoSublote = @search";
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
                        byte.Parse(dt.Rows[0][4].ToString()),                /*idCondicion*/
                        byte.Parse(dt.Rows[0][5].ToString()),               /*idUsuario*/
                        dt.Rows[0][6].ToString(),                           /*codigoSublote*/
                        dt.Rows[0][7].ToString(),                           /*nombreProducto*/
                        dt.Rows[0][8].ToString(),                           /*identificador*/
                        double.Parse(dt.Rows[0][9].ToString()),             /*costoUSD*/
                        double.Parse(dt.Rows[0][10].ToString()),             /*costoBOB*/
                        double.Parse(dt.Rows[0][11].ToString()),            /*precioVentaUSD*/
                        double.Parse(dt.Rows[0][12].ToString()),            /*precioVentaBOB*/
                        dt.Rows[0][13].ToString(),                          /*observaciones*/

                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][14].ToString()), 
                        DateTime.Parse(dt.Rows[0][15].ToString()),
                        dt.Rows[0][16].ToString());
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, CC.nombreCondicion AS Condicion, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                INNER JOIN condicion AS CC ON P.idCondicion = CC.idCondicion
                                WHERE P.estado = 1 ORDER BY 2 ASC, 6 ASC, 5 ASC";
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, CC.nombreCondicion AS Condicion, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                INNER JOIN condicion AS CC ON P.idCondicion = CC.idCondicion
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR P.nombreProducto LIKE @search OR P.identificador LIKE @search OR CC.nombreCondicion LIKE @search OR P.codigoSublote LIKE @search)
                                AND P.estado = 1 AND P.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 2 ASC, 6 ASC, 5 ASC";
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, CC.nombreCondicion AS Condicion, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                INNER JOIN condicion AS CC ON P.idCondicion = CC.idCondicion
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
            string query = @"SELECT P.idProducto AS ID, S.nombreSucursal AS Sucursal, C.nombreCategoria AS Categoria, CC.nombreCondicion AS Condicion, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones,P.fechaRegistro AS 'Fecha de Registro', IFNULL(P.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM producto AS P
                                INNER JOIN sucursal AS S ON P.idSucursal = S.idSucursal
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                INNER JOIN condicion AS CC ON P.idCondicion = CC.idCondicion
                                WHERE (S.nombreSucursal LIKE @search OR C.nombreCategoria LIKE @search OR P.nombreProducto LIKE @search OR P.identificador LIKE @search OR CC.nombreCondicion LIKE @search OR P.codigoSublote LIKE @search)
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
        public DataTable SelectProductHistory(string CadenaBusqueda)
        {
            string query = @"SELECT P.codigoSublote AS Codigo, P.nombreProducto AS 'Producto', P.identificador AS 'IMEI o SN', H.detalle AS Detalle, H.fechaRegistro AS 'Fecha de Registro' FROM historial AS H
                            INNER JOIN producto AS P ON H.idProducto = P.idProducto 
                            WHERE (P.identificador LIKE @search OR P.codigoSublote LIKE @search) ORDER BY 5 DESC, 1 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@search", CadenaBusqueda);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int InsertBatch(Lote l)
        {
            string query = @"INSERT INTO lote (idUsuario,codigoLote) VALUES (@idUsuario,@codigoLote);
                             INSERT INTO sublote (idLote) SELECT MAX(idLote) FROM lote";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", l.IdUsuario);
            command.Parameters.AddWithValue("@codigoLote", l.CodigoLote);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int UpdateBatch(Lote l)
        {
            string query = @"UPDATE lote SET 
                idUsuario=@idUsuario, codigoLote=@codigoLote, fechaActualizacion = CURRENT_TIMESTAMP WHERE idLote = @idLote";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", l.IdUsuario);
            command.Parameters.AddWithValue("@codigoLote", l.CodigoLote);
            command.Parameters.AddWithValue("@idLote", l.IdLote);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Lote GetBatch(int Id)
        {
            Lote l = null;
            string query = @"SELECT idLote, idUsuario, codigoLote, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM lote
                                WHERE idLote=@idLote";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idLote", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    l = new Lote(int.Parse(dt.Rows[0][0].ToString()),       /*idLote*/
                        byte.Parse(dt.Rows[0][1].ToString()),               /*idUsuario*/
                        dt.Rows[0][2].ToString(),                           /*codigoLote*/
                        /*estado, f. registro & f. actualización.*/
                        byte.Parse(dt.Rows[0][3].ToString()),
                        DateTime.Parse(dt.Rows[0][4].ToString()),
                        dt.Rows[0][ 5].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return l;
        }
        public DataTable SelectBatch()
        {
            string query = @"SELECT L.idLote AS ID, U.nombreUsuario AS Usuario, L.codigoLote AS 'Codigo', L.fechaRegistro AS 'Fecha de Registro', IFNULL(L.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM lote AS L
                                INNER JOIN usuario AS U ON L.idUsuario = U.idUsuario
                                WHERE L.estado = 1 ORDER BY 4 DESC";
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
        public DataTable SelectLikeBatch(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin)
        {
            string query = @"SELECT L.idLote AS ID, U.nombreUsuario AS Usuario, L.codigoLote AS 'Codigo', L.fechaRegistro AS 'Fecha de Registro', IFNULL(L.fechaActualizacion,'-') AS 'Fecha de Actualizacion' FROM lote AS L
                                INNER JOIN usuario AS U ON L.idUsuario = U.idUsuario
                                WHERE (U.nombreUsuario LIKE @search OR L.codigoLote LIKE @search)
                                AND L.estado = 1 AND L.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 4 DESC";
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
        public DataTable SelectProductNamesForComboBox()
        {
            string query = @"SELECT nombreProducto FROM PRODUCTO GROUP BY 1";
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

        public string UpdateBranchMovementTransaction(List<Producto> ListaProductos, byte IdSucursalDestino, string SucursalDestino)
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
                //INSERCION DE NUEVA TRANSFERENCIA.
                command.CommandText = @"INSERT INTO transferencia (sucursalOrigen,sucursalDestino)
                                    VALUES (@sucursalOrigen,@sucursalDestino)";
                command.Parameters.AddWithValue("@sucursalOrigen", Session.Sucursal_IdSucursal);
                command.Parameters.AddWithValue("@sucursalDestino", IdSucursalDestino);
                command.ExecuteNonQuery();

                foreach (var producto in ListaProductos)
                {
                    //INSERCIÓN DE DETALLE DE LA TRANSFERENCIA.
                    command.CommandText = @"INSERT INTO detalle_transferencia (idTransferencia,idProducto)
                                            VALUES((SELECT MAX(idTransferencia) FROM transferencia),@idProductoTransferido);";
                    command.Parameters.AddWithValue("@idProductoTransferido", producto.IdProducto);
                    command.ExecuteNonQuery();
                    //ACTUALIZACION DE LA SUCURSAL Y EL ESTADO.
                    command.CommandText = @"UPDATE producto SET idSucursal=@idSucursal, estado = 3 WHERE idProducto = @idProducto";
                    command.Parameters.AddWithValue("@idSucursal", IdSucursalDestino);
                    command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    command.ExecuteNonQuery();
                    //INSERCIÓN DE HISTORIAL DE PRODUCTO.
                    command.CommandText = @"INSERT INTO historial (idProducto,detalle) VALUES
                                (@idProductoTwo,@detalle)";
                    command.Parameters.AddWithValue("@idProductoTwo", producto.IdProducto);
                    command.Parameters.AddWithValue("@detalle", "PRODUCTO TRANSFERIDO A LA SUCURSAL: " + SucursalDestino + ", POR EL USUARIO: " + Session.NombreUsuario);
                    command.ExecuteNonQuery();
                    
                    //LIMPIEZA DE PARÁMETROS YA UTILIZADOS EN EL CICLO ANTERIOR PARA PROSEGUIR, CASO CONTRARIO LANZA ERROR.
                    command.Parameters.Clear();
                }
                //command.CommandText = "Insert into mytable (id, desc) VALUES (101, 'Description')";
                //command.ExecuteNonQuery();
                myTrans.Commit();
                return "PRODUCTOS TRANSFERIDOS EXITOSAMENTE.";
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

        public DataTable SelectPendingProducts()
        {
            string query = @"SELECT P.idProducto AS ID, C.nombreCategoria AS Categoria, CC.nombreCondicion AS Condicion, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'Identificador', P.costoUSD AS 'C USD', P.costoBOB AS 'C Bs', P.precioVentaUSD AS 'P USD', P.precioVentaBOB AS 'P BOB', P.observaciones AS Observaciones FROM producto AS P
                                INNER JOIN categoria AS C ON P.idCategoria = C.idCategoria
                                INNER JOIN condicion AS CC ON P.idCondicion = CC.idCondicion
                                WHERE P.estado = 3 AND idSucursal = @idSucursal ORDER BY 2 ASC, 4 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int UpdatePendingProduct(int IdProducto)
        {
            string query = @"UPDATE producto SET estado = 1 WHERE idProducto = @idProducto;
                            INSERT INTO historial (idProducto, detalle) VALUES
                            (@idProductoTwice,@detalle)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idProducto", IdProducto);
            command.Parameters.AddWithValue("@idProductoTwice", IdProducto);
            command.Parameters.AddWithValue("@detalle", "TRANSFERENCIA RECIBIDA POR EL USUARIO: " + Session.NombreUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SelectMovementsHistory()
        {
            string query = @"SELECT T.idTransferencia AS ID, S1.nombreSucursal AS 'Sucursal Origen', S2.nombreSucursal AS 'Sucursal Destino', COUNT(DT.idProducto) AS 'Productos transferidos', T.fechaRegistro AS 'Fecha de Registro' FROM transferencia AS T
                        INNER JOIN Sucursal S1 ON S1.idSucursal = T.sucursalOrigen
                        INNER JOIN Sucursal S2 ON S2.idSucursal = T.sucursalDestino
                        INNER JOIN Detalle_transferencia AS DT ON DT.idTransferencia = T.idTransferencia
                        GROUP BY T.idTransferencia
                        ORDER BY 1 DESC";
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
        public DataTable SelectLikeMovementsHistory(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin)
        {
            string query = @"SELECT T.idTransferencia AS ID, S1.nombreSucursal AS 'Sucursal Origen', S2.nombreSucursal AS 'Sucursal Destino', COUNT(DT.idProducto) AS 'Productos transferidos', T.fechaRegistro AS 'Fecha de Registro' FROM transferencia AS T
                        INNER JOIN Sucursal S1 ON S1.idSucursal = T.sucursalOrigen
                        INNER JOIN Sucursal S2 ON S2.idSucursal = T.sucursalDestino
                        INNER JOIN Detalle_transferencia AS DT ON DT.idTransferencia = T.idTransferencia
                        WHERE (S1.nombreSucursal LIKE @search OR S2.nombreSucursal LIKE @search)
                        AND T.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                        GROUP BY T.idTransferencia
                        ORDER BY 1 DESC";
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
        public DataTable SelectMovementsHistory_Details(int IdTransferencia)
        {
            string query = @"SELECT DT.idTransferencia, P.codigoSublote AS Codigo, P.nombreProducto AS Producto, P.identificador AS 'IMEI o SN', S1.nombreSucursal AS 'Sucursal Origen', S2.nombreSucursal AS 'Sucursal Destino' FROM detalle_transferencia AS DT
                            INNER JOIN Producto P ON P.idProducto = DT.idProducto
                            INNER JOIN Transferencia T ON T.idTransferencia = DT.idTransferencia
                            INNER JOIN Sucursal S1 ON S1.idSucursal = T.sucursalOrigen
							INNER JOIN Sucursal S2 ON S2.idSucursal = T.sucursalDestino
                            WHERE DT.idTransferencia = @idTransferencia
                            ORDER BY 2 ASC";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idTransferencia", IdTransferencia);
            try
            {
                return ExecuteDataTableCommand(command);
            }
            catch (Exception)
            {

                throw;
            }
        }
<<<<<<< HEAD
<<<<<<< HEAD
=======

>>>>>>> 186ced3ae7a536bb98b1c5c744b781d7fd732b66
=======

>>>>>>> 186ced3ae7a536bb98b1c5c744b781d7fd732b66
        public DataTable SelectProductIDandNamesForAutoCompleteBox()
        {
            string query = @"SELECT idProducto, nombreProducto FROM PRODUCTO WHERE estado = 1 GROUP BY 2";
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
