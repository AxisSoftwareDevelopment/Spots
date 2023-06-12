using Spots.Models.DatabaseManagement;
using Spots.Views;

namespace Spots.Models.SessionManagement
{
    public static class CurrentSession
    {
        public static bool sessionOnline { get; private set; } = false;
        public static User currentUser { get; private set; } = new();

        public static BusinessUser currentBusiness { get; private set; } = new();

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

        public static bool StartSession(BusinessUser bussinesUser)
        {
            if (!sessionOnline)
            {
                currentBusiness.UpdateUserData(bussinesUser);
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
            currentBusiness = new();
            sessionOnline = false;
            Task.Run( DatabaseManager.LogOutAsync );

            if (shouldUpdateMainPage)
                Application.Current.MainPage = new NavigationPage( new vwLogIn() );
        }
    }
}
