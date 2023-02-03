using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Xamarin.Essentials;

namespace Spots.Models
{
    static class FireBaseManager
    {

        private static UserSession userSession;

        public static bool StartConnection(string _firebaseApiKey)
        {
            userSession = new UserSession(_firebaseApiKey);

            return true;
        }

        public static async Task<UserCredential> SignInAsync(string _email, string _password)
        {
            return await userSession.authClient.SignInWithEmailAndPasswordAsync(_email, _password);
        }

        public static async Task<UserCredential> SignInAsync(AuthCredential _authCredential)
        {
            return await userSession.authClient.SignInWithCredentialAsync(_authCredential);
        }

        public static async Task<UserCredential> RegisterAsync(string _email, string _password)
        {
            return await userSession.authClient.CreateUserWithEmailAndPasswordAsync(_email, _password);
        }
    }

    internal class UserSession
    {
        internal FirebaseAuthClient authClient { get; set; }
        internal UserSession(string _firebaseApiKey)
        {
            FirebaseAuthConfig config = new FirebaseAuthConfig
            {
                ApiKey = _firebaseApiKey,
                AuthDomain = AppInfo.PackageName,
                Providers = new FirebaseAuthProvider[]
                {
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                },
                UserRepository = new FileUserRepository("SpotsFirebaseSample")
            };

            authClient = new FirebaseAuthClient(config);
        }
    }
}
