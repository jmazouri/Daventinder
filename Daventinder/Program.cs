using System;
using System.Security.Policy;
using Daventinder.Shared;

namespace Daventinder.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DiningWebpageParser parser = new DiningWebpageParser("http://www.davenport.edu/dining/dining-hall/weeks-menu");
            MenuPdfParser pdfParser = new MenuPdfParser(parser.PdfList);

            foreach (var menu in pdfParser.Menus)
            {
                Console.WriteLine(menu);
            }

            Console.ReadLine();
        }
    }
}
