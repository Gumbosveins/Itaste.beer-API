using Npgsql;

namespace ItbApi.Helpers
{
    public static class ConnectionStrings
    {
        // Accepts either a postgres:// URL (as Railway provides) or a plain Npgsql connection string
        public static string FromDatabaseUrl(string value)
        {
            if (!value.StartsWith("postgres://") && !value.StartsWith("postgresql://"))
            {
                return value;
            }

            var uri = new Uri(value);
            var userInfo = uri.UserInfo.Split(':', 2);
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = uri.AbsolutePath.TrimStart('/'),
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null
            };
            return builder.ConnectionString;
        }
    }
}
