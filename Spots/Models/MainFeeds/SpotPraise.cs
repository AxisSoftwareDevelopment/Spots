using Firebase.Firestore;
using Plugin.Firebase.Firestore;

namespace Spots;

public class SpotPraise
{
    private ImageSource? _AuthorProfilePicture;
    private ImageSource? _SpotProfilePicture;

    public string PraiseID { get; private set; }
    public string AuthorID { get; private set; }
    public string AuthorFullName { get; private set; }
    public ImageSource? AuthorProfilePicture
    {
        get
        {
            return _AuthorProfilePicture ?? ImageSource.FromFile("placeholder_logo.jpg");
        }

        private set
        {
            _AuthorProfilePicture = value;
        }
    }
    public string SpotID { get; private set; }
    public string SpotFullName { get; private set; }
    public ImageSource? SpotProfilePicture
    {
        get
        {
            return _SpotProfilePicture ?? ImageSource.FromFile("placeholder_logo.jpg");
        }

        private set
        {
            _SpotProfilePicture = value;
        }
    }
    public FirebaseLocation SpotLocation { get; private set; }
    public DateTimeOffset CreationDate { get; set; }
    public string Comment { get; set; }
    public ImageSource? AttachedPicture { get; set; }
    public int LikesCount { get; set; }

    public SpotPraise(SpotPraise_Firebase spotPraise_FB, Client author, Spot spot, ImageSource? attachment = null)
    {
        PraiseID = spotPraise_FB.PraiseID;
        AuthorID = spotPraise_FB.AuthorID;
        AuthorFullName = author.FullName;
        AuthorProfilePicture = author.ProfilePictureSource;
        SpotID = spotPraise_FB.SpotID;
        SpotFullName = spot.SpotName + " - " + spot.BrandName;
        SpotProfilePicture = spot.ProfilePictureSource;
        SpotLocation = spotPraise_FB.SpotLocation;
        CreationDate = spotPraise_FB.CreationDate;
        Comment = spotPraise_FB.Comment;
        AttachedPicture = attachment;
        LikesCount = spotPraise_FB.LikesCount;
    }

    public SpotPraise(string praiseID,
        string author,
        string authorFullName,
        string spotReviewed,
        string spotFullName,
        DateTimeOffset creationDate,
        ImageSource? authorPicture = null,
        ImageSource? spotPicture = null,
        FirebaseLocation? spotLocation = null,
        string comment = "",
        ImageSource? attachedPicture = null,
        int likesCount = 0)
    {
        PraiseID = praiseID;
        AuthorID = author;
        AuthorFullName = authorFullName;
        AuthorProfilePicture = authorPicture;
        SpotID = spotReviewed;
        SpotFullName = spotFullName;
        SpotProfilePicture = spotPicture;
        SpotLocation = spotLocation ?? new();
        CreationDate = creationDate;
        Comment = comment;
        AttachedPicture = attachedPicture;
        LikesCount = likesCount;
    }

    public static async Task<SpotPraise> GetPraiseFromFirebaseObject(SpotPraise_Firebase praise)
    {
        Client author = await DatabaseManager.GetClientDataAsync(praise.AuthorID);
        Spot spot = await DatabaseManager.GetSpotDataAsync(praise.SpotID);
        ImageSource? attachment = null;

        if (praise.AttachedPictureAddress.Length > 0)
        {
            string downloadAddress = await DatabaseManager.GetImageDownloadLink(praise.AttachedPictureAddress);
            Uri imageUri = new(downloadAddress);

            attachment = ImageSource.FromUri(imageUri);
        }

        return new(praise, author, spot, attachment);
    }

    public static async Task<List<SpotPraise>> GetPraisesFromFirebaseObject(List<IDocumentSnapshot<SpotPraise_Firebase>> documentSnapshots, Client? author = null, Spot? spot = null)
    {
        List<SpotPraise> spotPraises = [];

        List<IDocumentSnapshot<SpotPraise_Firebase>> documents = documentSnapshots;
        foreach (var document in documents)
        {
            if (document.Data != null)
            {
                SpotPraise_Firebase praise = document.Data;
                Client managed_author = author != null ? author : await DatabaseManager.GetClientDataAsync(praise.AuthorID);
                Spot managed_spot = spot != null ? spot : await DatabaseManager.GetSpotDataAsync(praise.SpotID);
                ImageSource? attachment = null;

                if (praise.AttachedPictureAddress.Length > 0)
                {
                    string downloadAddress = await DatabaseManager.GetImageDownloadLink(praise.AttachedPictureAddress);
                    Uri imageUri = new(downloadAddress);

                    attachment = ImageSource.FromUri(imageUri);
                }

                spotPraises.Add(new(praise, managed_author, managed_spot, attachment));
            }
        }

        return spotPraises;
    }
}

public class SpotPraise_Firebase
{
    [FirestoreDocumentId]
    public string PraiseID { get; set; }

    [FirestoreProperty(nameof(SearchCriteria_ID))]
    public IList<string> SearchCriteria_ID { get; set; }

    [FirestoreProperty(nameof(AuthorID))]
    public string AuthorID { get; set; }

    [FirestoreProperty(nameof(AuthorID_Array))]
    public IList<string> AuthorID_Array { get { return [AuthorID]; } private set { } }

    [FirestoreProperty(nameof(SpotID))]
    public string SpotID { get; set; }

    [FirestoreProperty(nameof(SpotLocation))]
    public FirebaseLocation SpotLocation { get; set; }

    [FirestoreProperty(nameof(CreationDate))]
    public DateTimeOffset CreationDate { get; set; }

    [FirestoreProperty(nameof(Comment))]
    public string Comment { get; set; }

    [FirestoreProperty(nameof(AttachedPictureAddress))]
    public string AttachedPictureAddress { get; set; }

    [FirestoreProperty(nameof(LikesCount))]
    public int LikesCount { get; set; }

    public SpotPraise_Firebase()
    {
        PraiseID = "";
        SearchCriteria_ID = [];
        AuthorID = "";
        SpotID = "";
        SpotLocation = new();
        CreationDate = DateTimeOffset.Now;
        Comment = "";
        AttachedPictureAddress = "";
        LikesCount = 0;
    }

    public SpotPraise_Firebase(SpotPraise spotPraise, string attachmentAddress = "")
    {
        PraiseID = spotPraise.PraiseID;
        SearchCriteria_ID = [spotPraise.PraiseID];
        AuthorID = spotPraise.AuthorID;
        SpotID = spotPraise.SpotID;
        SpotLocation = spotPraise.SpotLocation;
        CreationDate = spotPraise.CreationDate;
        Comment = spotPraise.Comment;
        AttachedPictureAddress = attachmentAddress;
        LikesCount = spotPraise.LikesCount;
    }

    public SpotPraise_Firebase(string praiseID, string authorID, string spotID, FirebaseLocation spotLocation, DateTimeOffset creationDate, string comment, string attachedPictureAddress, int likesCount)
    {
        PraiseID = praiseID;
        SearchCriteria_ID = [praiseID];
        AuthorID = authorID;
        SpotID = spotID;
        SpotLocation = spotLocation;
        CreationDate = creationDate;
        Comment = comment;
        AttachedPictureAddress = attachedPictureAddress;
        LikesCount = likesCount;
    }
}

public class PraiseLike_Firebase
{
    [FirestoreDocumentId]
    public string ID {  get; set; }
    [FirestoreProperty(nameof(ClientID))]
    public string ClientID {  get; set; }
    [FirestoreProperty(nameof(SpotID))]
    public string SpotID {  get; set; }
    public PraiseLike_Firebase()
    {
        ID = string.Empty;
        ClientID = string.Empty;
        SpotID = string.Empty;
    }

    public PraiseLike_Firebase(string client, string spot, string id = "")
    {
        ID = id;
        ClientID = client;
        SpotID = spot;
    }
}
