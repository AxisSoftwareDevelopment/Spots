using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

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

        Location spotLocation = new(_user.Location.Latitude, _user.Location.Longitude);
        _cvMiniMap.Pins.Clear();
        _cvMiniMap.MoveToRegion(new MapSpan(spotLocation, 0.01, 0.01));
        _cvMiniMap.Pins.Add(new Pin() { Label = _user.Location.Address, Location = spotLocation });
        _cvMiniMap.HeightRequest = profilePictureDimensions;

        if (_user.UserID == SessionManager.CurrentSession?.User?.UserID)
        {
            _btnEdit.IsVisible = true;
            _btnWriteReview.IsVisible = false;
        }
        else
        {
            _entryAddress.IsVisible = false;
            _btnEdit.IsVisible = false;
            if (SessionManager.CurrentSession?.User?.UserType != EUserType.SPOT)
            {
                _btnWriteReview.IsVisible = true;
            }
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