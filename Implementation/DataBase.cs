using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Model;
using System;
using System.Data;
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
