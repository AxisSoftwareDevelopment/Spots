using Spots.Notifications;
using Spots.Models;

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
        BindingContext = SessionManager.CurrentSession?.Client;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;

        _btnOpenNotifications.BindingContext = NotificationsManager.Handler;
        _btnOpenSearch.Clicked += _btnOpenSearch_Clicked;
        _btnOpenNotifications.Clicked += _btnOpenNotifications_Clicked;
    }

    private void _btnOpenNotifications_Clicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_Notifications());
    }

    private void _btnOpenSearch_Clicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_SearchPage());
    }

    private void ProfilePictureOnClicked(object sender, EventArgs e)
    {
        _FlyoutPage.IsPresented = !_FlyoutPage.IsPresented;
    }
}