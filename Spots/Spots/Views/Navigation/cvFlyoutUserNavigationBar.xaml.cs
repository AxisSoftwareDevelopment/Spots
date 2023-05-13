using Spots.Models.SessionManagement;

namespace Spots.Views.Navigation;

public partial class cvFlyoutUserNavigationBar : ContentView
{
	FlyoutPage _FlyoutPage;
	public cvFlyoutUserNavigationBar(FlyoutPage flyout)
	{
		_FlyoutPage = flyout;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.015;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
		_FrameProfilePicture.WidthRequest = profilePictureDimensions;

        _ProfileImage.Clicked += _ProfileImage_Clicked;
	}

    private void _ProfileImage_Clicked(object sender, EventArgs e)
    {
        _FlyoutPage.IsPresented = !_FlyoutPage.IsPresented;
    }

    private void ProfilePictureOnClicked(object sender, EventArgs e)
    {
        _FlyoutPage.IsPresented = !_FlyoutPage.IsPresented;
    }
}