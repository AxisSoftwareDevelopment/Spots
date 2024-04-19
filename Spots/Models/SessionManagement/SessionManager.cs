using Spots;
using System.ComponentModel;

namespace Spots;
public enum SessionModes
{
    UserSession,
    SpotSession
}

public static class SessionManager
{
    public static SessionModes SessionMode {  get; private set; } = SessionModes.UserSession;
    public static UserSession? CurrentSession;

    public static bool StartSession(Client user)
    {
        if (CurrentSession == null)
        {
            SessionMode = SessionModes.UserSession;
            CurrentSession = new(user);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool StartSession(Spot user)
    {
        if (CurrentSession == null)
        {
            SessionMode = SessionModes.SpotSession;
            CurrentSession = new(user);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void CloseSession( bool shouldUpdateMainPage = true)
    {
        if (Application.Current != null)
        {
            CurrentSession = null;
            Task.Run(DatabaseManager.LogOutAsync);

            if (shouldUpdateMainPage)
            {
                Application.Current.MainPage = new NavigationPage(new CP_Login());
            }
        }
    }
}

public class UserSession : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private SessionModes _SessionMode;
    public Client? Client {  get; private set; }
    public Spot? Spot {  get; private set; }
    public IUser? User => _SessionMode == SessionModes.UserSession ? Client : Spot;
    public UserSession(Client user)
    {
        this.Client = user;
        _SessionMode = SessionModes.UserSession;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
    }

    public UserSession(Spot spot)
    {
        this.Spot = spot;
        _SessionMode = SessionModes.SpotSession;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Spot)));
    }

    public void UpdateUserData(Client user)
    {
        if (_SessionMode != SessionModes.UserSession && this.Client != null)
        {
            this.Client.UpdateUserData(user);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
        }
    }

    public void UpdateUserData(Spot spot)
    {
        if(_SessionMode != SessionModes.SpotSession && this.Spot != null)
        {
            this.Spot.UpdateUserData(spot);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Spot)));
        }
    }
}
