using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.ViewEngines.Razor;

namespace Daventinder.Webapp.App.Config
{
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "Daventinder.Shared";
            yield return "Humanizer";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "Daventinder.Webapp";
            yield return "Daventinder.Shared";
            yield return "Humanizer";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}
