﻿using System;
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
    public class VentaImpl : DataBase, IVenta
    {
        public string InsertTransaction(Venta venta, List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<Categoria> ListaGarantias, List<MetodoPago> ListaMetodosPago, Cliente cliente  )
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
                command.CommandText = @"INSERT INTO venta (idCliente,idUsuario,idSucursal,totalUSD,totalBOB,saldoUSD,saldoBOB,observaciones) 
                            VALUES(@idCliente,@idUsuario,@idSucursal,@totalUSD,@totalBOB,@saldoUSD,@saldoBOB,@observaciones)";
                command.Parameters.AddWithValue("@idCliente", venta.IdCliente);
                command.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);
                command.Parameters.AddWithValue("@idSucursal", venta.IdSucursal);
                command.Parameters.AddWithValue("@totalUSD", venta.TotalUSD);
                command.Parameters.AddWithValue("@totalBOB", venta.TotalBOB);
                command.Parameters.AddWithValue("@saldoUSD", venta.SaldoUSD);
                command.Parameters.AddWithValue("@saldoBOB", venta.SaldoBOB);
                command.Parameters.AddWithValue("@observaciones", venta.Observaciones);
                command.ExecuteNonQuery();
                //REGISTRO DE LOS PRODUCTOS, ACTUALIZACIÓN DEL ESTADO DE LOS PRODUCTOS E INSERCIÓN EN HISTORIAL.
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    command.CommandText = @"INSERT INTO detalle_venta (idVenta,idProducto,cantidad,precioUSD,precioBOB,descuento,garantia)
                            VALUES((SELECT MAX(idVenta) FROM venta),@idProducto,@cantidad,@precioUSD,@precioBOB,@descuento,@garantia)";
                    command.Parameters.AddWithValue("@idProducto", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@cantidad", 1);
                    command.Parameters.AddWithValue("@precioUSD", ListaProductos[i].PrecioVentaUSD);
                    command.Parameters.AddWithValue("@precioBOB", ListaProductos[i].PrecioVentaBOB);
                    command.Parameters.AddWithValue("@descuento", ListaDescuentosPorcentaje[i]);
                    command.Parameters.AddWithValue("@garantia", ListaGarantias[i].Garantia);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE producto SET estado = 2, fechaActualizacion = CURRENT_TIMESTAMP WHERE idProducto = @idProductoTwice";
                    command.Parameters.AddWithValue("@idProductoTwice", ListaProductos[i].IdProducto);
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO historial (idProducto,detalle) VALUES
                                (@idProductoThree,@detalle)";
                    command.Parameters.AddWithValue("@idProductoThree", ListaProductos[i].IdProducto);
                    command.Parameters.AddWithValue("@detalle", "PRODUCTO VENDIDO POR EL USUARIO: " + Session.NombreUsuario + " AL CLIENTE: " + cliente.Nombre);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
                if (ListaMetodosPago.Count > 0)
                {
                    foreach (var item in ListaMetodosPago)
                    {
                        //REGISTRO DEL DETALLE DE VENTA.
                        command.CommandText = @"INSERT INTO metodo_pago (idVenta,montoUSD,montoBOB,tipo)
                            VALUES((SELECT MAX(idVenta) FROM venta),@montoUSD,@montoBOB,@tipo)";
                        command.Parameters.AddWithValue("@montoUSD", item.MontoUSD);
                        command.Parameters.AddWithValue("@montoBOB", item.MontoBOB);
                        command.Parameters.AddWithValue("@tipo", item.Tipo);
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();
                    }
                }
                command.CommandText = @"INSERT INTO detalle_caja (idVenta,idCaja)
                            VALUES((SELECT MAX(idVenta) FROM venta),(SELECT idCaja FROM caja WHERE idSucursal = @Session_idSucursal))";
                command.Parameters.AddWithValue("@Session_idSucursal", Session.Sucursal_IdSucursal);
                command.ExecuteNonQuery();
                //command.CommandText = "Insert into mytable (id, desc) VALUES (101, 'Description')";
                //command.ExecuteNonQuery();

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

        public int Update(Venta o)
        {
            throw new NotImplementedException();
        }
        public int Delete(Venta o)
        {
            throw new NotImplementedException();
        }

        public int Insert(Venta o)
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
