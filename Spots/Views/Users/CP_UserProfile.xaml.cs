namespace Spots;

public partial class CP_UserProfile : ContentPage
{
	private Client user;
	public CP_UserProfile(Client _user)
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
		Navigation.PushAsync(new CP_UpdateUserInformation(user));
	}
}