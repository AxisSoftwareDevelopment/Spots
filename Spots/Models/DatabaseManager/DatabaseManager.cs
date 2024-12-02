using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;
using Firebase.Firestore;

namespace Spots;
public static class DatabaseManager
{
    const long MAX_IMAGE_STREAM_DIMENSION = 1024;

    #region Public Methods
    public static async Task<Client> LogInUserAsync(string email, string password, FirebaseLocation? lastLocation = null, bool getUser = true)
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

        if(lastLocation != null)
        {
            await UpdateClientLocationAsync(iFUser.Uid, lastLocation);
        }

        return user;
    }

    public static async Task<bool> CreateUserAsync(string email, string password)
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

    public static async Task LogOutAsync(FirebaseLocation? lastLocation = null)
    {
        if (lastLocation != null)
        {
            await UpdateClientLocationAsync(CrossFirebaseAuth.Current.CurrentUser.Uid, lastLocation);
        }
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

    public static async Task<bool> SaveSpotDataAsync(Spot spot, string profilePictureAddress)
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
                IDocumentReference documentReference = await CrossFirebaseFirestore.Current.GetCollection("Spots").AddDocumentAsync(new Spot_Firebase(spot, profilePictureAddress));
            }
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Database Exception", ex.Message, "Ok");
            return false;
        }

        return true;
    }

    public static async Task<bool> SaveSpotDataAsync(Spot spot, ImageFile? imageFile = null)
    {
        try
        {
            ICollectionReference collectionReference = CrossFirebaseFirestore.Current.GetCollection("Spots");

            if (spot.SpotID.Length > 0)
            {
                string profilePictureAddress = "";
                if (imageFile != null)
                {
                    profilePictureAddress = await SaveFile($"Spots/{spot.SpotID}", "SpotPicture", imageFile);
                }
                await collectionReference.GetDocument(spot.SpotID).SetDataAsync(new Spot_Firebase(spot, profilePictureAddress));
            }
            else
            {
                Spot_Firebase spot_Firebase = new(spot, "");
                IDocumentReference documentReference = await collectionReference.AddDocumentAsync(spot_Firebase);
                if (imageFile != null)
                {
                    spot_Firebase.SpotID = documentReference.Id;
                    spot_Firebase.ProfilePictureAddress = await SaveFile($"Spots/{spot.SpotID}", "SpotsPicture", imageFile);
                    await documentReference.SetDataAsync(spot_Firebase);
                }
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

    public static async Task<bool> SaveSpotPraiseData(SpotPraise praise, ImageFile? imageFile = null)
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

            // We create dummy client and spot so the method doesnt call their info from db, as we dont need it.
            List<SpotPraise> praises = await FetchSpotPraises_Filtered(author: new Client(praise.AuthorID, "", "", DateTime.Now, ""), spot: new Spot(praise.SpotID, "", ""));

            await CrossFirebaseFirestore.Current
                .GetCollection("Spots")
                .GetDocument(praise.SpotID)
                .UpdateDataAsync(new Dictionary<object, object> { { nameof(Spot_Firebase.PraiseCount), praises.Count} });

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
        string filePath = $"Users/{userID}/PraiseAttachments/{praiseID}.{imageContentTypeArgs[imageContentTypeArgs.Length - 1] ?? ""}";

        IStorageReference storageRef = CrossFirebaseStorage.Current.GetReferenceFromPath(filePath);

        //await storageRef.DeleteAsync();
        await storageRef.PutBytes(imageFile.Bytes).AwaitAsync();
        return filePath;
    }

    public static async Task<string> SaveFile(string path, string fileName, ImageFile imageFile)
    {
        string filePath = $"{path}/{fileName}.{imageFile.ContentType?.Replace("image/", "") ?? ""}";

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

    public async static Task UpdateClientLocationAsync(string userID, FirebaseLocation location)
    {
        await CrossFirebaseFirestore.Current
            .GetCollection("UserData")
            .GetDocument(userID)
            .UpdateDataAsync(new Dictionary<object, object>() { { nameof(Client_Firebase.LastLocation), location } });
    }

    public static async Task<List<Client>> FetchClientsByID(List<string>clientIDs)
    {
        List<Client> clients = [];

        if(clientIDs.Count > 0)
        {
            IQuerySnapshot<Client_Firebase> documentReference = await CrossFirebaseFirestore.Current.GetCollection("UserData")
                    .WhereArrayContainsAny(nameof(Client_Firebase.ClientID_ForSearch), clientIDs.ToArray())
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

    public static async Task<List<Client>> FetchClients_Filtered(
        string[]? nameSearchTerms = null,
        string? currentUsrID_ToAvoid = null,
        FirebaseLocation? referenceLocation = null,
        int? searchAreaInKm = null,
        string? order = null,
        Client? lastClient = null)
    {
        List<Client> retVal = [];

        
        IQuery query = CrossFirebaseFirestore.Current.GetCollection("UserData").LimitedTo(25);
        if (lastClient != null)
        {
            IDocumentSnapshot<Client_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("UserData")
            .GetDocument(lastClient.UserID)
            .GetDocumentSnapshotAsync<Client_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }
        if (nameSearchTerms != null)
        {
            query = query.WhereArrayContainsAny(nameof(Client_Firebase.SearchTerms), nameSearchTerms);
        }
        if (referenceLocation != null && searchAreaInKm != null)
        {
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            query = query.WhereArrayContainsAny($"{nameof(Client_Firebase.LastLocation)}.Geohash", geohashesNearby.ToArray());
        }
        if (order != null)
        {
            query = query.OrderBy(order);
        }

        IQuerySnapshot<Client_Firebase> documentReference = await query.GetDocumentsAsync<Client_Firebase>();

        foreach (var document in documentReference.Documents)
        {
            if (currentUsrID_ToAvoid == null || document.Data.UserID != currentUsrID_ToAvoid)
            {
                ImageSource profilePictureImageSource = await document.Data.GetImageSource();
                retVal.Add(new(document.Data, profilePictureImageSource));
            }
        }

        return retVal;
    }

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromFollowedClients(Client client, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;
        List<FollowRegister_Firebase> followRegisters = await FetchFollowRegisters(followerID: client.UserID);
        List<string> clientIDs = [client.UserID];
        foreach (FollowRegister_Firebase register in followRegisters)
        {
            if (!clientIDs.Contains(register.FollowedID))
            {
                clientIDs.Add(register.FollowedID);
            }
        }

        IQuery query = CrossFirebaseFirestore.Current.GetCollection("Praises")
            .LimitedTo(5)
            .OrderBy(nameof(SpotPraise_Firebase.CreationDate))
            .WhereArrayContainsAny(nameof(SpotPraise_Firebase.AuthorID), clientIDs.ToArray());
        if (lastPraise != null)
        {
            
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }

        documentReference = await query.GetDocumentsAsync<SpotPraise_Firebase>();

        spotPraises = await SpotPraise.GetPraisesFromFirebaseObject(documentReference.Documents.ToList());

        return spotPraises;
    }

    //public static async Task<List<SpotPraise>> FetchSpotPraises_Filtered(Client? author = null, string? spot = null, SpotPraise? lastPraise = null)
    //{
    //    Spot? spot_object = spot != null ? await GetSpotDataAsync(spot) : null;

    //    return await FetchSpotPraises_Filtered(author: author, spot: spot_object, lastPraise);
    //}

    //public static async Task<List<SpotPraise>> FetchSpotPraises_Filtered(string? author = null, Spot? spot = null, SpotPraise? lastPraise = null)
    //{
    //    Client? client_object = author != null ? await GetClientDataAsync(author) : null;

    //    return await FetchSpotPraises_Filtered(author: client_object, spot: spot, lastPraise);
    //}

    public static async Task<List<SpotPraise>> FetchSpotPraises_Filtered(
        string? authorId = null,
        Client? author = null,
        string? spotId = null,
        Spot? spot = null,
        FirebaseLocation? referenceLocation = null,
        int? searchAreaInKm = null,
        string? order = null,
        SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises;
        IQuerySnapshot<SpotPraise_Firebase> documentReference;
        IQuery query = CrossFirebaseFirestore.Current.GetCollection("Praises")
                .OrderBy(nameof(SpotPraise_Firebase.CreationDate))
                .LimitedTo(5);

        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Praises")
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }
        if (author != null || authorId != null)
        {
#pragma warning disable CS8604 // Possible null reference argument warning. Removed because it is not possible due to if and null asignation combination.
            author ??= await GetClientDataAsync(authorId);
#pragma warning restore CS8604 // Possible null reference argument.
            string[] id = [author.UserID];
            query = query.WhereArrayContainsAny(nameof(SpotPraise_Firebase.AuthorID), id);
        }
        if (spot != null || spotId != null)
        {
#pragma warning disable CS8604 // Possible null reference argument. Removed because it is not possible due to if and null asignation combination.
            spot ??= await GetSpotDataAsync(spotId);
#pragma warning restore CS8604 // Possible null reference argument.
            string[] id = [spot.SpotID];
            query = query.WhereArrayContainsAny(nameof(SpotPraise_Firebase.SpotID), id);
        }
        if (referenceLocation != null && searchAreaInKm != null)
        {
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            query = query.WhereArrayContainsAny($"{nameof(SpotPraise_Firebase.SpotLocation)}.Geohash", geohashesNearby.ToArray());
        }
        if (order != null)
        {
            query = query.OrderBy(order);
        }

        documentReference = await query.GetDocumentsAsync<SpotPraise_Firebase>();

        spotPraises = await SpotPraise.GetPraisesFromFirebaseObject(documentReference.Documents.ToList(), author: author, spot: spot);

        return spotPraises;
    }

    public static async Task<List<Spot>> FetchSpots_Filtered(string[]? filterParams = null, FirebaseLocation? referenceLocation = null, int? searchAreaInKm = null, string? order = null, Spot? lastSpot = null)
    {
        List<Spot> retVal = [];

        IQuerySnapshot<Spot_Firebase> documentReference;
        IQuery query = CrossFirebaseFirestore.Current.GetCollection("Spots").LimitedTo(25);
        if(filterParams != null)
        {
            query = query.WhereArrayContainsAny(nameof(Spot_Firebase.SearchTerms), filterParams);
        }
        if(referenceLocation != null && searchAreaInKm != null)
        {
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            query = query.WhereArrayContainsAny($"{nameof(Spot_Firebase.Location)}.Geohash", geohashesNearby.ToArray());
        }
        if (lastSpot != null)
        {
            IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("Spots")
            .GetDocument(lastSpot.SpotID)
            .GetDocumentSnapshotAsync<Spot_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }
        if (order != null)
        {
            query = query.OrderBy(order);
        }
        documentReference = await query.GetDocumentsAsync<Spot_Firebase>();

        foreach (var document in documentReference.Documents)
        {
            ImageSource profilePictureImageSource = await document.Data.GetImageSource();
            retVal.Add(new(document.Data, profilePictureImageSource));
        }

        return retVal;
    }

    public static async Task<List<FollowRegister_Firebase>> FetchFollowRegisters(string? followerID = null, string? followedID = null)
    {
        List<FollowRegister_Firebase> retVal = [];

        IQuery query = CrossFirebaseFirestore.Current.GetCollection("Follows").OrderBy(nameof(FollowRegister_Firebase.FollowerID));
        if(followedID != null)
        {
            query.WhereEqualsTo(nameof(FollowRegister_Firebase.FollowedID), followedID);
        }
        if(followerID != null)
        {
            query.WhereEqualsTo(nameof(FollowRegister_Firebase.FollowerID), followerID);
        }
        IQuerySnapshot<FollowRegister_Firebase> querySnapshot = await query.GetDocumentsAsync<FollowRegister_Firebase>();
        
        foreach(IDocumentSnapshot<FollowRegister_Firebase> doc in querySnapshot.Documents)
        {
            if(doc.Data != null)
            {
                retVal.Add(doc.Data);
            }
        }

        return retVal;
    }

    public static async Task<List<object>> FetchDiscoveryPageItems(DiscoveryPageFilters filters, FirebaseLocation currentLocation, FirebaseLocation selectedLocation, object? lastItem = null)
    {
        List<object> retVal = [];

        switch(filters.Subject)
        {
            case DiscoveryPageFilters.FILTER_SUBJECT.CLIENTS:
                {
                    string orderCode = nameof(Client.FollowersCount);
                    FirebaseLocation location = filters.Location == DiscoveryPageFilters.FILTER_LOCATION.CURRENT
                        ? currentLocation : selectedLocation;
                    List<Client> clients = await FetchClients_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastClient: (Client?)lastItem);
                    foreach(Client client in clients)
                    {
                        retVal.Add(client);
                    }
                    break;
                }
            case DiscoveryPageFilters.FILTER_SUBJECT.SPOTS:
                {
                    string orderCode = nameof(Spot.PraiseCount);
                    FirebaseLocation location = filters.Location == DiscoveryPageFilters.FILTER_LOCATION.CURRENT
                        ? currentLocation : selectedLocation;
                    List<Spot> spots = await FetchSpots_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastSpot: (Spot?)lastItem);
                    foreach (Spot spot in spots)
                    {
                        retVal.Add(spot);
                    }
                    break;
                }
            case DiscoveryPageFilters.FILTER_SUBJECT.SPOT_PRAISES:
                {
                    string orderCode = filters.Order switch
                    {
                        DiscoveryPageFilters.FILTER_ORDER.POPULARITY => nameof(SpotPraise_Firebase.CreationDate),
                        _ => nameof(SpotPraise_Firebase.LikesCount)
                    };
                    FirebaseLocation location = filters.Location == DiscoveryPageFilters.FILTER_LOCATION.CURRENT
                        ? currentLocation : selectedLocation;
                    List<SpotPraise> praises = await FetchSpotPraises_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastPraise: (SpotPraise?)lastItem);
                    foreach (SpotPraise praise in praises)
                    {
                        retVal.Add(praise);
                    }
                    break;
                }
            default:
                break;
        }

        return retVal;
    }

    public static async Task<bool> UpdateClientFollowedList(string followerID, string followedID, bool follow)
    {
        bool retVal = false;

        try
        {
            List<FollowRegister_Firebase> followRegisters = await FetchFollowRegisters(followedID: followedID);
            int finalFollowersCount = followRegisters.Count;
            int preExistantFollowsCount = 0;

            foreach (FollowRegister_Firebase register in followRegisters)
            {
                if (register.FollowerID == followerID)
                {
                    if(!follow)
                    {
                        await CrossFirebaseFirestore.Current.GetCollection("Follows").GetDocument(register.RegisterID).DeleteDocumentAsync();
                        finalFollowersCount--;
                    }
                    preExistantFollowsCount++;
                }
            }

            if (follow && preExistantFollowsCount == 0)
            {
                await CrossFirebaseFirestore.Current.GetCollection("Follows").CreateDocument().SetDataAsync(new FollowRegister_Firebase(followerID, followedID));
                finalFollowersCount++;
            }

            if(followRegisters.Count !=  finalFollowersCount)
            {
                await CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(followedID).SetDataAsync((nameof(Client_Firebase.FollowersCount), finalFollowersCount));
            }

            retVal = true;
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
