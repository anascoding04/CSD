using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;
using MySql.Data.MySqlClient;

namespace WPFCSDProject.Services
{
    // Public class to handle database connections. It is public so that it can be
    // accessed by other parts of the application and unit tests
    public class DatabaseService
    {
        /// <summary>
        /// Creates and opens a new connection to the database.
        /// Usage: using (var connection = databaseService.GetConnection()) { ... }
        /// </summary>
        /// <returns>An open MySqlConnection object ready for queries</returns
        public MySqlConnection GetConnection()
        {
            // The conection string is stored in PrivateInfo for security
            string connectionSTring = PrivateInfo.ConnectionString;

            // Try to create and open the connection
            try
            {
                var connection = new MySqlConnection(connectionSTring);
                connection.Open();
                return connection;

            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to database: " + ex.Message);
            }
            }
    }
}
