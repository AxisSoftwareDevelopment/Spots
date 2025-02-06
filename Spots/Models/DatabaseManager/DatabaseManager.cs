using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;

using Spots.Models;
using Spots.Firestore;
using Spots.Utilities;
using Spots.ResourceManager;
using Plugin.Firebase.CloudMessaging;

namespace Spots.Database;
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

        Client user = await GetClientDataAsync(iFUser.Uid);
        if (!user.UserDataRetrieved)
        {
            await LogOutAsync();
            return user;
        }

        SessionManager.StartSession(user);

        if (lastLocation != null)
        {
            await UpdateClientLocationAsync(iFUser.Uid, lastLocation);
        }

        await UpdateCurrentUserFCMToken();

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
                await UpdateCurrentUserFCMToken();
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
        Client_Firebase? clientFB = await FirestoreManager.GetDocumentData<Client_Firebase>(COLLECTION_USER_DATA, userID);

        Client user = clientFB != null ? new(clientFB, await clientFB.GetImageSource()) : new() { UserID = userID };

        return user;
    }

    public async static Task<Spot> GetSpotDataAsync(string spotID)
    {
        Spot_Firebase? spotFB = await FirestoreManager.GetDocumentData<Spot_Firebase>(COLLECTION_SPOTS, spotID);

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

    public static Task UpdateClientLocationAsync(string userID, FirebaseLocation location)
    {
        return FirestoreManager.UpdateData(COLLECTION_USER_DATA, userID, nameof(Client_Firebase.LastLocation), location);
    }

    public static async Task UpdateCurrentUserFCMToken()
    {
        string FCMToken = await CloudMessaging.CloudMessagingManager.GetFCMTokenAsync();
        if (FCMToken.Length > 0
            && SessionManager.CurrentSession?.Client != null
            && SessionManager.CurrentSession.Client.FCMToken != FCMToken)
        {
            SessionManager.CurrentSession.Client.FCMToken = FCMToken;
            await UpdateClientFCMToken(SessionManager.CurrentSession.Client.UserID, FCMToken);
        }
    }

    public static Task UpdateClientFCMToken(string userID, string FCMToken)
    {
        return FirestoreManager.UpdateData(COLLECTION_USER_DATA, userID, nameof(Client_Firebase.FCMToken), FCMToken);
    }

    public static async Task<List<Client>> FetchClientsByID(List<string> clientIDs)
    {
        List<Client> clients = [];

        if (clientIDs.Count > 0)
        {
            Dictionary<string, object[]> arrayContainsAnyFilter = new Dictionary<string, object[]>() { { nameof(Client_Firebase.ClientID_ForSearch), clientIDs.ToArray() } };
            
            List<Client_Firebase> clients_Firebase = await FirestoreManager.QueryFiltered<Client_Firebase>(COLLECTION_USER_DATA, filters_ArrayContainsAny: arrayContainsAnyFilter);

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
        string? order = null,
        Client? lastClient = null)
    {
        List<Client> retVal = [];

        const int maxItemsToFetch = 25;
        Dictionary<string, object[]>? arrayContainsAnyFilters = null;
        string? lastItemFetchedID = lastClient?.UserID;
        string? orderBy = order;

        if (nameSearchTerms != null)
        {
            arrayContainsAnyFilters ??= [];
            arrayContainsAnyFilters.Add(nameof(Client_Firebase.SearchTerms), nameSearchTerms);
        }

        List<Client_Firebase>? clients_Firebase = await FirestoreManager.QueryFiltered<Client_Firebase>(COLLECTION_USER_DATA,
            maxItems: maxItemsToFetch,
            filters_ArrayContainsAny: arrayContainsAnyFilters,
            lastItemQueriedID: lastItemFetchedID,
            filters_orderBy: orderBy);

        foreach (var clientFB in clients_Firebase?? [])
        {
            if (currentUsrID_ToAvoid == null || clientFB.UserID != currentUsrID_ToAvoid)
            {
                retVal.Add(new(clientFB, await clientFB.GetImageSource()));
            }
        }

        return retVal;
    }

    public static async Task<List<Client>> FetchClients_Filtered(
        string? currentUsrID_ToAvoid = null,
        FirebaseLocation? referenceLocation = null,
        int? searchAreaInKm = null,
        string? order = null,
        Client? lastClient = null)
    {
        List<Client> retVal = [];

        const int maxItemsToFetch = 25;
        Dictionary<string, object[]>? arrayContainsAnyFilters = null;
        string? lastItemFetchedID = lastClient?.UserID;
        string? orderBy = order;

        if (referenceLocation != null && searchAreaInKm != null)
        {
            arrayContainsAnyFilters ??= [];
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            arrayContainsAnyFilters.Add($"{nameof(Client_Firebase.LastLocation)}.Geohash", geohashesNearby.ToArray());
        }

        List<Client_Firebase>? clients_Firebase = await FirestoreManager.QueryFiltered<Client_Firebase>(COLLECTION_USER_DATA,
            maxItems: maxItemsToFetch,
            filters_ArrayContainsAny: arrayContainsAnyFilters,
            lastItemQueriedID: lastItemFetchedID,
            filters_orderBy: orderBy);

        foreach (var clientFB in clients_Firebase ?? [])
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
        // Show personal praises too
        List<string> praiseAuthorIDs = client.Followed;
        praiseAuthorIDs.Add(client.UserID);

        // Query Filters
        const int maxItemsToFetch = 25;
        Dictionary<string, object[]>? arrayContainsAnyFilters =  new() { { nameof(SpotPraise_Firebase.AuthorID_Array), praiseAuthorIDs.ToArray() } };
        string? lastItemFetchedID = lastPraise?.PraiseID;
        string orderBy = nameof(SpotPraise_Firebase.CreationDate);

        if (client.FollowedCount > 0)
        {
            List<SpotPraise_Firebase> praises_Firebase = await FirestoreManager.QueryFiltered<SpotPraise_Firebase>(COLLECTION_PRAISES,
                maxItems: maxItemsToFetch,
                filters_ArrayContainsAny: arrayContainsAnyFilters,
                lastItemQueriedID: lastItemFetchedID,
                filters_orderBy: orderBy);

            spotPraises = await SpotPraise.GetPraisesFromFirebaseObject(praises_Firebase);
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
        List<SpotPraise> retVal = [];

        const int maxItemsToFetch = 5;
        Dictionary<string, object>? equalsToFilters = null;
        Dictionary<string, object[]>? arrayContainsAnyFilters = null;
        string? lastItemFetchedID = lastPraise?.PraiseID;
        string? orderBy = order;

        if (author != null ^ authorId != null)
        {
            equalsToFilters ??= [];
            author ??= await GetClientDataAsync(authorId?? "");
            equalsToFilters.Add(nameof(SpotPraise_Firebase.AuthorID), author.UserID);
        }
        if (spot != null || spotId != null)
        {
            equalsToFilters ??= [];
            spot ??= await GetSpotDataAsync(spotId?? "");
            equalsToFilters.Add(nameof(SpotPraise_Firebase.SpotID), spot.SpotID);
        }
        if (referenceLocation != null && searchAreaInKm != null)
        {
            arrayContainsAnyFilters ??= [];
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            arrayContainsAnyFilters.Add($"{nameof(SpotPraise_Firebase.SpotLocation)}.Geohash", geohashesNearby.ToArray());
        }

        List<SpotPraise_Firebase> praises_Firebase = await FirestoreManager.QueryFiltered<SpotPraise_Firebase>(COLLECTION_PRAISES, maxItemsToFetch,
            filters_EqualsTo: equalsToFilters,
            filters_ArrayContainsAny: arrayContainsAnyFilters,
            filters_orderBy: orderBy,
            lastItemQueriedID: lastItemFetchedID);

        retVal = await SpotPraise.GetPraisesFromFirebaseObject(praises_Firebase, author: author, spot: spot);

        return retVal;
    }

    public static async Task<List<Spot>> FetchSpots_Filtered(
        FirebaseLocation? referenceLocation = null,
        int? searchAreaInKm = null,
        string? order = null,
        Spot? lastSpot = null)
    {
        List<Spot> retVal = [];

        const int maxItemsToFetch = 25;
        Dictionary<string, object[]>? arrayContainsAnyFilters = null;
        string? lastItemFetchedID = lastSpot?.SpotID;
        string? orderBy = order;

        if (referenceLocation != null && searchAreaInKm != null)
        {
            arrayContainsAnyFilters ??= [];
            List<string> geohashesNearby = GenerateGeohashSearchGrid(referenceLocation, (int)searchAreaInKm);
            geohashesNearby.Add(referenceLocation.Geohash[0]);

            arrayContainsAnyFilters.Add($"{nameof(Spot_Firebase.Location)}.Geohash", geohashesNearby.ToArray());
        }
        
        List<Spot_Firebase> spots_Firebase = await FirestoreManager.QueryFiltered<Spot_Firebase>(COLLECTION_SPOTS,
            maxItems:maxItemsToFetch,
            filters_ArrayContainsAny: arrayContainsAnyFilters,
            lastItemQueriedID: lastItemFetchedID,
            filters_orderBy: orderBy);

        foreach (var spot_Firebase in spots_Firebase)
        {
            retVal.Add(new(spot_Firebase, await spot_Firebase.GetImageSource()));
        }

        return retVal ?? [];
    }

    public static async Task<List<Spot>> FetchSpots_Filtered(
        string[]? filterParams = null,
        string? order = null,
        Spot? lastSpot = null)
    {
        List<Spot> retVal = [];

        const int maxItemsToFetch = 25;
        Dictionary<string, object[]>? arrayContainsAnyFilters = null;
        string? lastItemFetchedID = lastSpot?.SpotID;
        string? orderBy = order;

        if (filterParams != null)
        {
            arrayContainsAnyFilters ??= [];
            arrayContainsAnyFilters.Add(nameof(Spot_Firebase.SearchTerms), filterParams);
        }

        List<Spot_Firebase> spots_Firebase = await FirestoreManager.QueryFiltered<Spot_Firebase>(COLLECTION_SPOTS,
            maxItems: maxItemsToFetch,
            filters_ArrayContainsAny: arrayContainsAnyFilters,
            lastItemQueriedID: lastItemFetchedID,
            filters_orderBy: orderBy);

        foreach (var spot_Firebase in spots_Firebase)
        {
            retVal.Add(new(spot_Firebase, await spot_Firebase.GetImageSource()));
        }

        return retVal ?? [];
    }

    // TODO: Consider moving this method to the Discovery Page logic.
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

        const int maxItemsToFetch = 5;
        Dictionary<string, object>? arrayContainsSingleFilters = null;
        string? lastItemFetchedID = lastItem?.TableID;
        //string? orderBy = order;

        if (tableOwnerID != null)
        {
            arrayContainsSingleFilters ??= [];
            arrayContainsSingleFilters.Add(nameof(Table_Firebase.TableMembers), tableOwnerID);
        }
        List<Table_Firebase> tables_Firebase = await FirestoreManager.QueryFiltered<Table_Firebase>(COLLECTION_TABLES,
            maxItems: maxItemsToFetch,
            filters_ArrayContainsSingle: arrayContainsSingleFilters,
            lastItemQueriedID: lastItemFetchedID);

        foreach (var table_Firebase in tables_Firebase)
        {
            retVal.Add(new(table_Firebase, await table_Firebase.GetImageSource()));
        }

        return retVal;
    }

    public static async Task<bool?> Transaction_UpdateLikeOnSpotPraise(string clientID, SpotPraise praise)
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

    public static async Task<bool> Transaction_UpdateClientFollowedList(string followerID, string followedID, bool follow)
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
