using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vcSideUserMenu : ContentPage
{
	public vcSideUserMenu()
	{
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;
        NavigationPage.SetHasNavigationBar(this, false);

        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
    }

    private void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
    {
        var x = NavigationPage.GetHasNavigationBar(this);
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {

    }

    private void LogOutOnClicked(object sender, EventArgs e)
    {
        CurrentSession.CloseSession(shouldUpdateMainPage: true);
    }
}