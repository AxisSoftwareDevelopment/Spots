using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;

namespace Spots.Models
{
    public static class CurrentSession
    {
        // public static string sessionID { get; private set; }
        public static bool sessionOnline { get; private set; } = false;
        public static User currentUser { get; private set; }

        public static bool StartSession(User user)
        {
            if (!sessionOnline)
            {
                currentUser = user;
                sessionOnline = true;
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CloseSession()
        {
            currentUser = new User();
            sessionOnline = false;
        }
    }
}
