using Firebase.Auth;
using Spots.Models.SessionManagement;
using Spots.Views.Users;

namespace Spots.Views.MainMenu;

public partial class vcSideUserMenu : ContentPage
{
    INavigation _Navigation;
    FlyoutPage _Flyout;
    public vcSideUserMenu(INavigation navigation, FlyoutPage flyoutPage)
    {
        _Navigation = navigation;
        _Flyout = flyoutPage;
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        SetBindings(CurrentSession.sessionMode);
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
            _Navigation.PushAsync(new vwUserProfile(CurrentSession.currentUser));
        else
            _Navigation.PushAsync(new vwBusinessProfile(CurrentSession.currentBusiness));
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;

        _Navigation.PushAsync(new vwAppPreferences());
    }

    private async void LogOutOnClickedAsync(object sender, EventArgs e)
    {
        if (await Application.Current.MainPage.DisplayAlert("Log Out", "Are you sure?", "Yes", "Cancel"))
            CurrentSession.CloseSession(shouldUpdateMainPage: true);
    }

    private void SetBindings(SessionMode sessionMode)
    {
        if(sessionMode == SessionMode.UserSession)
        {
            BindingContext = CurrentSession.currentUser;
            _ProfileImage.SetBinding(Image.SourceProperty, "profilePictureSource");
            _lblUserName.SetBinding(Label.TextProperty, "fullName");
        }
        else
        {
            BindingContext = CurrentSession.currentBusiness;
            _ProfileImage.SetBinding(Image.SourceProperty, "profilePictureSource");
            _lblUserName.SetBinding(Label.TextProperty, "businessName");
        }
    }
}