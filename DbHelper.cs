using Npgsql;
using System.Data;

namespace CampusEventManager
{
    public static class DbHelper
    {
        // Şifreni aşağıya doğru şekilde yaz:
        private static string connectionString = "Host=localhost;Port=5432;Database=CampusFinalDB;Username=postgres;Password=kaganguner";

        public static NpgsqlConnection GetConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            return conn;
        }
    }
}