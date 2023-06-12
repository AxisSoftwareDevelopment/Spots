using Firebase.Auth;
using Spots.Models.SessionManagement;
using Spots.Views.Users;

namespace Spots.Views.MainMenu;

public partial class vcSideUserMenu : ContentPage
{
    INavigation _Navigation;
    FlyoutPage _Flyout;
	public vcSideUserMenu( INavigation navigation, FlyoutPage flyoutPage)
	{
        _Navigation = navigation;
        _Flyout = flyoutPage;
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;

        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
    }

    private void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
    {
        _Flyout.IsPresented = false;
        
        _Navigation.PushAsync(new vwUserProfile(CurrentSession.currentUser));
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {

    }

    private async void LogOutOnClickedAsync(object sender, EventArgs e)
    {
        if (await Application.Current.MainPage.DisplayAlert("Log Out", "Are you sure?", "Yes", "Cancel"))
            CurrentSession.CloseSession(shouldUpdateMainPage: true);
    }
}