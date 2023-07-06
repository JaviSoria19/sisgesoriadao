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
    public class CategoriaImpl : DataBase, ICategoria
    {
        public int Insert(Categoria c)
        {
            string query = @"INSERT INTO Categoria (idUsuario,nombreCategoria,garantia) 
                            VALUES (@idUsuario,@nombreCategoria,@garantia)";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            command.Parameters.AddWithValue("@nombreCategoria", c.NombreCategoria);
            command.Parameters.AddWithValue("@garantia", c.Garantia);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Update(Categoria c)
        {
            string query = @"UPDATE Categoria SET 
                idUsuario=@idUsuario, nombreCategoria=@nombreCategoria, garantia=@garantia, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCategoria = @idCategoria";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCategoria", c.IdCategoria);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            command.Parameters.AddWithValue("@nombreCategoria", c.NombreCategoria);
            command.Parameters.AddWithValue("@garantia", c.Garantia);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int Delete(Categoria c)
        {
            string query = @"UPDATE Categoria SET estado = 0, idUsuario=@idUsuario, fechaActualizacion = CURRENT_TIMESTAMP WHERE idCategoria = @idCategoria";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCategoria", c.IdCategoria);
            command.Parameters.AddWithValue("@idUsuario", c.IdUsuario);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Categoria Get(byte Id)
        {
            Categoria c = null;
            string query = @"SELECT idCategoria, idUsuario, nombreCategoria, garantia, estado, fechaRegistro, IFNULL(fechaActualizacion,'-') FROM Categoria 
                            WHERE idCategoria=@idCategoria";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@idCategoria", Id);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    c = new Categoria(byte.Parse(dt.Rows[0][0].ToString()),     /*idCategoria*/
                        byte.Parse(dt.Rows[0][1].ToString()),                   /*idUsuario*/
                        dt.Rows[0][2].ToString(),                               /*nombreCategoria*/
                        byte.Parse(dt.Rows[0][3].ToString()),                   /*garantia*/

                        //Estado, FechaRegistro y FechaActualizacion.
                        byte.Parse(dt.Rows[0][4].ToString()),
                        DateTime.Parse(dt.Rows[0][5].ToString()), 
                        dt.Rows[0][6].ToString());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return c;
        }
        
        public DataTable Select()
        {
            string query = @"SELECT C.idCategoria AS ID, C.nombreCategoria AS Categoria, C.garantia AS Garantia,C.fechaRegistro AS 'Fecha de Registro', IFNULL(C.fechaActualizacion,'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM Categoria AS C
                                INNER JOIN usuario AS U ON C.idUsuario = U.idUsuario
                                WHERE C.estado = 1 ORDER BY 3 DESC, 2 ASC";
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
            string query = @"SELECT idCategoria, nombreCategoria FROM Categoria WHERE estado = 1";
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
            string query = @"SELECT C.idCategoria AS ID, C.nombreCategoria AS Categoria, C.garantia AS Garantia, C.fechaRegistro AS 'Fecha de Registro', IFNULL(C.fechaActualizacion,'-') AS 'Fecha de Actualizacion', U.nombreUsuario AS Usuario FROM Categoria AS C
                                INNER JOIN Usuario AS U ON C.idUsuario = U.idUsuario
                                WHERE (C.nombreCategoria LIKE @search OR C.garantia LIKE @search) 
                                AND C.estado = 1 AND C.fechaRegistro BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY 3 DESC, 2 ASC";
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

        public string SelectGroupConcatIDForComboBox()
        {
            string groupConcatIDs = null;
            string query = @"SELECT group_concat(idCategoria) AS idCategorias FROM Categoria WHERE estado = 1";
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
