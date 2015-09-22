using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Daventinder.Shared;
using Daventinder.Webapp.App.Bing;
using Newtonsoft.Json;
using Npgsql;

namespace Daventinder.Webapp.App.Database
{
    public class MenuDbUpdater
    {
        public void UpdateMenus()
        {
            DiningWebpageParser parser = new DiningWebpageParser("http://www.davenport.edu/dining/dining-hall/weeks-menu");
            this.Log().Info("Webpage parser completed, {0} pdfs found", parser.PdfList.Count);

            MenuPdfParser pdfParser = new MenuPdfParser(parser.PdfList);
            this.Log().Info("PDF Parser completed, {0} menus loaded with {1} items", pdfParser.Menus.Count, pdfParser.Menus.Sum(d=>d.AllMeals.Count));

            if (!MenuDbRepository.Current.IsInitialized)
            {
                return;
            }

            foreach (var m in pdfParser.Menus)
            {
                MenuDbRepository.Current.UpsertMenu(m.Date, JsonConvert.SerializeObject(m.DailyMeals));

                using (WebClient client = new WebClient())
                {
                    Directory.CreateDirectory("images");

                    foreach (string item in m.AllMeals.Where(item => !File.Exists(Path.Combine("images", item + ".jpg"))))
                    {
                        try
                        {
                            int resultCount = 0;
                            List<string> results = SearchResult.FindImageForQuery(item);

                            if (results == null || results.Count == 0)
                            {
                                this.Log().Error("Could not find any results for " + item);
                                continue;
                            }

                            string result = results[resultCount];

                            while (!DownloadImage(item, result))
                            {
                                this.Log().Error("Could not download image for " + item + ", try "+(resultCount + 1));
                                resultCount++;
                                result = results[resultCount];
                            }
                        }
                        catch (WebException)
                        {
                            this.Log().Error("Could not query image for "+item);
                        }
                        
                    }
                }
            }
        }

        private bool DownloadImage(string item, string result)
        {
            try
            {
                this.Log().Info("Downloading image for " + item);
                ImageResize.ResizeImageFixedWidth(result, Path.Combine("images", item + ".jpg"), 512);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
