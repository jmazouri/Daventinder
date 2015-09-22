using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace Daventinder.Shared
{
    public class DiningWebpageParser
    {
        public string Url { get; set; }
        public Dictionary<DateTime, string> PdfList = new Dictionary<DateTime, string>();

        public DiningWebpageParser(string url)
        {
            Url = url;

            CQ document = CQ.CreateFromUrl(Url);
            var days = document["#block-system-main > div > div > div.field.field-name-body.field-type-text-with-summary.field-label-hidden > div > div > table:nth-child(2) > tbody td"];

            foreach (var day in days)
            {
                if (day.FirstChild.HasAttribute("href"))
                {
                    DateTime cur = DateTime.Parse(day.FirstChild.InnerText);
                    string linkUrl = day.FirstChild["href"];

                    if (!linkUrl.EndsWith("pdf"))
                    {
                        CQ tempdoc = CQ.CreateFromUrl(linkUrl);
                        PdfList.Add(cur, tempdoc["a"].Where(d => d.HasAttribute("href")).First(d => d["href"].EndsWith("pdf"))["href"]);
                    }
                    else
                    {
                        PdfList.Add(cur, linkUrl);
                    }
                }
            }
        }

    }
}
