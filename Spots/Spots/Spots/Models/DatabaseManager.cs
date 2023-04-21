using Org.Apache.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Spots.Models
{
    public static class DatabaseManager
    {
        private static IAuth authorizator;
        private static  IFirestoreManager firestoreManager;

        public static void Load()
        {
            authorizator = DependencyService.Get<IAuth>();
            firestoreManager = DependencyService.Get<IFirestoreManager>();
        }

        #region Private Methods
        #endregion

        #region Public Methods
        public static async Task<User> LogInAsync(string email, string password)
        {
            string userID = await authorizator.LogInWithEmailAndPasswordAsync(email, password);
            Dictionary<string, string> userData = await firestoreManager.GetUserDataAsync(userID);

            return new User(userID, userData);
        }

        public static async Task<bool> CreateUserAsync(string firstName, string lastName, string email, string password, string birthDate)
        {
            string id = await authorizator.RegisterWithEmailAndPasswordAsync(email, password);
            authorizator.LogOut();
            Dictionary<string, string> userData = new Dictionary<string, string>
            {
                { "firstName", firstName },
                { "lastName", lastName },
                { "birthDate", birthDate },
                { "email", email },
                { "profilePicture", "null" }
            };
            return await firestoreManager.SaveNewUserDataAsync(id, userData);
        }
        #endregion
    }
}
