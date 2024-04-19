namespace Spots;

public partial class CP_SideUserMenu : ContentPage
{
	readonly INavigation _Navigation;
    readonly FlyoutPage _Flyout;
    public CP_SideUserMenu(INavigation navigation, FlyoutPage flyoutPage)
    {
        _Navigation = navigation;
        _Flyout = flyoutPage;
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;
        TapGestureRecognizer layoutTapRecognizer = new();
        layoutTapRecognizer.Tapped += (s, e) => {};

        InitializeComponent();
        BindingContext = SessionManager.CurrentSession?.User;
        // We add a gesture recognizer to avoid clickthrough behaviour.
        // this might be fixed in the future and no longer necessary.
        _LayoutView.GestureRecognizers.Add( layoutTapRecognizer );
        //CurrentSession.OnSessionModeChanged += CurrentSession_OnSessionModeChanged;

        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
    }

    private void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;

        if (SessionManager.CurrentSession != null)
        {
            if (SessionManager.SessionMode == SessionModes.UserSession && SessionManager.CurrentSession.Client != null)
            {
                _Navigation.PushAsync(new CP_UserProfile(SessionManager.CurrentSession.Client));
            }
            else if(SessionManager.CurrentSession.Spot != null)
            {
                _Navigation.PushAsync(new CP_BusinessProfile(SessionManager.CurrentSession.Spot));
            }
        }
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;

        _Navigation.PushAsync(new CP_AppPreferences());
    }

    private async void LogOutOnClickedAsync(object sender, EventArgs e)
    {
        if (await UserInterface.DisplayPopPup_Choice("Log Out", "Are you sure?", "Yes", "Cancel"))
            SessionManager.CloseSession(shouldUpdateMainPage: true);
    }
}