using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Daventinder.Shared;

namespace Daventinder.Webapp.App.Database
{
    public class MenuDbRepository
    {
        private static MenuDbRepository _curr;
        public static MenuDbRepository Current => _curr ?? (_curr = new MenuDbRepository(ConnectionProvider.GetConnection()));

        private IDbConnection _conn;
        public MenuDbRepository(IDbConnection connection)
        {
            _conn = connection;
        }

        public void Initialize()
        {
            if (_conn.Query("SELECT to_regclass('public.menus')") == null)
            {
                this.Log().Debug("Table public.menus does NOT exist.");
            }
        }

        public void Connect()
        {
            
        }
    }
}
