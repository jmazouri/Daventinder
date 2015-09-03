using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daventinder.Shared;
using Npgsql;

namespace Daventinder.Webapp.App.Database
{
    public class MenuDbUpdater
    {
        private IDbConnection _conn;

        public MenuDbUpdater()
        {
            _conn = new NpgsqlConnection(ConnectionProvider.ConnectionString);
        }

        public void UpdateMenus()
        {
            DiningWebpageParser parser = new DiningWebpageParser("http://www.davenport.edu/dining/dining-hall/weeks-menu");
            this.Log().Info("Webpage parser completed, {0} pdfs found", parser.PdfList.Count);

            MenuPdfParser pdfParser = new MenuPdfParser(parser.PdfList);
            this.Log().Info("PDF Parser completed, {0} menus loaded with {1} items", pdfParser.Menus.Count, pdfParser.Menus.Sum(d=>d.AllMeals.Count));
        }
    }
}
