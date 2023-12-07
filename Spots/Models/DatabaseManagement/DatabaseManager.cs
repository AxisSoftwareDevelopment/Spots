using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Plugin.Firebase.Storage;
using Spots.Models.ResourceManagement;

namespace Spots.Models.DatabaseManagement
{
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
                user = await GetUserDataAsync(iFUser);
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
                user = await GetBusinessDataAsync(iFUser);
                if (!user.userDataRetrieved)
                    await LogOutAsync();
            }
            else
            {
                user = new();
            }
            return user;
        }

        public static async Task<bool> CreateUserAsync(string email, string password, bool isBusinessUser, string phoneNunber = null, string phoneCountryCode = null)
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);
            await CrossFirebaseAuth.Current.CurrentUser.UpdateProfileAsync(displayName: isBusinessUser ? "Business" : "User");
            bool x;
            if (isBusinessUser)
                x = await SaveBusinessDataAsync(new BusinessUser() { userID = CrossFirebaseAuth.Current.CurrentUser.Uid, email = email, phoneNumber = phoneNunber, phoneCountryCode = phoneCountryCode });

            string id = CrossFirebaseAuth.Current.CurrentUser?.Uid;
            if (id.Length == 0 || id == null)
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
                        BusinessUser user = await GetBusinessDataAsync(CrossFirebaseAuth.Current.CurrentUser);
                        CurrentSession.StartSession(user);
                    }
                    else
                    {
                        User user = await GetUserDataAsync(CrossFirebaseAuth.Current.CurrentUser);
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
                IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(user.sUserID);
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
                IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("BusinessData").GetDocument(user.userID);
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
            string filePath = $"{(isBusiness? "Businesses" : "Users")}/{userID}/profilePicture.{imageFile.ContentType.Split('/')[1]}";

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
        private async static Task<User> GetUserDataAsync(IFirebaseUser firebaseUser)
        {
            IDocumentSnapshot<User> documentSnapshot = await CrossFirebaseFirestore.Current
                .GetCollection("UserData")
                .GetDocument(firebaseUser.Uid)
                .GetDocumentSnapshotAsync<User>();

            User user = new() { sUserID = firebaseUser.Uid, sEmail = firebaseUser.Email };
            // If there is no _user data in the database
            if (documentSnapshot.Data != null)
            {
                user.sFirstName = documentSnapshot.Data.sFirstName;
                user.sLastName = documentSnapshot.Data.sLastName;
                user.dtBirthDate = documentSnapshot.Data.dtBirthDate;
                user.sProfilePictureAddress = documentSnapshot.Data.sProfilePictureAddress;
                user.sDescription = documentSnapshot.Data.sDescription;
                user.sPhoneNumber = documentSnapshot.Data.sPhoneNumber;
                user.sPhoneCountryCode = documentSnapshot.Data.sPhoneCountryCode;
                user.bUserDataRetrieved = true;
                // Getting profile picture
                ImageSource imageSource = null;
                if (!documentSnapshot.Data.sProfilePictureAddress.Equals("null"))
                {
                    Uri imageUri = new( await GetImageDownloadLink(documentSnapshot.Data.sProfilePictureAddress) );

                    imageSource = ImageSource.FromUri(imageUri);
                }
                user.imProfilePictureSource = imageSource;
            }

            return user;
        }

        private async static Task<BusinessUser> GetBusinessDataAsync(IFirebaseUser firebaseUser)
        {
            IDocumentSnapshot<BusinessUser> documentSnapshot = await CrossFirebaseFirestore.Current
                .GetCollection("BusinessData")
                .GetDocument(firebaseUser.Uid)
                .GetDocumentSnapshotAsync<BusinessUser>();

            BusinessUser user = new() { userID = firebaseUser.Uid, email = firebaseUser.Email };
            // If there is no _user data in the database
            if (documentSnapshot.Data != null)
            {
                user.brandName = documentSnapshot.Data.brandName;
                user.businessName = documentSnapshot.Data.businessName;
                user.location = documentSnapshot.Data.location;
                user.profilePictureAddress = documentSnapshot.Data.profilePictureAddress;
                user.description = documentSnapshot.Data.description;
                user.phoneNumber = documentSnapshot.Data.phoneNumber;
                user.phoneCountryCode = documentSnapshot.Data.phoneCountryCode;
                user.userDataRetrieved = documentSnapshot.Data.brandName.Length > 0;
            }

            return user;
        }
        #endregion

        #region Utilities
        #endregion
    }
}
