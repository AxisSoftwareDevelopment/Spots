using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;

namespace Spots.Models.DatabaseManagement
{
    public static class DatabaseManager
    {
        public static IFirebaseAuth firebaseAuth = CrossFirebaseAuth.Current;
        #region Public Methods
        public static async Task<User> LogInWithEmailAndPasswordAsync(string email, string password)
        {
            string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

            if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
                throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

            IFirebaseUser iFUser = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

            //if(!firebaseUser.IsEmailVerified)
            //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

            return await GetUserDataAsync(iFUser);
        }

        public static async Task<bool> CreateUserAsync(string firstName, string lastName, string email, string password, DateTimeOffset birthDate, string profilePictureAddress)
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);

            string id = CrossFirebaseAuth.Current.CurrentUser?.Uid;
            if (id.Length == 0 || id == null)
                throw new FirebaseAuthException(FIRAuthError.Undefined, "Custom Exception -> Unhandled exception: User not registered correctly.");

            await LogOutAsync();

            User user = new( id, firstName,  lastName, birthDate, email, profilePictureAddress );

            return await SaveNewUserDataAsync(user);
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
        #endregion

        #region Private Methods
        private async static Task<User> GetUserDataAsync(IFirebaseUser firebaseUser)
        {
            IDocumentSnapshot<User> documentSnapshot = await CrossFirebaseFirestore.Current
                .GetCollection("UserData")
                .GetDocument(firebaseUser.Uid)
                .GetDocumentSnapshotAsync<User>();

            User user = new( firebaseUser.Uid,
                documentSnapshot.Data.firstName,
                documentSnapshot.Data.lastName,
                documentSnapshot.Data.birthDate,
                documentSnapshot.Data.email,
                documentSnapshot.Data.profilePictureAddress );

            return user;
        }

        private static async Task<bool> SaveNewUserDataAsync(User user)
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

        #region Utilities
        #endregion
    }
}
