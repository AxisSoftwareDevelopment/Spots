using Spots.Models.SessionManagement;
using Plugin.Firebase.Auth;

namespace Spots.Models.DatabaseManagement
{
    public static class DatabaseManager
    {

        #region Public Methods
        public static async Task<User> LogInAsync(string email, string password)
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password);
            //Dictionary<string, string> userData = GetUserDataAsync(user);

            return new User();//(user.Uid, userData);
        }

        //private static Dictionary<string, string> GetUserDataAsync(IFirebaseUser user)
        //{
        //    Dictionary<string, string> userData = new Dictionary<string, string>
        //    {
        //        //{ "firstName", firstName },
        //        //{ "lastName", lastName },
        //        //{ "birthDate", birthDate },
        //        //{ "email", email },
        //        { "profilePicture", "null" }
        //    };
        //    return userData;
        //}

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
    }
}
