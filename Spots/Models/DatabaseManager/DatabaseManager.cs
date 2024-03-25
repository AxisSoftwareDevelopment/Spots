using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;

namespace Spots;
public static class DatabaseManager
{
    const long MAX_IMAGE_STREAM_SIZE = 1 * 1024 * 1024;

    private static readonly IFirebaseAuth firebaseAuth = CrossFirebaseAuth.Current;

    #region Public Methods
    public static async Task<User> LogInUserAsync(string email, string password, bool getUser = true)
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

        User user = new() { UserID = iFUser.Uid };
        if (getUser)
        {
            user = await GetUserDataAsync(iFUser.Uid);
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
            await SaveUserDataAsync(new User() { UserID = CrossFirebaseAuth.Current.CurrentUser.Uid, Email = email });
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
                    CurrentSession.StartSession(user);
                }
                else
                {
                    User user = await GetUserDataAsync(CrossFirebaseAuth.Current.CurrentUser.Uid);
                    CurrentSession.StartSession(user);
                }
                return true;
            }
        }
        catch (Exception) 
        {
            CurrentSession.CloseSession();
        }
        return false;
    }

    public static async Task LogOutAsync()
    {
        await CrossFirebaseAuth.Current.SignOutAsync();
    }

    public static async Task<bool> SaveUserDataAsync(User user, string profilePictureAddress = "")
    {
        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(user.UserID);
            await documentReference.SetDataAsync(new User_Firebase(user, profilePictureAddress));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static async Task<string> SaveProfilePicture(bool isBusiness, string userID, ImageFile imageFile)
    {
        string filePath = $"{(isBusiness ? "Businesses" : "Users")}/{userID}/profilePicture.{imageFile.ContentType?.Split('/')[1] ?? ""}";

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

    public static async Task<List<SpotPraise>> FetchSpotPraises(SpotPraise? lastPraise = null)
    {
        List<SpotPraise> spotPraises = [];
        IQuerySnapshot<SpotPraise_Firebase> documentReference;

        if (lastPraise != null)
        {
            IDocumentSnapshot<SpotPraise_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises").GetDocument(lastPraise?.SpotID).GetDocumentSnapshotAsync<SpotPraise_Firebase>();

            documentReference = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises")
            .OrderBy("CreationDate")
            .StartingAt(documentSnapshot)
            .LimitedTo(5)
            .GetDocumentsAsync<SpotPraise_Firebase>();
        }
        else
        {
            documentReference = await CrossFirebaseFirestore.Current.GetCollection("SpotPraises")
            .OrderBy("CreationDate")
            .LimitedTo(5)
            .GetDocumentsAsync<SpotPraise_Firebase>();
        }

        foreach(var document in documentReference.Documents)
        {
            User author = await GetUserDataAsync(document.Data.AuthorID);
            Spot spot = await GetSpotDataAsync(document.Data.SpotID);
            spotPraises.Add(new(document.Data, author, spot));
        }

        return spotPraises;
    }

    public static async Task<List<Spot>> FetchBusinessUsers_ByNameAsync(string filterParams)
    {
        //TODO: Finish fetching
        List<Spot> retVal = [];

        IQuerySnapshot<Spot_Firebase> documentReference;
        documentReference = await CrossFirebaseFirestore.Current.GetCollection("BusinessData")
            .OrderBy("CreationDate")
            .LimitedTo(25)
            .GetDocumentsAsync<Spot_Firebase>();

        return retVal;
    }
    #endregion

    #region Private Methods
    private async static Task<User> GetUserDataAsync(string userID)
    {
        IDocumentSnapshot<User_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("UserData")
            .GetDocument(userID)
            .GetDocumentSnapshotAsync<User_Firebase>();

        int x = 0;

        // Even if documentSnapshot.Data doesnt support nullable returns, it is still possible to hold a null value.
        User user = documentSnapshot.Data != null ? new(documentSnapshot.Data) : new() { UserID = userID };

        return user;
    }
    private async static Task<Spot> GetSpotDataAsync(string bussinessID)
    {
        IDocumentSnapshot<Spot_Firebase> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("BusinessData")
            .GetDocument(bussinessID)
            .GetDocumentSnapshotAsync<Spot_Firebase>();

        // Even if documentSnapshot.Data doesnt support nullable returns, it is still possible to hold a null value.
        Spot user = documentSnapshot.Data != null ? new(documentSnapshot.Data) : new() { UserID = bussinessID };

        return user;
    }
    #endregion

    #region Utilities
    #endregion
}
