using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Daventinder.Shared;
using Newtonsoft.Json;

namespace Daventinder.Webapp.App.Database
{
    public class MenuDbRepository
    {
        private static MenuDbRepository _curr;
        public static MenuDbRepository Current => _curr ?? (_curr = new MenuDbRepository(ConnectionProvider.GetConnection()));

        private IDbConnection _conn;
        public MenuDbRepository(IDbConnection connection)
        {
            _conn = connection;
        }

        public bool IsInitialized { get; set; }

        public void Initialize()
        {
            if (_conn.ExecuteScalar("SELECT to_regclass('public.ratings')") == null)
            {
                this.Log().Warn("Table public.ratings does NOT exist. Creating.");

                _conn.Execute(@"CREATE TABLE ratings (
                                ID          SERIAL  PRIMARY KEY  NOT NULL,
                                MEAL        TEXT    UNIQUE       NOT NULL,
                                UPVOTES     INTEGER              NOT NULL,
                                DOWNVOTES   INTEGER              NOT NULL
                            ); ");
            }

            if (_conn.ExecuteScalar("SELECT to_regclass('public.menus')") == null)
            {
                this.Log().Warn("Table public.menus does NOT exist. Creating.");

                _conn.Execute(@"CREATE TABLE menus (
                                ID          SERIAL  PRIMARY KEY  NOT NULL,
                                DATE        DATE    UNIQUE       NOT NULL,
                                DailyMeals  TEXT                 NOT NULL
                            ); ");
            }

            IsInitialized = true;

            new MenuDbUpdater().UpdateMenus();
        }

        public void UpsertMenu(DateTime date, string jsonMenu)
        {
            if ((bool) _conn.ExecuteScalar(@"SELECT EXISTS(SELECT 1 FROM menus WHERE DATE = @a)", new {a = date}))
            {
                _conn.Execute(@"UPDATE menus SET DailyMeals = @a", new { a = jsonMenu });
                this.Log().Info("Updated menu for {0}", date);
            }
            else
            {
                _conn.Execute(@"INSERT INTO menus(DATE, DailyMeals) VALUES (@a, @b)", new { a = date, b = jsonMenu });
                this.Log().Info("Inserted new menu for {0}", date);
            }
        }

        public void ChangeRating(string menuItem, Sentiment sentiment)
        {
            if ((bool) _conn.ExecuteScalar(@"SELECT EXISTS(SELECT 1 FROM ratings WHERE MEAL = @a)", new {a = menuItem}))
            {
                _conn.Execute(
                    sentiment == Sentiment.Positive
                        ? @"UPDATE ratings SET UPVOTES = UPVOTES + 1 WHERE MEAL = @a"
                        : @"UPDATE ratings SET DOWNVOTES = DOWNVOTES + 1 WHERE MEAL = @a", new {a = menuItem});
            }
            else
            {
                _conn.Execute(@"INSERT INTO ratings(MEAL, UPVOTES, DOWNVOTES) VALUES (@a, @b, @c)", new
                {
                    a = menuItem,
                    b = (sentiment == Sentiment.Positive ? 1 : 0),
                    c = (sentiment == Sentiment.Negative ? 1 : 0)
                });
            }
        }

        public Dictionary<string, Rating> GetRatings()
        {
            var ret = new Dictionary<string, Rating>();

            foreach (dynamic result in _conn.Query(@"SELECT * FROM ratings ;"))
            {
                ret.Add(result.meal, new Rating(result.upvotes, result.downvotes));
            }

            return ret;
        }

        public Rating GetRatingForItem(string menuItem)
        {
            var found = _conn.Query(@"SELECT * FROM ratings WHERE MEAL = @item;", new { item = menuItem }).First();
            return new Rating(found.upvotes, found.downvotes);
        }

        public List<Menu> ParseQuery(IEnumerable<dynamic> queryResult)
        {
            return queryResult.Select(result => new Menu(result.date, JsonConvert.DeserializeObject<Dictionary<DailyMeal, List<string>>>(result.dailymeals))).ToList();
        }

        public List<Menu> AllMenus => ParseQuery(_conn.Query(@"SELECT * FROM menus;"));
        public List<Menu> CurrentMenus => ParseQuery(_conn.Query(@"SELECT * FROM menus WHERE DATE >= current_date;"));
    }
}
