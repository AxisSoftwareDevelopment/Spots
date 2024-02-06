using Spots;
using System.ComponentModel;

namespace Spots;
public enum SessionMode
{
    UserSession,
    BusinessSession
}

public static class CurrentSession
{
    //public static event EventHandler OnSessionModeChanged;
    public static SessionMode sessionMode {  get; private set; } = SessionMode.UserSession;
    public static bool sessionOnline { get; private set; } = false;
    public static User currentUser { get; private set; } = new();
    public static BusinessUser currentBusiness { get; private set; } = new();

    public static bool StartSession(User user)
    {
        if (!sessionOnline)
        {
            sessionMode = SessionMode.UserSession;
            currentUser.UpdateUserData( user );
            sessionOnline = true;
            //OnSessionModeChanged.Invoke( null, EventArgs.Empty);

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
            sessionMode = SessionMode.BusinessSession;
            currentBusiness.UpdateUserData(bussinesUser);
            sessionOnline = true;
            //OnSessionModeChanged.Invoke(null, EventArgs.Empty);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void CloseSession( bool shouldUpdateMainPage = true)
    {
        if(Application.Current != null)
        {
            currentUser = new();
            currentBusiness = new();
            sessionOnline = false;
            Task.Run( DatabaseManager.LogOutAsync );

            if (shouldUpdateMainPage)
            {
                Application.Current.MainPage = new NavigationPage( new CP_Login() );
            }
        }
    }
}
