namespace Spots;

public partial class CV_FlyoutUserNavigationBar : ContentView
{
	FlyoutPage _FlyoutPage;
	public CV_FlyoutUserNavigationBar(FlyoutPage flyout)
	{
		_FlyoutPage = flyout;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.015;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;

        // This will prevent overlap between burger button (only android) and image button.
#if ANDROID
        _FrameProfilePicture.IsVisible = false;
#endif
    }

    private void ProfilePictureOnClicked(object sender, EventArgs e)
    {
        _FlyoutPage.IsPresented = !_FlyoutPage.IsPresented;
    }
}