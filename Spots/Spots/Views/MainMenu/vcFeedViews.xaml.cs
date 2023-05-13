using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vcFeedViews : ContentPage
{
    public vcFeedViews()
	{
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.015;

        InitializeComponent();
        BindingContext = CurrentSession.currentUser;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
    }
}