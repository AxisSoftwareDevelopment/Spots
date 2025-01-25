using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;
using Firebase.Firestore;

namespace Spots;
public static class DatabaseManager
{
    private const string COLLECTION_USER_DATA = "UserData";
    private const string COLLECTION_SPOTS = "Spots";
    private const string COLLECTION_PRAISES = "Praises";
    private const string COLLECTION_TABLES = "Tables";
    const long MAX_IMAGE_STREAM_DIMENSION = 1024;

    #region Public Methods
    public static async Task<Client> LogInUserAsync(string email, string password, FirebaseLocation? lastLocation = null, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if (userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
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

        if (lastLocation != null)
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
        if (id == null || id.Length == 0)
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
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA).GetDocument(user.UserID);
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
                IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_SPOTS).GetDocument(spot.SpotID);
                await documentReference.SetDataAsync(new Spot_Firebase(spot, profilePictureAddress));
            }
            else
            {
                IDocumentReference documentReference = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_SPOTS).AddDocumentAsync(new Spot_Firebase(spot, profilePictureAddress));
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
            ICollectionReference collectionReference = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_SPOTS);

            if (spot.SpotID.Length > 0)
            {
                string profilePictureAddress = "";
                if (imageFile != null)
                {
                    profilePictureAddress = await SaveFile($"{COLLECTION_SPOTS}/{spot.SpotID}", "SpotPicture", imageFile);
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
                    spot_Firebase.ProfilePictureAddress = await SaveFile($"{COLLECTION_SPOTS}/{spot.SpotID}", "SpotsPicture", imageFile);
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

    public static async Task<bool> SaveTableDataAsync(Table table, ImageFile? imageFile = null)
    {
        try
        {
            ICollectionReference collectionReference = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_TABLES);

            if (table.TableID.Length > 0)
            {
                string profilePictureAddress = "";
                if (imageFile != null)
                {
                    profilePictureAddress = await SaveFile($"{COLLECTION_TABLES}/{table.TableID}", "TablePicture", imageFile);
                }
                await collectionReference.GetDocument(table.TableID).SetDataAsync(new Table_Firebase(table, profilePictureAddress));
            }
            else
            {
                Table_Firebase table_Firebase = new(table, "");
                IDocumentReference documentReference = await collectionReference.AddDocumentAsync(table_Firebase);
                if (imageFile != null)
                {
                    table_Firebase.TableID = documentReference.Id;
                    table_Firebase.TablePictureAddress = await SaveFile($"{COLLECTION_TABLES}/{table.TableID}", "TablePicture", imageFile);
                    await documentReference.SetDataAsync(table_Firebase);
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
        IDocumentReference docRef = CrossFirebaseFirestore.Current
            .GetCollection(COLLECTION_USER_DATA)
            .GetDocument(userID);
        Client_Firebase? clientFB = await Firebase_GetDocumentData<Client_Firebase>(docRef);

        Client user = clientFB != null ? new(clientFB, await clientFB.GetImageSource()) : new() { UserID = userID };

        return user;
    }

    public async static Task<Spot> GetSpotDataAsync(string spotID)
    {
        IDocumentReference docRef = CrossFirebaseFirestore.Current
            .GetCollection(COLLECTION_SPOTS)
            .GetDocument(spotID);
        Spot_Firebase? spotFB = await Firebase_GetDocumentData<Spot_Firebase>(docRef);

        Spot spot = new() { SpotID = spotID };
        if (spotFB != null)
        {
            spot = new(spotFB, await spotFB.GetImageSource());
        }

        return spot;
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
                praiseDocumentReference = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES).GetDocument(praise_Firebase.PraiseID);
            }
            else
            {
                praiseDocumentReference = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES).AddDocumentAsync(praise_Firebase);
                bAlreadySaved = true;
            }

            if (praiseDocumentReference.Id.Length > 0 && imageFile != null)
            {
                string attachmentAddress = await SavePraiseAttachment(praiseDocumentReference.Id, praise_Firebase.AuthorID, imageFile);
                praise_Firebase.AttachedPictureAddress = attachmentAddress;
            }

            if ((praiseDocumentReference.Id.Length > 0) && (!bAlreadySaved || imageFile != null))
            {
                await praiseDocumentReference.SetDataAsync(praise_Firebase);
            }

            // We create dummy client and spot so the method doesnt call their info from db, as we dont need it.
            List<SpotPraise> praises = await FetchSpotPraises_Filtered(author: new Client(praise.AuthorID, "", "", DateTime.Now, ""), spot: new Spot(praise.SpotID, "", ""));

            await CrossFirebaseFirestore.Current
                .GetCollection(COLLECTION_SPOTS)
                .GetDocument(praise.SpotID)
                .UpdateDataAsync(new Dictionary<object, object> { { nameof(Spot_Firebase.PraiseCount), praises.Count } });

            return true;
        }
        catch (Exception ex)
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
        try
        {
            await CrossFirebaseFirestore.Current
            .GetCollection(COLLECTION_USER_DATA)
            .GetDocument(userID)
            .UpdateDataAsync((nameof(Client_Firebase.LastLocation), location))
            .WaitAsync(TimeSpan.FromMilliseconds(150)); // I dont like this, but for some reason Firebase failed to return after updating the value succesfully.
        }
        catch { } // Try catch is necessary, because it throws an exception when hitting the timeout.
    }

    public static async Task<List<Client>> FetchClientsByID(List<string> clientIDs)
    {
        List<Client> clients = [];

        if (clientIDs.Count > 0)
        {
            IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA)
                    .WhereArrayContainsAny(nameof(Client_Firebase.ClientID_ForSearch), clientIDs.ToArray());
            
            List<Client_Firebase>? clients_Firebase = await Firebase_GetGocumentsData<Client_Firebase>(query);

            foreach (Client_Firebase client in clients_Firebase?? [])
            {
                clients.Add(new(client, await client.GetImageSource()));
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


        IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA).LimitedTo(25);
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
        if (lastClient != null)
        {
            IDocumentSnapshot<Client_Firebase> docSnapshot = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA)
                .GetDocument(lastClient.UserID)
                .GetDocumentSnapshotAsync<Client_Firebase>();

            query = query.StartingAfter(docSnapshot);
        }

        List<Client_Firebase>? clients_Firebase = await Firebase_GetGocumentsData<Client_Firebase>(query);

        foreach (var clientFB in clients_Firebase?? [])
        {
            if (currentUsrID_ToAvoid == null || clientFB.UserID != currentUsrID_ToAvoid)
            {
                retVal.Add(new(clientFB, await clientFB.GetImageSource()));
            }
        }

        return retVal;
    }

    public static async Task<List<SpotPraise>> FetchSpotPraises_FromFollowedClients(Client client, SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        List<SpotPraise_Firebase>? praises_Firebase;

        if (client.FollowedCount > 0)
        {
            IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES)
                .LimitedTo(5)
                .OrderBy(nameof(SpotPraise_Firebase.CreationDate))
                .WhereArrayContainsAny(nameof(SpotPraise_Firebase.AuthorID_Array), client.Followed.ToArray());
            if (lastPraise != null)
            {
                IDocumentSnapshot<SpotPraise_Firebase> docSnapshot = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES)
                    .GetDocument(lastPraise.PraiseID)
                    .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

                query = query.StartingAfter(docSnapshot);
            }

            praises_Firebase = await Firebase_GetGocumentsData<SpotPraise_Firebase>(query);

            spotPraises = await SpotPraise.GetPraisesFromFirebaseObject(praises_Firebase?? []);
        }

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
        List<SpotPraise> spotPraises = [];
        List<SpotPraise_Firebase>? praises_Firebase;
        IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES)
                .LimitedTo(5);

        if (author != null || authorId != null)
        {
            author ??= await GetClientDataAsync(authorId);
            query = query.WhereEqualsTo(nameof(SpotPraise_Firebase.AuthorID), author.UserID);
        }
        if (spot != null || spotId != null)
        {
            spot ??= await GetSpotDataAsync(spotId);
            query = query.WhereEqualsTo(nameof(SpotPraise_Firebase.SpotID), spot.SpotID);
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
        else
        {
            query = query.OrderBy(nameof(SpotPraise_Firebase.CreationDate));
        }
        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES)
                .GetDocument(lastPraise.PraiseID)
                .GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }

        praises_Firebase = await Firebase_GetGocumentsData<SpotPraise_Firebase>(query);

        spotPraises = await SpotPraise.GetPraisesFromFirebaseObject(praises_Firebase?? [], author: author, spot: spot);

        return spotPraises;
    }

    public static async Task<List<Spot>> FetchSpots_Filtered(string[]? filterParams = null, FirebaseLocation? referenceLocation = null, int? searchAreaInKm = null, string? order = null, Spot? lastSpot = null)
    {
        List<Spot> retVal = [];

        List<Spot_Firebase>? spots_Firebase;
        IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_SPOTS).LimitedTo(25);
        if (filterParams != null)
        {
            query = query.WhereArrayContainsAny(nameof(Spot_Firebase.SearchTerms), filterParams);
        }
        if (referenceLocation != null && searchAreaInKm != null)
        {
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            query = query.WhereArrayContainsAny($"{nameof(Spot_Firebase.Location)}.Geohash", geohashesNearby.ToArray());
        }
        if (order != null)
        {
            query = query.OrderBy(order);
        }
        if (lastSpot != null)
        {
            IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_SPOTS)
            .GetDocument(lastSpot.SpotID)
            .GetDocumentSnapshotAsync<Spot_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }
        spots_Firebase = await Firebase_GetGocumentsData<Spot_Firebase>(query);

        foreach (var spot_Firebase in spots_Firebase?? [])
        {
            retVal.Add(new(spot_Firebase, await spot_Firebase.GetImageSource()));
        }

        return retVal ?? [];
    }

    public static async Task<List<object>> FetchDiscoveryPageItems(DiscoveryPageFilters filters, FirebaseLocation currentLocation, FirebaseLocation selectedLocation, object? lastItem = null)
    {
        List<object> retVal = [];

        switch (filters.Subject)
        {
            case DiscoveryPageFilters.FILTER_SUBJECT.CLIENTS:
                {
                    string orderCode = nameof(Client.FollowersCount);
                    FirebaseLocation location = filters.Location == DiscoveryPageFilters.FILTER_LOCATION.CURRENT
                        ? currentLocation : selectedLocation;
                    List<Client> clients = await FetchClients_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastClient: lastItem != null ? (Client)lastItem : null);
                    foreach (Client client in clients)
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
                    List<Spot> spots = await FetchSpots_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastSpot: lastItem != null ? (Spot)lastItem : null);
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
                    List<SpotPraise> praises = await FetchSpotPraises_Filtered(referenceLocation: location, searchAreaInKm: (int)filters.Area, order: orderCode, lastPraise: lastItem != null ? (SpotPraise)lastItem : null);
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

    public static async Task<List<Table>> FetchTables_Filtered(string? tableOwnerID = null, Table? lastItem = null)
    {
        List<Table> retVal = [];

        List<Table_Firebase>? tables_Firebase;
        IQuery query = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_TABLES).LimitedTo(5);
        if (tableOwnerID != null)
        {
            query = query.WhereArrayContains(nameof(Table_Firebase.TableMembers), tableOwnerID);
        }
        //if (order != null)
        //{
        //    query = query.OrderBy(order);
        //}
        if (lastItem != null)
        {
            IDocumentSnapshot<Table_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection(COLLECTION_TABLES)
            .GetDocument(lastItem.TableID)
            .GetDocumentSnapshotAsync<Table_Firebase>();

            query = query.StartingAfter(documentSnapshot);
        }
        tables_Firebase = await Firebase_GetGocumentsData<Table_Firebase>(query);

        foreach (var table_Firebase in tables_Firebase?? [])
        {
            retVal.Add(new(table_Firebase, await table_Firebase.GetImageSource()));
        }

        return retVal;
    }

    public static async Task<bool?> UpdateLikeOnSpotPraise(string clientID, SpotPraise praise)
    {
        bool? retVal = null;
        IDocumentReference praiseDocument = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_PRAISES).GetDocument(praise.PraiseID);
        IDocumentReference authorDocument = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA).GetDocument(praise.AuthorID);

        retVal = await CrossFirebaseFirestore.Current.RunTransactionAsync<bool?>(transaction => {
            IDocumentSnapshot<SpotPraise_Firebase> praise = transaction.GetDocument<SpotPraise_Firebase>(praiseDocument);
            IDocumentSnapshot<Client_Firebase> author = transaction.GetDocument<Client_Firebase>(authorDocument);
            
            if (praise?.Data != null && author?.Data != null)
            {
                bool alreadyLiked = praise.Data.Likes.Contains(clientID);

                var updatedAuthorCount = author.Data.LikesCount + (alreadyLiked ? -1 : 1);
                var updatedPraiseCount = praise.Data.LikesCount + (alreadyLiked ? -1 : 1);
                var updatedPraiseList = praise.Data.Likes;
                if (alreadyLiked)
                {
                    updatedPraiseList.Remove(clientID);
                }
                else
                {
                    updatedPraiseList.Add(clientID);
                }

                transaction.UpdateData(authorDocument, (nameof(Client_Firebase.LikesCount), updatedAuthorCount));
                transaction.UpdateData(praiseDocument, (nameof(SpotPraise_Firebase.LikesCount), updatedPraiseCount));
                transaction.UpdateData(praiseDocument, (nameof(SpotPraise_Firebase.Likes), updatedPraiseList));

                // We return the contrary to the AlreadyLiked state to represent if the end result is LIKED or not.
                return !alreadyLiked;
            }

            return null;
        });

        return retVal;
    }

    public static async Task<bool> UpdateClientFollowedList(string followerID, string followedID, bool follow)
    {
        bool retVal = false;

        IDocumentReference followedDocument = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA).GetDocument(followedID);
        IDocumentReference followerDocument = CrossFirebaseFirestore.Current.GetCollection(COLLECTION_USER_DATA).GetDocument(followerID);

        bool bDataRetrivalWorked = await CrossFirebaseFirestore.Current.RunTransactionAsync(transaction => {
            bool retVal = false;

            IDocumentSnapshot<Client_Firebase> followed = transaction.GetDocument<Client_Firebase>(followedDocument);
            IDocumentSnapshot<Client_Firebase> follower = transaction.GetDocument<Client_Firebase>(followerDocument);

            if (followed?.Data != null && follower?.Data != null)
            {
                bool followedChangesRequired = follow ? !followed.Data.Followers.Contains(followerID) : followed.Data.Followers.Contains(followerID);
                bool followerChangesRequired = follow ? !follower.Data.Followers.Contains(followedID) : follower.Data.Followers.Contains(followedID);
                List<string> CachedFollowers = [.. followed.Data.Followers];
                List<string> CachedFollowed = [.. follower.Data.Followers];
                if (follow)
                {
                    if (followedChangesRequired) { CachedFollowers.Add(followerID); }
                    if (followerChangesRequired) { CachedFollowed.Add(followedID); }
                }
                else if (!follow)
                {
                    if (followedChangesRequired) { CachedFollowers.Remove(followerID); }
                    if (followerChangesRequired) { CachedFollowed.Remove(followedID); }
                }
                transaction.UpdateData(followedDocument, (nameof(Client_Firebase.Followers), CachedFollowers));
                transaction.UpdateData(followedDocument, (nameof(Client_Firebase.Followed), CachedFollowed));

                retVal = true;
            }

            return retVal;
        });

        return retVal;
    }
    #endregion

    #region Private Methods
    private static async Task<T?> Firebase_GetDocumentData<T>(IDocumentReference docRef)
    {
        T? retval = default;
        int retries = 0;

        while (retval == null || retries < 3)
        {
            T? currVal = default;
            try
            {
                currVal = (await docRef.GetDocumentSnapshotAsync<T>()).Data;
            }
            catch (Exception ex)
            {
                currVal = default;
            }
            finally
            {
                retries++;
                retval = currVal;
            }
        }

        return retval;
    }

    private static async Task<List<T>?> Firebase_GetGocumentsData<T>(IQuery query)
    {
        List<T>? retval = default;
        int cont = 0;

        while (retval == null || cont < 3)
        {
            List<T>? currVal = default;
            try
            {
                currVal = [];
                IQuerySnapshot<T> documentsReference = await query.GetDocumentsAsync<T>();
                foreach (var document in documentsReference.Documents)
                {
                    if (document != null)
                    {
                        currVal.Add(document.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                currVal = default;
            }
            finally
            {
                cont++;
                retval = currVal;
            }
        }

        return retval;
    }
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
