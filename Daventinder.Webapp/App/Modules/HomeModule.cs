using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daventinder.Shared;
using Daventinder.Webapp.App.Database;
using Humanizer;
using Nancy;
using Nancy.Responses;

namespace Daventinder.Webapp.App.Modules
{
    public class HomeModule : NancyModule
    {
        private Response RateItem(string name, Sentiment sentiment)
        {
            if (String.IsNullOrWhiteSpace(name)) { return new Response().WithStatusCode(HttpStatusCode.InternalServerError); }

            try
            {
                string voteName = "lastvote_" + name.Dehumanize();
                var lastVote = Request.Session[voteName];

                if (lastVote != null)
                {
                    TimeSpan voteLimit = new TimeSpan(0, 5, 0);
                    TimeSpan remaining = voteLimit - (((DateTime)lastVote) - DateTime.Now);

                    if (((DateTime)lastVote) - DateTime.Now <= voteLimit)
                    {
                        return Response.AsText(remaining.Humanize()).WithStatusCode(HttpStatusCode.TooManyRequests);
                    }
                }
                Request.Session[voteName] = DateTime.Now;

                MenuDbRepository.Current.ChangeRating(name, sentiment);
                return Response.AsJson(MenuDbRepository.Current.GetRatingForItem(name)).WithStatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.Log().Error("Could not rate item " + name, ex);
                return new Response().WithStatusCode(HttpStatusCode.InternalServerError);
            }
        }

        public HomeModule()
        {
            Get["/"] = x => View["Home", new
            {
                Menus = MenuDbRepository.Current.CurrentMenus,
                Ratings = MenuDbRepository.Current.GetRatings()
            }];

            Get["/all"] = x => View["Home", new
            {
                Menus = MenuDbRepository.Current.AllMenus,
                Ratings = MenuDbRepository.Current.GetRatings()
            }];

            Get["/rate/bad/{item}"] = x => RateItem(x.item, Sentiment.Negative);

            Get["/rate/good/{item}"] = x => RateItem(x.item, Sentiment.Positive);

            Get["/image/{food}"] = x => Response.AsImage(!File.Exists(Path.Combine("images", (string) x.food)) ? "help.jpg" : Path.Combine("images", (string)x.food));
        }
    }
}
