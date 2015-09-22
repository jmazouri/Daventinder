using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daventinder.Shared
{
    public enum Sentiment
    {
        Positive,
        Negative,
        Neutral
    }

    public class Rating
    {
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public int TotalVotes => Upvotes + Downvotes;

        public float UpvotePercent => (TotalVotes == 0 ? 0 : ((float)Upvotes /TotalVotes));

        public float DownvotePercent => (TotalVotes == 0 ? 0 : ((float)Downvotes / TotalVotes));

        public Sentiment Sentiment => (TotalVotes == 0 ? Sentiment.Neutral : (UpvotePercent >= 0.65 ? Sentiment.Positive : Sentiment.Negative));

        public Rating(int upvotes, int downvotes)
        {
            Upvotes = upvotes;
            Downvotes = downvotes;
        }
    }
}
