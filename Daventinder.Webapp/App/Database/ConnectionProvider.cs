using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Daventinder.Webapp.App.Database
{
    public static class ConnectionProvider
    {
        public const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=daventinder;User Id=postgres;Password=1234;";
        private static IDbConnection _conn;

        public static IDbConnection GetConnection()
        {
            return _conn ?? (_conn = new NpgsqlConnection(ConnectionString));
        }
    }
}