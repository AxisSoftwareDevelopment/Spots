using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Core.Exceptions;
#if IOS
    using Plugin.Firebase.Firestore.Platforms.iOS.Extensions;
    using Foundation;
#else
    using Plugin.Firebase.Firestore.Platforms.Android.Extensions;
    using Java.Util;
#endif

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

            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, false);

            //if(!firebaseUser.IsEmailVerified)
            //    throw new FirebaseAuthException(FIRAuthError.UserDisabled, "Custon Exception -> Email not verified.");

            return await GetUserDataAsync(user);
        }

        public static async Task<bool> CreateUserAsync(string firstName, string lastName, string email, string password, string birthDate)
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);

            string id = CrossFirebaseAuth.Current.CurrentUser?.Uid;
            if (id.Length == 0)
                throw new FirebaseAuthException(FIRAuthError.Undefined, "Custom Exception -> Unhandled exception: User not registered correctly.");

            await LogOutAsync();

            Dictionary<string, string> userData = new()
            {
                { "firstName", firstName },
                { "lastName", lastName },
                { "birthDate", birthDate },
                { "email", email },
                { "profilePicture", "null" }
            };

            return await SaveNewUserDataAsync(id, userData);
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
            User user = new();
            try
            {
                IDocumentSnapshot documentSnapshot = await CrossFirebaseFirestore.Current
                    .GetCollection("UserData")
                    .GetDocument(firebaseUser.Uid)
                    .GetDocumentSnapshotAsync<Task<IDocumentSnapshot>>();

                user = new(firebaseUser.Uid, ParseFromDocumentSnapshot(documentSnapshot));
            }
            catch (Exception) { }

            return user;
        }

        private static async Task<bool> SaveNewUserDataAsync(string id, Dictionary<string, string> userData)
        {
            object parsedData = ParseFromDictionary(userData);

            await CrossFirebaseFirestore.Current
                .GetCollection("UserData")
                .GetDocument(id)
                .SetDataAsync(parsedData);

            return true;
        }
        #endregion

        #region Utilities
        private static Dictionary<string, string> ParseFromDocumentSnapshot(IDocumentSnapshot documentSnapshot)
        {
            Dictionary<string, string> dict = new();
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

        private static object ParseFromDictionary(Dictionary<string, string> dictionary)
        {
            object parsedObject;
#if IOS
            NSDictionary<NSString, NSObject> plataformObject = new();
            foreach (string key in dictionary.Keys)
            {
                plataformObject.SetValueForKey(NSObject.FromObject(dictionary[key]), (NSString)NSObject.FromObject(key));
            }
            parsedObject = plataformObject;
#else
            HashMap plataformObject = new();
            foreach (string key in dictionary.Keys)
            {
                plataformObject.Put(key, dictionary[key]);
            }
            parsedObject = plataformObject;
#endif
            return parsedObject;
        }
        #endregion
    }
}
