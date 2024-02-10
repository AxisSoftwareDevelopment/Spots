namespace Spots;

public partial class CP_SideUserMenu : ContentPage
{
	INavigation _Navigation;
    FlyoutPage _Flyout;
    public CP_SideUserMenu(INavigation navigation, FlyoutPage flyoutPage)
    {
        _Navigation = navigation;
        _Flyout = flyoutPage;
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;
        TapGestureRecognizer layoutTapRecognizer = new TapGestureRecognizer();
        layoutTapRecognizer.Tapped += (s, e) => {};

        InitializeComponent();
        SetBindings(CurrentSession.sessionMode);
        // We add a gesture recognizer to avoid clickthrough behaviour.
        // this might be fixed in the future and no longer necessary.
        _LayoutView.GestureRecognizers.Add( layoutTapRecognizer );
        //CurrentSession.OnSessionModeChanged += CurrentSession_OnSessionModeChanged;


        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
    }

    private void CurrentSession_OnSessionModeChanged(object sender, EventArgs e)
    {
        SetBindings(CurrentSession.sessionMode);
    }

    private void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;
        
        if(CurrentSession.sessionMode == SessionMode.UserSession)
            _Navigation.PushAsync(new CP_UserProfile(CurrentSession.currentUser));
        else
            _Navigation.PushAsync(new CP_BusinessProfile(CurrentSession.currentBusiness));
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;

        _Navigation.PushAsync(new CP_AppPreferences());
    }

    private async void LogOutOnClickedAsync(object sender, EventArgs e)
    {
        if (await UserInterface.DisplayPopUp("Log Out", "Are you sure?", "Yes", "Cancel"))
            CurrentSession.CloseSession(shouldUpdateMainPage: true);
    }

    private void SetBindings(SessionMode sessionMode)
    {
        if(sessionMode == SessionMode.UserSession)
        {
            BindingContext = CurrentSession.currentUser;
            _ProfileImage.SetBinding(Image.SourceProperty, "ProfilePictureSource");
            _lblUserName.SetBinding(Label.TextProperty, "FullName");
        }
        else
        {
            BindingContext = CurrentSession.currentBusiness;
            _ProfileImage.SetBinding(Image.SourceProperty, "ProfilePictureSource");
            _lblUserName.SetBinding(Label.TextProperty, "BusinessName");
        }
    }
}