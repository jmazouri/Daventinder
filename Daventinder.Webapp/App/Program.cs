using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin.Hosting;

namespace Daventinder.Webapp
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();

#if DEBUG
            var url = "http://localhost:1515";
#else
            var url = "http://daventinder.xyz:80";
#endif

            LogExtensions.Log(typeof(Program)).Info("Initializing OWIN with url {0}", url);

            using (WebApp.Start<Startup>(url))
            {
                LogExtensions.Log(typeof(Program)).Info("OWIN Webapp running");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
