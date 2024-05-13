using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;
using Firebase.Abt;
using Android.Hardware.Camera2;
using Firebase.Firestore;

namespace Spots;
public static class DatabaseManager
{
    const long MAX_IMAGE_STREAM_SIZE = 1 * 1024 * 1024;

    private static readonly IFirebaseAuth firebaseAuth = CrossFirebaseAuth.Current;

    #region Public Methods
    public static async Task<Client> LogInUserAsync(string email, string password, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
            throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

        IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

        bool isBusinessUser = iFUser.DisplayName != null && iFUser.DisplayName.Equals("Business");
        if (isBusinessUser)
        {
            await LogOutAsync();
            throw new FirebaseAuthException(FIRAuthError.EmailAlreadyInUse, "txt_LogInError_WrongCredentials_User -> There is alredy a business business with this email.");
        }

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

    public static async Task<Spot> LogInSpotAsync(string email, string password, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if (userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
            throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

        IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

        bool isBusinessUser = iFUser.DisplayName != null && iFUser.DisplayName.Equals("Business");
        if (!isBusinessUser)
        {
            await LogOutAsync();
            throw new FirebaseAuthException(FIRAuthError.EmailAlreadyInUse, "txt_LogInError_WrongCredentials_Business -> There is alredy a regular business with this email.");
        }

        //if(!firebaseUser.IsEmailVerified)
        //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

        Spot user;
        if (getUser)
        {
            user = await GetSpotDataAsync(iFUser.Uid);
            if (!user.UserDataRetrieved)
                await LogOutAsync();
        }
        else
        {
            user = new();
        }
        return user;
    }

    public static async Task<bool> CreateUserAsync(string email, string password, bool isBusinessUser, string phoneNunber = "", string phoneCountryCode = "")
    {
        await CrossFirebaseAuth.Current.CreateUserAsync(email, password);
        await CrossFirebaseAuth.Current.CurrentUser.UpdateProfileAsync(displayName: isBusinessUser ? "Business" : "User");
        if (isBusinessUser)
        {
            await SaveBusinessDataAsync(new Spot() { UserID = CrossFirebaseAuth.Current.CurrentUser.Uid, Email = email, PhoneNumber = phoneNunber, PhoneCountryCode = phoneCountryCode });
        }
        else
        {
            await SaveUserDataAsync(new Client() { UserID = CrossFirebaseAuth.Current.CurrentUser.Uid, Email = email });
        }

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
                if (CrossFirebaseAuth.Current.CurrentUser.DisplayName.Equals("Business"))
                {
                    Spot user = await GetSpotDataAsync(CrossFirebaseAuth.Current.CurrentUser.Uid);
                    SessionManager.StartSession(user);
                }
                else
                {
                    Client user = await GetClientDataAsync(CrossFirebaseAuth.Current.CurrentUser.Uid);
                    SessionManager.StartSession(user);
                }
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

    public static async Task<bool> SaveBusinessDataAsync(Spot user, string profilePictureAddress = "")
    {
        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("BusinessData").GetDocument(user.UserID);
            await documentReference.SetDataAsync(new Spot_Firebase(user, profilePictureAddress));
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

    public async static Task<Spot> GetSpotDataAsync(string bussinessID)
    {
        IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("BusinessData")
            .GetDocument(bussinessID)
            .GetDocumentSnapshotAsync<Spot_Firebase>();

        // Even if documentSnapshot.Data doesnt support nullable returns, it is still possible to hold a null value.
        Spot user = new() { UserID = bussinessID };
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
                praiseDocumentReference = await CrossFirebaseFirestore.Current.GetCollection("Praises").AddDocumentAsync(praise_Firebase);
                bAlreadySaved = true;
            }
            else
            {
                praiseDocumentReference = CrossFirebaseFirestore.Current.GetCollection("Praises").GetDocument(praise_Firebase.PraiseID);
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
            else
            {
                return false;
            }

            if(addToSpotList)
            {
                IDocumentSnapshot<Spot_Firebase> spotSnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection("BusinessData")
                    .GetDocument(praise.SpotID)
                    .GetDocumentSnapshotAsync<Spot_Firebase>();
                if(spotSnapshot.Data != null && !spotSnapshot.Data.Praises.Contains(praise.AuthorID))
                {
                    Spot_Firebase spot = spotSnapshot.Data;
                    spot.Praises.Add(praise.AuthorID);

                    await CrossFirebaseFirestore.Current
                    .GetCollection("BusinessData")
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
        string filePath = $"Users/{userID}/PraieAttachments/{praiseID}.{imageFile.ContentType?.Split('/')[1] ?? ""}";

        IStorageReference storageRef = CrossFirebaseStorage.Current.GetReferenceFromPath(filePath);

        //await storageRef.DeleteAsync();
        await storageRef.PutBytes(imageFile.Bytes).AwaitAsync();
        return filePath;
    }

    public static async Task<string> SaveProfilePicture(bool isBusiness, string userID, ImageFile imageFile)
    {
        string filePath = $"{(isBusiness ? "Businesses" : "Users")}/{userID}/profilePicture.{imageFile.ContentType?.Replace("image/", "") ?? ""}";

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

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromFollowedClients(Client client, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;

        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises")
                .GetDocument(lastPraise.SpotID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises")
                .OrderBy("CreationDate")
                .StartingAfter(documentSnapshot)
                .WhereArrayContainsAny("AuthorID", client.FollowedClients.ToArray())
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises")
                .OrderBy("CreationDate")
                .WhereArrayContainsAny("AuthorID", client.FollowedClients.ToArray())
                .LimitedTo(5)
                .GetDocumentsAsync<SpotPraise_Firebase>();
        }

        foreach(var document in documentReference.Documents)
        {
            Client author = await GetClientDataAsync(document.Data.AuthorID[0]);
            Spot spot = await GetSpotDataAsync(document.Data.SpotID[0]);
            ImageSource? attachment = null;

            if (document.Data.AttachedPictureAddress.Length > 0)
            {
                string downloadAddress = await GetImageDownloadLink(document.Data.AttachedPictureAddress);
                Uri imageUri = new(downloadAddress);

                attachment = ImageSource.FromUri(imageUri);
            }

            spotPraises.Add(new(document.Data, author, spot, attachment));
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
                .GetDocument(lastSpot.UserID)
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

    public static async Task<List<Client>> FetchClients_ByNameAsync(string filterParams, Client? lastClient = null)
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
                    .LimitedTo(25)
                    .WhereArrayContainsAny("SearchTerms", searchTerms)
                    .GetDocumentsAsync<Client_Firebase>();
            }
            else
            {
                documentReference = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                    .LimitedTo(25)
                    .WhereArrayContainsAny("SearchTerms", searchTerms)
                    .GetDocumentsAsync<Client_Firebase>();
            }
            
            foreach (var document in documentReference.Documents)
            {
                ImageSource profilePictureImageSource = await document.Data.GetImageSource();
                retVal.Add(new(document.Data, profilePictureImageSource));
            }
        }

        return retVal;
    }

    public static async Task<List<Spot>> FetchSpots_ByNameAsync(string filterParams, Spot? lastSpot = null)
    {
        List<Spot> retVal = [];
        string[] searchTerms = [filterParams.ToUpper().Trim()];

        if (searchTerms[0].Length > 0)
        {
            IQuerySnapshot<Spot_Firebase> documentReference;
            if(lastSpot != null)
            {
                IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
                .GetDocument(lastSpot.UserID)
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
                    }
                }
                else
                {
                    while (follower_firebase.FollowedClients.Contains(followedID))
                    {
                        follower_firebase.FollowedClients.Remove(followedID);
                    }
                }

                await documentReference.SetDataAsync(follower_firebase);
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
