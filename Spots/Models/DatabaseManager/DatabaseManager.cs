using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;

namespace Spots;
public static class DatabaseManager
{
    const long MAX_IMAGE_STREAM_DIMENSION = 1024;

    #region Public Methods
    public static async Task<Client> LogInUserAsync(string email, string password, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
            throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no 'email and password' login method, or none at all.");

        IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

        //if(!firebaseUser.IsEmailVerified)
        //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

        Client user = new() { UserID = iFUser.Uid };
        if (getUser)
        {
            user = await GetClientDataAsync(iFUser.Uid);
            if (!user.UserDataRetrieved)
                await LogOutAsync();
        }
        return user;
    }

    public static async Task<bool> CreateUserAsync(string email, string password, string phoneNunber = "", string phoneCountryCode = "")
    {
        await CrossFirebaseAuth.Current.CreateUserAsync(email, password);
        await SaveUserDataAsync(new Client() { UserID = CrossFirebaseAuth.Current.CurrentUser.Uid, Email = email });

        string? id = CrossFirebaseAuth.Current.CurrentUser?.Uid;
        if (id == null || id.Length == 0 )
            throw new FirebaseAuthException(FIRAuthError.Undefined, "Custom Exception -> Unhandled exception: User not registered correctly.");

        await LogOutAsync();
        return true;
    }

    public static async Task<bool> ValidateCurrentSession()
    {
        try
        {
            if (CrossFirebaseAuth.Current.CurrentUser != null)
            {
                Client user = await GetClientDataAsync(CrossFirebaseAuth.Current.CurrentUser.Uid);
                SessionManager.StartSession(user);
                return true;
            }
        }
        catch (Exception ex) 
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
            SessionManager.CloseSession();
        }
        return false;
    }

    public static async Task LogOutAsync()
    {
        await CrossFirebaseAuth.Current.SignOutAsync();
    }

    public static async Task<bool> SaveUserDataAsync(Client user, string profilePictureAddress = "")
    {
        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(user.UserID);
            await documentReference.SetDataAsync(new Client_Firebase(user, profilePictureAddress));
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
            return false;
        }

        return true;
    }

    public static async Task<bool> SaveSpotDataAsync(Spot spot, string profilePictureAddress = "")
    {
        try
        {
            if (spot.SpotID.Length > 0)
            {
                IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("Spots").GetDocument(spot.SpotID);
                await documentReference.SetDataAsync(new Spot_Firebase(spot, profilePictureAddress));
            }
            else
            {
                IDocumentReference documentReference = await CrossFirebaseFirestore.Current.GetCollection("Spots").AddDocumentAsync(spot);
            }
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
            return false;
        }

        return true;
    }

    public async static Task<Client> GetClientDataAsync(string userID)
    {
        IDocumentSnapshot<Client_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("UserData")
            .GetDocument(userID)
            .GetDocumentSnapshotAsync<Client_Firebase>();

        // Even if documentSnapshot.Data doesnt support nullable returns, it is still possible to hold a null value.
        Client user = documentSnapshot.Data != null ? new(documentSnapshot.Data, await documentSnapshot.Data.GetImageSource()) : new() { UserID = userID };

        return user;
    }

    public async static Task<Spot> GetSpotDataAsync(string spotID)
    {
        IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("Spots")
            .GetDocument(spotID)
            .GetDocumentSnapshotAsync<Spot_Firebase>();

        // Even if documentSnapshot.Data doesnt support nullable returns, it is still possible to hold a null value.
        Spot user = new() { SpotID = spotID };
        if (documentSnapshot.Data != null)
        {
            Spot_Firebase spot_Firebase = documentSnapshot.Data;
            ImageSource profilePictureImageSource = await spot_Firebase.GetImageSource();
            user = new(spot_Firebase, profilePictureImageSource);
        }

        return user;
    }

    public static async Task<bool> SaveSpotPraiseData(SpotPraise praise, bool addToSpotList, ImageFile? imageFile = null)
    {
        try
        {
            bool bAlreadySaved = false;
            SpotPraise_Firebase praise_Firebase = new(praise);
            IDocumentReference praiseDocumentReference;
            if (praise_Firebase.PraiseID.Length > 0)
            {
                praiseDocumentReference = CrossFirebaseFirestore.Current.GetCollection("Praises").GetDocument(praise_Firebase.PraiseID);
            }
            else
            {
                praiseDocumentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises").AddDocumentAsync(praise_Firebase);
                bAlreadySaved = true;
            }

            if (praiseDocumentReference.Id.Length > 0 && imageFile != null)
            {
                string attachmentAddress = await SavePraiseAttachment(praiseDocumentReference.Id, praise_Firebase.AuthorID[0], imageFile);
                praise_Firebase.AttachedPictureAddress = attachmentAddress;
            }

            if((praiseDocumentReference.Id.Length > 0) && (!bAlreadySaved || imageFile != null))
            {
                await praiseDocumentReference.SetDataAsync(praise_Firebase);
            }

            if(addToSpotList)
            {
                IDocumentSnapshot<Spot_Firebase> spotSnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection("Spots")
                    .GetDocument(praise.SpotID)
                    .GetDocumentSnapshotAsync<Spot_Firebase>();
                if(spotSnapshot.Data != null && !spotSnapshot.Data.Praisers.Contains(praise.AuthorID))
                {
                    Spot_Firebase spot = spotSnapshot.Data;
                    spot.Praisers.Add(praise.AuthorID);

                    await CrossFirebaseFirestore.Current
                    .GetCollection("Spots")
                    .GetDocument(praise.SpotID)
                    .SetDataAsync(spot);
                }
            }

            return true;
        }
        catch(Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
            return false;
        }
    }

    public static async Task<string> SavePraiseAttachment(string praiseID, string userID, ImageFile imageFile)
    {
        string[] imageContentTypeArgs = imageFile.ContentType?.Split('/') ?? [];
        string filePath = $"Users/{userID}/PraieAttachments/{praiseID}.{imageContentTypeArgs[imageContentTypeArgs.Count() - 1] ?? ""}";

        IStorageReference storageRef = CrossFirebaseStorage.Current.GetReferenceFromPath(filePath);

        //await storageRef.DeleteAsync();
        await storageRef.PutBytes(imageFile.Bytes).AwaitAsync();
        return filePath;
    }

    public static async Task<string> SaveProfilePicture(bool isBusiness, string ID, ImageFile imageFile)
    {
        string filePath = $"{(isBusiness ? "Businesses" : "Users")}/{ID}/profilePicture.{imageFile.ContentType?.Replace("image/", "") ?? ""}";

        IStorageReference storageRef = CrossFirebaseStorage.Current.GetReferenceFromPath(filePath);

        //await storageRef.DeleteAsync();
        await storageRef.PutBytes(imageFile.Bytes).AwaitAsync();
        return filePath;
    }

    public static async Task<string> GetImageDownloadLink(string path)
    {
        IStorageReference storageRef = CrossFirebaseStorage.Current.GetReferenceFromPath(path);
        string imageStream = await storageRef.GetDownloadUrlAsync();

        return imageStream;
    }

    public static async Task<List<Client>> FetchClientsByID(List<string>clientIDs)
    {
        List<Client> clients = [];

        if(clientIDs.Count > 0)
        {
            IQuerySnapshot<Client_Firebase> documentReference = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                    .WhereArrayContainsAny("ClientID_ForSearch", clientIDs.ToArray())
                    .GetDocumentsAsync<Client_Firebase>();

            List<IDocumentSnapshot<Client_Firebase>> documents = documentReference.Documents.ToList();
            foreach (var document in documents)
            {
                ImageSource profilePictureImageSource = await document.Data.GetImageSource();
                clients.Add(new(document.Data, profilePictureImageSource));
            }
        }
        
        return clients;
    }

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromFollowedClients(Client client, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;
        List<string> clientIDs = new(client.FollowedClients) { client.UserID };

        if (lastPraise != null)
        {
            
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .StartingAfter(documentSnapshot)
                .WhereArrayContainsAny("AuthorID", clientIDs.ToArray())
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .WhereArrayContainsAny("AuthorID", clientIDs.ToArray())
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }

        List<IDocumentSnapshot<SpotPraise_Firebase>> documents = documentReference.Documents.ToList();
        foreach (var document in documents)
        {
            if (document.Data != null)
            {
                SpotPraise_Firebase praise = document.Data;
                Client author = await GetClientDataAsync(praise.AuthorID[0]);
                Spot spot = await GetSpotDataAsync(praise.SpotID[0]);
                ImageSource? attachment = null;

                if (praise.AttachedPictureAddress.Length > 0)
                {
                    string downloadAddress = await GetImageDownloadLink(praise.AttachedPictureAddress);
                    Uri imageUri = new(downloadAddress);

                    attachment = ImageSource.FromUri(imageUri);
                }

                spotPraises.Add(new(praise, author, spot, attachment));
            }
        }

        return spotPraises;
    }

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromClient(Client client, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;
        string[] id = [client.UserID];

        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .StartingAfter(documentSnapshot)
                .WhereArrayContainsAny("AuthorID", id)
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .WhereArrayContainsAny("AuthorID", id)
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }

        foreach (var document in documentReference.Documents)
        {
            if(document.Data != null)
            {
                SpotPraise_Firebase praise = document.Data;
                Spot spot = await GetSpotDataAsync(praise.SpotID[0]);
                ImageSource? attachment = null;

                if (praise.AttachedPictureAddress.Length > 0)
                {
                    string downloadAddress = await GetImageDownloadLink(praise.AttachedPictureAddress);
                    Uri imageUri = new(downloadAddress);

                    attachment = ImageSource.FromUri(imageUri);
                }

                spotPraises.Add(new(praise, client, spot, attachment));
            }
        }

        return spotPraises;
    }

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromSpot(Spot spot, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;
        string[] id = [spot.SpotID];

        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .StartingAfter(documentSnapshot)
                .WhereArrayContainsAny("SpotID", id)
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy("CreationDate")
                .WhereArrayContainsAny("SpotID", id)
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }

        foreach (var document in documentReference.Documents)
        {
            if (document.Data != null)
            {
                SpotPraise_Firebase praise = document.Data;
                Client author = await GetClientDataAsync(praise.AuthorID[0]);
                ImageSource? attachment = null;

                if (praise.AttachedPictureAddress.Length > 0)
                {
                    string downloadAddress = await GetImageDownloadLink(praise.AttachedPictureAddress);
                    Uri imageUri = new(downloadAddress);

                    attachment = ImageSource.FromUri(imageUri);
                }

                spotPraises.Add(new(praise, author, spot, attachment));
            }
        }

        return spotPraises;
    }

    public static async Task<List<Spot>> FetchSpotsNearby_Async(FirebaseLocation referenceLocation, int searchAreaInKm, Spot? lastSpot = null)
    {
        List<Spot> spots = [];
        IQuerySnapshot<Spot_Firebase> documentReference;

        List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, searchAreaInKm);
        geohashesNearby.Add(referenceLocation.Geohash[0]);

        if (lastSpot != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .GetDocument(lastSpot.SpotID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .WhereArrayContainsAny("Location.Geohash", geohashesNearby.ToArray())
                .StartingAfter(documentSnapshot)
                .LimitedTo(5)
                .GetDocumentsAsync<Spot_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .WhereArrayContainsAny("Location.Geohash", geohashesNearby.ToArray())
                .LimitedTo(5)
                .GetDocumentsAsync<Spot_Firebase>();
        }

        foreach (IDocumentSnapshot<Spot_Firebase> document in documentReference.Documents)
        {
            if (document.Data != null)
            {
                Spot_Firebase firebaseData = document.Data;
                spots.Add(new(firebaseData, await firebaseData.GetImageSource()));
            }
        }

        return spots;
    }

    public static async Task<List<Client>> FetchClients_ByNameAsync(string filterParams, string currentUserID, Client? lastClient = null)
    {
        List<Client> retVal = [];
        string[] searchTerms = [filterParams.ToUpper().Trim()];

        if (searchTerms[0].Length > 0)
        {
            IQuerySnapshot<Client_Firebase> documentReference;
            if (lastClient != null)
            {
                IDocumentSnapshot<Client_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                .GetDocument(lastClient.UserID)
                .GetDocumentSnapshotAsync<Client_Firebase>();

                documentReference = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                    .StartingAfter(documentSnapshot)
                    .WhereArrayContainsAny("SearchTerms", searchTerms)
                    .LimitedTo(25)
                    .GetDocumentsAsync<Client_Firebase>();
            }
            else
            {
                documentReference = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                    .WhereArrayContainsAny("SearchTerms", searchTerms)
                    .LimitedTo(25)
                    .GetDocumentsAsync<Client_Firebase>();
            }
            
            foreach (var document in documentReference.Documents)
            {
                if(document.Data.UserID !=  currentUserID)
                {
                    ImageSource profilePictureImageSource = await document.Data.GetImageSource();
                    retVal.Add(new(document.Data, profilePictureImageSource));
                }
            }
        }

        return retVal;
    }

    public static async Task<List<Spot>> FetchSpots_ByNameAsync(string filterParams, string currentUserID, Spot? lastSpot = null)
    {
        List<Spot> retVal = [];
        string[] searchTerms = [filterParams.ToUpper().Trim()];

        if (searchTerms[0].Length > 0)
        {
            IQuerySnapshot<Spot_Firebase> documentReference;
            if(lastSpot != null)
            {
                IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .GetDocument(lastSpot.SpotID)
                .GetDocumentSnapshotAsync<Spot_Firebase>();

                documentReference = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .StartingAfter(documentSnapshot)
                .LimitedTo(25)
                .WhereArrayContainsAny("SearchTerms", searchTerms)
                .GetDocumentsAsync<Spot_Firebase>();
            }
            else
            {
                documentReference = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .LimitedTo(25)
                .WhereArrayContainsAny("SearchTerms", searchTerms)
                .GetDocumentsAsync<Spot_Firebase>();
            }

            foreach (var document in documentReference.Documents)
            {
                ImageSource profilePictureImageSource = await document.Data.GetImageSource();
                retVal.Add(new(document.Data, profilePictureImageSource));
            }
        }

        return retVal;
    }

    public static async Task<bool> UpdateClientFollowedList(string followerID, string followedID, bool follow)
    {
        bool retVal = false;

        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current
            .GetCollection("UserData")
            .GetDocument(followerID);
            IDocumentSnapshot<Client_Firebase> documentSnapshot = await documentReference.GetDocumentSnapshotAsync<Client_Firebase>();

            if (documentSnapshot != null && documentSnapshot.Data != null)
            {
                Client_Firebase follower_firebase = documentSnapshot.Data;
                if(follow)
                {
                    if (!follower_firebase.FollowedClients.Contains(followedID))
                    {
                        follower_firebase.FollowedClients.Add(followedID);
                        await documentReference.SetDataAsync(follower_firebase);
                    }
                }
                else
                {
                    while (follower_firebase.FollowedClients.Contains(followedID))
                    {
                        follower_firebase.FollowedClients.Remove(followedID);
                        await documentReference.SetDataAsync(follower_firebase);
                    }
                }                
                retVal = true;
            }
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
        }

        return retVal;
    }
    #endregion

    #region Private Methods
    
    #endregion

    #region Utilities
    private static List<string> GenerateGeohashSearchGrid(FirebaseLocation centerPoint, int searchAreaMagnitude)
    {
        List<string> retVal = [];

        string lastGeohashPivot = centerPoint.Geohash[0];

        for(int step = 0; step < searchAreaMagnitude; step++)
        {
            // Start at North neighbor
            string currentGeohashPivot = LocationManager.Encoder.GetNeighbor(lastGeohashPivot, Geohash.Direction.North);
            lastGeohashPivot = currentGeohashPivot;
            retVal.Add(currentGeohashPivot);
            // Get all neighbors between North and West
            for(int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.West);
                retVal.Add(currentGeohashPivot);
            }
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.South);
                retVal.Add(currentGeohashPivot);
            }
            // Get all neighbors between North and South
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.South);
                retVal.Add(currentGeohashPivot);
            }
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.East);
                retVal.Add(currentGeohashPivot);
            }
            // Get all neighbors between South and East
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.East);
                retVal.Add(currentGeohashPivot);
            }
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.North);
                retVal.Add(currentGeohashPivot);
            }
            // Get all neighbors between East and North
            for (int neighborIndex = 0; neighborIndex <= step; neighborIndex++)
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.North);
                retVal.Add(currentGeohashPivot);
            }
            for (int neighborIndex = 0; neighborIndex < step; neighborIndex++) // Here we use '<' isntead of '<=' to prevent adding North neighbor again
            {
                currentGeohashPivot = LocationManager.Encoder.GetNeighbor(currentGeohashPivot, Geohash.Direction.West);
                retVal.Add(currentGeohashPivot);
            }
        }

        return retVal;
    }
    #endregion
}
