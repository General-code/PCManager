using MySqlConnector;

namespace PCManager.Repositories
{
    public class DBManager
    {
        private static readonly string CONNECTION_STRING = App.DBConnectionString;

        private static void SafeExecute(Action<MySqlConnection> action)
        {
            try
            {
                using var conn = new MySqlConnection(CONNECTION_STRING);
                conn.Open();
                action(conn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
            }
        }

        public static T? SafeExecute<T>(Func<MySqlConnection, T> func)
        {
            try
            {
                using var conn = new MySqlConnection(CONNECTION_STRING);
                conn.Open();
                return func(conn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
                return default;
            }
        }

        public static async Task SafeExecuteAsync(Func<MySqlConnection, Task> action)
        {
            try
            {
                using var conn = new MySqlConnection(CONNECTION_STRING);
                await conn.OpenAsync();
                await action(conn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
            }
        }

        public static async Task<T?> SafeExecuteAsync<T>(Func<MySqlConnection, Task<T>> func)
        {
            try
            {
                using var conn = new MySqlConnection(CONNECTION_STRING);
                await conn.OpenAsync();
                return await func(conn);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB Error]: {ex.Message}");
                return default(T);
            }
        }

        public static void ExecuteNonQuery(string sql)
        {
            SafeExecute(conn =>
            {
                using var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            });
        }
    }
}
