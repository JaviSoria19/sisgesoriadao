﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using MySql.Data.MySqlClient;//MySql.Data
namespace sisgesoriadao.Implementation
{
    public class DataBase
    {
        public MySqlCommand CreateBasicCommand()
        {
            MySqlConnection connection = new MySqlConnection(Session.CadenaConexionBdD);
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            return command;
        }
        public MySqlCommand CreateBasicCommand(string query)
        {
            MySqlCommand command = CreateBasicCommand();
            command.CommandText = query;
            return command;
        }
        public DataTable ExecuteDataTableCommand(MySqlCommand command)
        {
            DataTable table = new DataTable();
            try
            {
                command.Connection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Connection.Close();
            }
        }
        public int ExecuteBasicCommand(MySqlCommand command)
        {
            try
            {
                command.Connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Connection.Close();
            }
        }
    }
}
