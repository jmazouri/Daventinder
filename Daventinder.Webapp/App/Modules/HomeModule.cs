using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daventinder.Shared;
using Nancy;

namespace Daventinder.Webapp.App.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = x => View["Home", pdfParser.Menus.OrderByDescending(d=>d.Date).Take(1)];
        }
    }
}
