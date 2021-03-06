﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog.Internal;
using System.Configuration;
using Nancy.Helpers;

namespace Daventinder.Webapp.App.Bing
{
    public static class SearchResult
    {
        public static List<string> FindImageForQuery(string query)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings.Get("bingkey"), System.Configuration.ConfigurationManager.AppSettings.Get("bingkey"));

                int searchCount = 1;

                "SearchResult".Log().Info("Searching for " + query);

                XDocument doc = XDocument.Parse(client.DownloadString($"https://api.datamarket.azure.com/Bing/Search/v1/Image?Query=%27{HttpUtility.UrlEncode(query)}%27"));

                var found = doc.Root.Elements("{http://www.w3.org/2005/Atom}entry")
                    .Select(d => d.Elements("{http://www.w3.org/2005/Atom}content")
                                .Elements("{http://schemas.microsoft.com/ado/2007/08/dataservices/metadata}properties")
                                .Elements("{http://schemas.microsoft.com/ado/2007/08/dataservices}MediaUrl")
                                .First().Value);

                return found.ToList();
            }
            catch (WebException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
