using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daventinder.Webapp.App.Database;
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
            var sb = new NpgsqlConnectionStringBuilder(ConnectionProvider.ConnectionString) { Pooling = false };

            JobStorage.Current = new PostgreSqlStorage(sb.ConnectionString);

            app.UseErrorPage()
               .UseHangfireServer()
               .UseHangfireDashboard("/dashboard")
               .UseNancy(options => options.PassThroughWhenStatusCodesAre(
                   HttpStatusCode.NotFound,
                    HttpStatusCode.InternalServerError));

            RecurringJob.AddOrUpdate(() => new MenuDbUpdater().UpdateMenus(), Cron.Daily(6));
        }
    }
}
