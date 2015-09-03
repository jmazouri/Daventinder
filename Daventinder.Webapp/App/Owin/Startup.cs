using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Nancy;
using Nancy.Owin;
using Npgsql;
using Owin;

namespace Daventinder.Webapp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var sb =
                new NpgsqlConnectionStringBuilder("Server=192.168.1.54;Port=5432;Database=daventinder;User Id=postgres;Password=1234;")
                {
                    Pooling = false
                };

            JobStorage.Current = new PostgreSqlStorage(sb.ConnectionString);

            app.UseErrorPage()
               .UseHangfireServer()
               .UseHangfireDashboard("/dashboard")
               .UseNancy(options => options.PassThroughWhenStatusCodesAre(
                   HttpStatusCode.NotFound,
                    HttpStatusCode.InternalServerError));
        }
    }
}
