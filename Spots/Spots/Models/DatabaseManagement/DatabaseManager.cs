using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
using Spots.Views.Users;

namespace Spots.Models.DatabaseManagement
{
    public static class DatabaseManager
    {
        public static IFirebaseAuth firebaseAuth = CrossFirebaseAuth.Current;
        #region Public Methods
        public static async Task<User> LogInWithEmailAndPasswordAsync(string email, string password, bool getUser = true)
        {
            string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

            if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
                throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

            IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

            //if(!firebaseUser.IsEmailVerified)
            //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

            User user;
            if (getUser)
            {
                user = await GetUserDataAsync(iFUser);
                if (!user.userDataRetrieved)
                    await LogOutAsync();
            }
            else
            {
                user = new();
            }
            return user;
        }

        public static async Task<bool> CreateUserAsync(string email, string password)
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);

            string id = CrossFirebaseAuth.Current.CurrentUser?.Uid;
            if (id.Length == 0 || id == null)
                throw new FirebaseAuthException(FIRAuthError.Undefined, "Custom Exception -> Unhandled exception: User not registered correctly.");

            await LogOutAsync();
            return true;
        }

        public static async Task ValidateCurrentSession()
        {
            try
            {
                if (CrossFirebaseAuth.Current.CurrentUser != null)
                {
                    User user = await GetUserDataAsync( CrossFirebaseAuth.Current.CurrentUser );
                    CurrentSession.StartSession( user );
                }
            }
            catch (Exception) 
            {
                CurrentSession.CloseSession();
            }
        }

        public static async Task LogOutAsync()
        {
            await CrossFirebaseAuth.Current.SignOutAsync();
        }

        public static async Task<bool> SaveUserDataAsync(User user)
        {
            try
            {
                IDocumentReference documentReference = CrossFirebaseFirestore.Current.GetCollection("UserData").GetDocument(user.userID);
                await documentReference.SetDataAsync(user);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        private async static Task<User> GetUserDataAsync(IFirebaseUser firebaseUser)
        {
            IDocumentSnapshot<User> documentSnapshot = await CrossFirebaseFirestore.Current
                .GetCollection("UserData")
                .GetDocument(firebaseUser.Uid)
                .GetDocumentSnapshotAsync<User>();

            User user = new() { userID = firebaseUser.Uid, email = firebaseUser.Email };
            // If there is no _user data in the database
            if (documentSnapshot.Data != null)
            {
                user.firstName = documentSnapshot.Data.firstName;
                user.lastName = documentSnapshot.Data.lastName;
                user.birthDate = documentSnapshot.Data.birthDate;
                user.profilePictureAddress = documentSnapshot.Data.profilePictureAddress;
                user.description = documentSnapshot.Data.description;
                user.phoneNumber = documentSnapshot.Data.phoneNumber;
                user.phoneCountryCode = documentSnapshot.Data.phoneCountryCode;
                user.userDataRetrieved = true;
            }

            return user;
        }
        #endregion

        #region Utilities
        #endregion
    }
}
