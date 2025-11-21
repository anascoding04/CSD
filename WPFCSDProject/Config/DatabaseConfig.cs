using MySql.Data.MySqlClient;

namespace WPFCSDProject.Config
{
    public static class DatabaseConfig
    {
        private const string ConnectionString = 
            "Server=localhost;" +
            "Port=3306;" +
            "Database=WS374402_CSD;" +
            "Uid=WS374402_CSD;" +
            "Pwd=aCULut%2lx^ykn31;";

        public static string GetConnectionString() => ConnectionString;
    }
}
