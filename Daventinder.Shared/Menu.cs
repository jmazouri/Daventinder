using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daventinder.Shared
{
    public enum DailyMeal
    {
        Brunch,
        Breakfast,
        Lunch,
        Dinner,
        Other
    }

    public class Menu
    {
        public DateTime Date = DateTime.MinValue;
        public Dictionary<DailyMeal, List<string>> DailyMeals = new Dictionary<DailyMeal, List<string>>();

        public List<string> AllMeals
        {
            get { return DailyMeals.SelectMany(d => d.Value).ToList(); }
        }

        public Menu(DateTime date, string menuContent)
        {
            Date = date;
            DailyMeals = ParseMenu(menuContent);
        }

        private static Dictionary<DailyMeal, List<string>> ParseMenu(string rawMenu)
        {
            var items = new Dictionary<DailyMeal, List<string>>();
            //var removedItems = new List<string>();

            string[] allLines = rawMenu.Split(Environment.NewLine.ToCharArray());

            List<string> bannedWords = new List<string> { "chef", "subject", "entree" };
            bannedWords.AddRange(Enum.GetNames(typeof(DayOfWeek)).Select(d => d.ToLower()));

            DailyMeal lastSection = DailyMeal.Other;

            for (int i = 0; i < allLines.Length; i++)
            {
                string input = allLines[i];

                input = input.Replace(" X", "").Replace(" x", "").Trim();

                if (bannedWords.Any(d => input.ToLower().Contains(d)))
                {
                    //removedItems.Add(input);
                    continue;
                }

                if (String.IsNullOrWhiteSpace(input.Trim())) { continue; }

                string endWord = input.Split(' ').Last().ToLower().Trim();

                if ((endWord == "and" || endWord == "with") && i != allLines.Length - 1)
                {
                    input = input + " " + allLines[i + 2].Trim();
                    i+=2;
                }

                foreach (string meal in Enum.GetNames(typeof (DailyMeal)).Select(d => d.ToLower()))
                {
                    if (input.ToLower().Contains(meal))
                    {
                        var inputWords = input.ToLower().Split(':', ' ');
                        foreach (string word in inputWords)
                        {
                            if (word != meal)
                            {
                                continue;
                            }

                            lastSection = (DailyMeal)Enum.Parse(typeof(DailyMeal), word, true);
                            break;
                        }
                    }
                }

                if (items.ContainsKey(lastSection))
                {
                    items[lastSection].Add(input);
                }
                else
                {
                    items.Add(lastSection, new List<string> {});
                }
            }

            return items;
        }

        public override string ToString()
        {
            var build = new StringBuilder();

            build.AppendLine(Date.ToShortDateString());

            foreach (var s in DailyMeals)
            {
                build.AppendLine();
                build.AppendLine("===== "+s.Key+ " =====");
                foreach (var m in s.Value)
                {
                    build.AppendLine(m);
                }
            }

            build.AppendLine();
            build.AppendLine(new String('~', AllMeals.Max(d=>d.Length) + 2));

            return build.ToString();
        }
    }
}
