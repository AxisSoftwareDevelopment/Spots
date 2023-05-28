using Spots.Models.SessionManagement;

namespace Spots.Views.Users;

public partial class vwUserProfile : ContentPage
{
	private User user;
	public vwUserProfile(User _user)
	{
		user = _user;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
		BindingContext = user;

		_FrameProfilePicture.HeightRequest = profilePictureDimensions;
		_FrameProfilePicture.WidthRequest = profilePictureDimensions;
	}

	private void EditPersonalInformation(object sender, EventArgs e)
	{
		Navigation.PushAsync(new vwUpdateUserInformation(user));
	}
}