using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacation.DBaccess
{
    class DatabaseConnection
    {
        private MySqlConnection connection;
        private readonly ReadFile rf;
        public string SQLpassword = "";

        public DatabaseConnection()
        {
            rf = new ReadFile();
            SQLpassword = rf.ReadFileCreds("SQLpassword");
            //actual connection with server info
            string connectionString = $"Server=127.0.0.1;Database=Locations;User=root;Password={SQLpassword};";
            connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        public void OpenConnection()
        {
            connection.Open();
        }

        public void CloseConnection() { 
            connection.Close();
        }

        public MySqlDataReader ExecuteQuery(string query)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
                return null;
            }
        }

    }
}
