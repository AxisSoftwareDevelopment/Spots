using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vcFeedViews : ContentPage
{
    FlyoutPage _FlyoutPage;
    public vcFeedViews(FlyoutPage flyoutPage)
	{
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.015;

        _FlyoutPage = flyoutPage;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
    }

    private void ProfilePictureOnClicked(object sender, EventArgs e)
    {
        var x = NavigationPage.GetHasNavigationBar(this);
        _FlyoutPage.IsPresented = !_FlyoutPage.IsPresented;
    }
}