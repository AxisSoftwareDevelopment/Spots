using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
#if IOS
    using Plugin.Firebase.Firestore.Platforms.iOS.Extensions;
    using Foundation;
#else
    using Plugin.Firebase.Firestore.Platforms.Android.Extensions;
#endif

namespace Spots.Models.DatabaseManagement
{
    public static class DatabaseManager
    {

        #region Public Methods
        public static async Task<User> LogInWithEmailAndPasswordAsync(string email, string password)
        {
            string[] userSignInMethods = await CrossFirebaseAuth.Current.FetchSignInMethodsAsync(email);

            if(userSignInMethods.Length == 0 || !userSignInMethods.Contains("password"))
                throw new FirebaseAuthException(FIRAuthError.InvalidEmail, "Custom Exception -> There was no email and password login method, or none at all.");

            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

            if(!user.IsEmailVerified)
                throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

            Dictionary<string, string> userData = await GetUserDataAsync(user);

            return new User(user.Uid, userData);
        }

        private async static Task<Dictionary<string, string>> GetUserDataAsync(IFirebaseUser user)
        {
            IDocumentSnapshot documentSnapshot = await CrossFirebaseFirestore.Current
                .GetCollection("UserData")
                .GetDocument(user.Uid)
                .GetDocumentSnapshotAsync<Task<IDocumentSnapshot>>();

            return ParseDocumentSnapshot(documentSnapshot);
        }

        //public static async Task<bool> CreateUserAsync(string firstName, string lastName, string email, string password, string birthDate)
        //{
        //    string id = await authorizator.RegisterWithEmailAndPasswordAsync(email, password);
        //    authorizator.LogOut();
        //    Dictionary<string, string> userData = new Dictionary<string, string>
        //    {
        //        { "firstName", firstName },
        //        { "lastName", lastName },
        //        { "birthDate", birthDate },
        //        { "email", email },
        //        { "profilePicture", "null" }
        //    };
        //    return await firestoreManager.SaveNewUserDataAsync(id, userData);
        //}

        //public static void LogOut()
        //{
        //    authorizator.LogOut();
        //}
        #endregion

        #region Utilities
        private static Dictionary<string, string> ParseDocumentSnapshot(IDocumentSnapshot documentSnapshot)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
#if IOS
            NSDictionary<NSString, NSObject> nsDict = documentSnapshot.ToNative().GetData(Firebase.CloudFirestore.ServerTimestampBehavior.Previous);
            foreach (var key in nsDict.Keys)
            {
                dict[key.Description] = nsDict[key].Description;
            }
#else
            IDictionary<string, Java.Lang.Object> dataSnapshot = documentSnapshot.ToNative().Data;
            foreach (string key in dataSnapshot.Keys)
            {
                dict[key] = dataSnapshot[key].ToString();
            }
#endif
            return dict;
        }
        #endregion
    }
}
