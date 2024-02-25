namespace Spots;

public class SpotPraise(string author, string spotReviewed, DateTimeOffset creationDate,
    string comment = "", Dictionary<string, ImageSource>? images = null)
{
    public string sPraiseID { get; }
    public string sAuthorID { get; } = author;
    public string sSpotReviewedID { get; } = spotReviewed;
    public DateTimeOffset dtCreationDate { get; } = creationDate;
    public string sComment { get; } = comment;
    /// <summary>
    /// Dictionary -> (Key = Image Address) - (Value = ImageSource object)
    /// </summary>
    public Dictionary<string, ImageSource> dctPictures { get; } = images ?? [];
}
