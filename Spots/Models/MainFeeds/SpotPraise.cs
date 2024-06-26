﻿using Plugin.Firebase.Firestore;

namespace Spots;

public class SpotPraise
{
    private ImageSource? _AuthorProfilePicture;
    private ImageSource? _SpotProfilePicture;

    public string PraiseID { get; private set; }
    public string AuthorID { get; private set; }
    public string AuthorFullName { get; private set; }
    public ImageSource AuthorProfilePicture
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
    public ImageSource SpotProfilePicture
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
    public DateTimeOffset CreationDate { get; set; }
    public string Comment { get; set; }
    public ImageSource? AttachedPicture { get; set; }

    public SpotPraise(SpotPraise_Firebase spotPraise_FB, Client author, Spot spot, ImageSource? attachment = null)
    {
        PraiseID = spotPraise_FB.PraiseID;
        AuthorID = spotPraise_FB.AuthorID[0];
        AuthorFullName = author.FullName;
        AuthorProfilePicture = author.ProfilePictureSource;
        SpotID = spotPraise_FB.SpotID[0];
        SpotFullName = spot.SpotName + " - " + spot.BrandName;
        SpotProfilePicture = spot.ProfilePictureSource;
        CreationDate = spotPraise_FB.CreationDate;
        Comment = spotPraise_FB.Comment;
        AttachedPicture = attachment;
    }

    public SpotPraise(string praiseID,
        string author,
        string authorFullName,
        string spotReviewed,
        string spotFullName,
        DateTimeOffset creationDate,
        ImageSource? authorPicture = null,
        ImageSource? spotPicture = null,
        string comment = "",
        ImageSource? attachedPicture = null)
    {
        PraiseID = praiseID;
        AuthorID = author;
        AuthorFullName = authorFullName;
        AuthorProfilePicture = authorPicture;
        SpotID = spotReviewed;
        SpotFullName = spotFullName;
        SpotProfilePicture = spotPicture;
        CreationDate = creationDate;
        Comment = comment;
        AttachedPicture = attachedPicture;
    }
}

public class SpotPraise_Firebase
{
    [FirestoreDocumentId]
    public string PraiseID { get; set; }

    [FirestoreProperty(nameof(SearchCriteria_ID))]
    public IList<string> SearchCriteria_ID { get; set; }

    [FirestoreProperty(nameof(AuthorID))]
    public IList<string> AuthorID { get; set; }

    [FirestoreProperty(nameof(SpotID))]
    public IList<string> SpotID { get; set; }

    [FirestoreProperty(nameof(CreationDate))]
    public DateTimeOffset CreationDate { get; set; }

    [FirestoreProperty(nameof(Comment))]
    public string Comment { get; set; }

    [FirestoreProperty(nameof(AttachedPictureAddress))]
    public string AttachedPictureAddress { get; set; }

    public SpotPraise_Firebase()
    {
        PraiseID = "";
        SearchCriteria_ID = [];
        AuthorID = [];
        SpotID = [];
        CreationDate = DateTimeOffset.Now;
        Comment = "";
        AttachedPictureAddress = "";
    }

    public SpotPraise_Firebase(SpotPraise spotPraise, string attachmentAddress = "")
    {
        PraiseID = spotPraise.PraiseID;
        SearchCriteria_ID = [spotPraise.PraiseID];
        AuthorID = [spotPraise.AuthorID];
        SpotID = [spotPraise.SpotID];
        CreationDate = spotPraise.CreationDate;
        Comment = spotPraise.Comment;
        AttachedPictureAddress = attachmentAddress;
    }

    public SpotPraise_Firebase(string praiseID, string authorID, string spotID, DateTimeOffset creationDate, string comment, string attachedPictureAddress)
    {
        PraiseID = praiseID;
        SearchCriteria_ID = [praiseID];
        AuthorID = [authorID];
        SpotID = [spotID];
        CreationDate = creationDate;
        Comment = comment;
        AttachedPictureAddress = attachedPictureAddress;
    }
}
