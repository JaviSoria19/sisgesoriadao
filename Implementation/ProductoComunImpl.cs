using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class ProductoComunImpl : DataBase, IProductoComun
    {
        public int Insert(ProductoComun p)
        {
            string query = @"INSERT INTO Producto_Comun 
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
            string query = @"UPDATE Producto_Comun SET 
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
            string query = @"UPDATE Producto_Comun SET estado = 0, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProductoComun = @idProductoComun";
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
                                estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Producto_Comun
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
                            FROM Producto_Comun WHERE estado = 1 ORDER BY 2 ASC";
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
                            FROM Producto_Comun 
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

        public DataTable SelectForComboBox()
        {
            string query = @"SELECT idProductoComun, nombreProductoComun FROM Producto_Comun WHERE estado = 1";
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

        public string InsertTransaction(List<ProductoComun> ListaProductosComunes, List<string> ListaDetalles, double Total)
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
                //REGISTRO DE LA VENTA.
                command.CommandText = @"INSERT INTO Venta_Comun (idUsuario,idSucursal,totalBOB) 
                            VALUES(@idUsuario,@idSucursal,@totalBOB)";
                command.Parameters.AddWithValue("@idUsuario", Session.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", Session.Sucursal_IdSucursal);
                command.Parameters.AddWithValue("@totalBOB", Total);
                command.ExecuteNonQuery();
                //REGISTRO DE LOS PRODUCTOS, ACTUALIZACIÓN DEL ESTADO DE LOS PRODUCTOS E INSERCIÓN EN HISTORIAL.
                for (int i = 0; i < ListaProductosComunes.Count; i++)
                {
                    command.CommandText = @"INSERT INTO Venta_Comun_detalle (idVentaComun,idProductoComun,precioBOB,detalle)
                            VALUES((SELECT MAX(idVentaComun) FROM Venta_Comun),@idProductoComun,@precioBOB,@detalle)";
                    command.Parameters.AddWithValue("@idProductoComun", ListaProductosComunes[i].IdProductoComun);
                    command.Parameters.AddWithValue("@precioBOB", ListaProductosComunes[i].PrecioSugerido);
                    command.Parameters.AddWithValue("@detalle", ListaDetalles[i]);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                myTrans.Commit();
                return "VENTA_EXITOSA";
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

        public DataTable SelectLikeCommonProductsSales(DateTime FechaInicio, DateTime FechaFin, string IdSucursales, string IdUsuarios)
        {
            string query = @"SELECT VC.idVentaComun AS ID, U.nombreUsuario AS Usuario, S.nombreSucursal AS Sucursal, CONCAT('Venta: ',VC.idVentaComun, ' ',GROUP_CONCAT('- ',PC.nombreProductoComun,' ', VCD.detalle, ' ',VCD.precioBOB SEPARATOR ' \n')) AS Detalle, VC.totalBOB AS 'Total Bs', VC.fechaRegistro AS 'Fecha' FROM Venta_Comun VC
                            INNER JOIN Usuario U ON U.idUsuario = VC.idUsuario
                            INNER JOIN Sucursal S ON S.idSucursal = VC.idSucursal
                            INNER JOIN Venta_Comun_detalle VCD ON VCD.idVentaComun = VC.idVentaComun
                            INNER JOIN Producto_Comun PC ON PC.idProductoComun = VCD.idProductoComun
                            WHERE VC.estado = 1 AND VC.idSucursal IN (" + IdSucursales + ") AND VC.idUsuario IN (" + IdUsuarios + @")
                            AND VC.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                            GROUP BY VC.idVentaComun
                            ORDER BY 1 DESC";
            MySqlCommand command = CreateBasicCommand(query);
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
