using Firebase.Auth;
using Spots.Droid.Implementations;
using Spots.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthentication_Android))]
namespace Spots.Droid.Implementations
{
    public class FirebaseAuthentication_Android : IAuth
    {
        public async Task<string> LogInWithEmailAndPasswordAsync(string email, string password)
        {
            var sesion = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
            return sesion.User.Uid;
        }

        public async Task<string> RegisterWithEmailAndPasswordAsync(string email, string password)
        {
            var sesion = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
            //var verification = sesion.User.SendEmailVerificationAsync();
            return sesion.User.Uid;
        }
    }
}