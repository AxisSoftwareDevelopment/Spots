namespace Spots;

public partial class CP_BusinessProfile : ContentPage
{
	private Spot business;
    public CP_BusinessProfile(Spot _user)
    {
        business = _user;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        BindingContext = business;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
    }

    private void EditPersonalInformation(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateBusinessInformation(business));
    }
}