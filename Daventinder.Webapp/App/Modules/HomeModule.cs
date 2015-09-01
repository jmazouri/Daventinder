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
            DiningWebpageParser parser = new DiningWebpageParser("http://www.davenport.edu/dining/dining-hall/weeks-menu");
            MenuPdfParser pdfParser = new MenuPdfParser(parser.PdfList);

            Get["/"] = x => View["Home", pdfParser];
        }
    }
}
