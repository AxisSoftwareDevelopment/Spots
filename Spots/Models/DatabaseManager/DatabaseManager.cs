using Spots;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;

namespace Spots;
public static class DatabaseManager
{
    const long MAX_IMAGE_STREAM_SIZE = 1 * 1024 * 1024;

    public static IFirebaseAuth firebaseAuth = CrossFirebaseAuth.Current;

    #region Public Methods
    public static async Task<User> LogInUserAsync(string email, string password, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
            throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

        IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

        bool isBusinessUser = iFUser.DisplayName != null ? iFUser.DisplayName.Equals("Business") : false;
        if (isBusinessUser)
        {
            await LogOutAsync();
            throw new FirebaseAuthException(FIRAuthError.EmailAlreadyInUse, "txt_LogInError_WrongCredentials_User -> There is alredy a business business with this email.");
        }

        //if(!firebaseUser.IsEmailVerified)
        //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

        User user;
        if (getUser)
        {
            user = await GetUserDataAsync(iFUser.Uid);
            if (!user.bUserDataRetrieved)
                await LogOutAsync();
        }
        else
        {
            user = new();
        }
        return user;
    }

    public static async Task<BusinessUser> LogInBusinessAsync(string email, string password, bool getUser = true)
    {
        string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

        if (userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
            throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

        IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

        bool isBusinessUser = iFUser.DisplayName != null ? iFUser.DisplayName.Equals("Business") : false;
        if (!isBusinessUser)
        {
            await LogOutAsync();
            throw new FirebaseAuthException(FIRAuthError.EmailAlreadyInUse, "txt_LogInError_WrongCredentials_Business -> There is alredy a regular business with this email.");
        }

        //if(!firebaseUser.IsEmailVerified)
        //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

        BusinessUser user;
        if (getUser)
        {
            user = await GetBusinessDataAsync(iFUser.Uid);
            if (!user.userDataRetrieved)
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
        bool x;
        if (isBusinessUser)
            x = await SaveBusinessDataAsync(new BusinessUser() { UserID = CrossFirebaseAuth.Current.CurrentUser.Uid, Email = email, PhoneNumber = phoneNunber, PhoneCountryCode = phoneCountryCode });

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
                    BusinessUser user = await GetBusinessDataAsync(CrossFirebaseAuth.Current.CurrentUser.Uid);
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

    public static async Task<bool> SaveUserDataAsync(User user)
    {
        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(user.UserID);
            await documentReference.SetDataAsync(user);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static async Task<bool> SaveBusinessDataAsync(BusinessUser user)
    {
        try
        {
            IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("BusinessData").GetDocument(user.UserID);
            await documentReference.SetDataAsync(user);
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
    #endregion

    #region Private Methods
    private async static Task<User> GetUserDataAsync(string userID)
    {
        IDocumentSnapshot<User> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("UserData")
            .GetDocument(userID)
            .GetDocumentSnapshotAsync<User>();

        User user = new() { UserID = userID };
        // If there is no _user data in the database
        if (documentSnapshot.Data != null)
        {
            user.Email = documentSnapshot.Data.Email;
            user.FirstName = documentSnapshot.Data.FirstName;
            user.LastName = documentSnapshot.Data.LastName;
            user.BirthDate = documentSnapshot.Data.BirthDate;
            user.ProfilePictureAddress = documentSnapshot.Data.ProfilePictureAddress;
            user.Description = documentSnapshot.Data.Description;
            user.PhoneNumber = documentSnapshot.Data.PhoneNumber;
            user.PhoneCountryCode = documentSnapshot.Data.PhoneCountryCode;
            user.bUserDataRetrieved = true;
            // Getting profile picture
            ImageSource? imageSource = null;
            if (!documentSnapshot.Data.ProfilePictureAddress.Equals("null"))
            {
                Uri imageUri = new(await GetImageDownloadLink(documentSnapshot.Data.ProfilePictureAddress));

                imageSource = ImageSource.FromUri(imageUri);
            }
            if (imageSource != null)
                user.ProfilePictureSource = imageSource;
        }

        return user;
    }
    private async static Task<BusinessUser> GetBusinessDataAsync(string bussinessID)
    {
        IDocumentSnapshot<BusinessUser> documentSnapshot = await CrossFirebaseFirestore.Current
            .GetCollection("BusinessData")
            .GetDocument(bussinessID)
            .GetDocumentSnapshotAsync<BusinessUser>();

        BusinessUser user = new() { UserID = bussinessID };
        // If there is no _user data in the database
        if (documentSnapshot.Data != null)
        {
            user.Email = documentSnapshot.Data.Email;
            user.BrandName = documentSnapshot.Data.BrandName;
            user.BusinessName = documentSnapshot.Data.BusinessName;
            user.Location = documentSnapshot.Data.Location;
            user.ProfilePictureAddress = documentSnapshot.Data.ProfilePictureAddress;
            user.Description = documentSnapshot.Data.Description;
            user.PhoneNumber = documentSnapshot.Data.PhoneNumber;
            user.PhoneCountryCode = documentSnapshot.Data.PhoneCountryCode;
            user.userDataRetrieved = documentSnapshot.Data.BrandName.Length > 0;
        }

        return user;
    }
    #endregion

    #region Utilities
    #endregion
}
