namespace Spots;

public partial class CP_BusinessProfile : ContentPage
{
	private Spot Business;
    public CP_BusinessProfile(Spot _user)
    {
        Business = _user;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        BindingContext = Business;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;

        if (_user.UserID == SessionManager.CurrentSession?.User?.UserID)
        {
            _btnEdit.IsVisible = true;
        }
        else
        {
            _btnEdit.IsVisible = false;
        }
    }

    private void EditPersonalInformation(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateBusinessInformation(Business));
    }

    private void WriteSpotReview(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateSpotPraise(Business));
    }
}