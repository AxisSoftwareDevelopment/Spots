using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace Spots;

public class SpotPraise
{
    [FirestoreDocumentId]
    public string PraiseID { get; }

    [FirestoreProperty(nameof(AuthorID))]
    public string AuthorID { get; }
    public string AuthorFullName { get; }
    public string AuthorPictureAddress {  get; }
    //public ImageSource? AuthorProfilePicture
    //{
    //    get
    //    {
    //        return AuthorProfilePicture ?? ImageSource.FromFile("placeholder_logo.jpg");
    //    }

    //    private set
    //    {
    //        AuthorProfilePicture = value;
    //    }
    //}

    [FirestoreProperty(nameof(SpotID))]
    public string SpotID { get; }
    public string SpotFullName { get; }
    public string SpotPictureAddress { get; }
    //public ImageSource? SpotProfilePicture
    //{
    //    get
    //    {
    //        return SpotProfilePicture ?? ImageSource.FromFile("placeholder_logo.jpg");
    //    }

    //    private set
    //    {
    //        SpotProfilePicture = value;
    //    }
    //}

    [FirestoreProperty(nameof(CreationDate))]
    public DateTimeOffset CreationDate { get; }

    [FirestoreProperty(nameof(Comment))]
    public string Comment { get; }

    [FirestoreProperty(nameof(AttachedPictureAddress))]
    public string AttachedPictureAddress { get; }

    //public ImageSource? AttachedPicture
    //{
    //    get
    //    {
    //        return AttachedPicture ?? ImageSource.FromFile("placeholder_logo.jpg");
    //    }

    //    private set
    //    {
    //        AttachedPicture = value;
    //    }
    //}

    public SpotPraise()
    {
        PraiseID = "";
        AuthorID = "";
        AuthorFullName = "";
        AuthorPictureAddress = "placeholder_logo.jpg";
        SpotID = "";
        SpotFullName = "";
        CreationDate = new DateTimeOffset();
        Comment = "";
        AttachedPictureAddress = "";
    }

    public SpotPraise(string praiseID, string author, string authorFullName, string spotReviewed, string spotFullName, DateTimeOffset creationDate,
        string authorPicture = "placeholder_logo.jpg", string spotPicture = "placeholder_logo.jpg", string comment = "", string pictureAddress = "")
    {
        PraiseID = praiseID;
        AuthorID = author;
        AuthorFullName = authorFullName;
        AuthorPictureAddress = authorPicture;
        //AuthorProfilePicture = authorPicture;
        SpotID = spotReviewed;
        SpotFullName = spotFullName;
        SpotPictureAddress = spotPicture;
        //SpotProfilePicture = spotPicture;
        CreationDate = creationDate;
        Comment = comment;
        AttachedPictureAddress = pictureAddress;
        //AttachedPicture = picture;
    }
}
