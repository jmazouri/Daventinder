using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;

namespace Daventinder.Shared
{
    public class MenuPdfParser
    {
        public List<Menu> Menus = new List<Menu>();

        public MenuPdfParser(Dictionary<DateTime, string> pdfUrlList)
        {
            using (WebClient client = new WebClient())
            {
                foreach (var entry in pdfUrlList)
                {
                    ParsePdf(entry.Key, client.DownloadData(entry.Value));
                }
            }
        }

        private void ParsePdf(DateTime curDate, byte[] pdfData)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromBytes(pdfData);

            Menus.Add(new Menu(curDate, doc.Pages[0].ExtractText()));
        }
    }
}
