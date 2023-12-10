using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models.MainFeeds
{
    public class SpotPraise
    {
        public string sAuthor { get; }
        public string sSpotReviewed { get; }
        public DateTimeOffset dtCreationDate { get; }
        public string sComment { get; }
        /// <summary>
        /// Dictionary -> (Key = Image Address) - (Value = ImageSource object)
        /// </summary>
        public Dictionary<string, ImageSource> dctPictures { get; }

        public SpotPraise(string author, string spotReviewed, DateTimeOffset creationDate, string comment = "", Dictionary<string, ImageSource> images = null)
        {
            sAuthor = author;
            sSpotReviewed = spotReviewed;
            dtCreationDate = creationDate;
            sComment = comment;
            dctPictures = images == null ? new Dictionary<string, ImageSource>() : images;
        }
    }
}
