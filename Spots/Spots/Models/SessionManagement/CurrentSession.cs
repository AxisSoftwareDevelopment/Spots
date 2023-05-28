using Plugin.Firebase.Auth;
using Spots.Models.DatabaseManagement;
using Spots.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.Models.SessionManagement
{
    public static class CurrentSession
    {
        public static bool sessionOnline { get; private set; } = false;
        public static User currentUser { get; private set; } = new();

        public static bool StartSession(User user)
        {
            if (!sessionOnline)
            {
                currentUser.UpdateUserData( user );
                sessionOnline = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CloseSession( bool shouldUpdateMainPage = true)
        {
            currentUser = new();
            sessionOnline = false;
            Task.Run( DatabaseManager.LogOutAsync );

            if (shouldUpdateMainPage)
                Application.Current.MainPage = new NavigationPage( new vwLogIn() );
        }
    }
}
