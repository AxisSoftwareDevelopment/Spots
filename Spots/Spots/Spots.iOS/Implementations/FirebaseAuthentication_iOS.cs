using Firebase.Auth;
using Spots.iOS.Implementations;
using Spots.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthentication_iOS))]
namespace Spots.iOS.Implementations
{
    internal class FirebaseAuthentication_iOS : IAuth
    {
        public async Task<string> LogInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var sesion = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return sesion.User.Uid;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<string> RegisterWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var sesion = await Auth.DefaultInstance.CreateUserAsync(email, password);
                //var verification = sesion.User.SendEmailVerificationAsync(); -> This is for Android, have to adapt.
                return sesion.User.Uid;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}