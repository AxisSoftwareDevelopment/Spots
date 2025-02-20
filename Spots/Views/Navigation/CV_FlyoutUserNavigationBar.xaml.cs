using eatMeet.Notifications;
using eatMeet.Models;

namespace eatMeet;

public partial class CV_FlyoutUserNavigationBar : ContentView
{
	public CV_FlyoutUserNavigationBar()
	{
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
        FP_MainShell.SetIsPresented(true);
    }
}