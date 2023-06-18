using Spots.Models.SessionManagement;

namespace Spots.Views.Users;

public partial class vwBusinessProfile : ContentPage
{
    private BusinessUser business;
    public vwBusinessProfile(BusinessUser _user)
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
        Navigation.PushAsync(new vwUpdateBusinessInformation(business));
    }
}