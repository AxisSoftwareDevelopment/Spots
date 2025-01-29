using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

using Spots.Models;

namespace Spots;

public partial class CP_SpotView : ContentPage
{
    private Spot CachedSpot;
    public CP_SpotView(Spot spot)
	{
        CachedSpot = spot;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
        BindingContext = CachedSpot;

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;

        Location spotLocation = new(spot.Location.Latitude, spot.Location.Longitude);
        _cvMiniMap.Pins.Clear();
        _cvMiniMap.MoveToRegion(new MapSpan(spotLocation, 0.01, 0.01));
        _cvMiniMap.Pins.Add(new Pin() { Label = spot.Location.Address, Location = spotLocation });
        _cvMiniMap.HeightRequest = profilePictureDimensions;

        _entryAddress.IsVisible = true;
        _btnEdit.IsVisible = true;

        _btnWriteReview.IsVisible = true;
    }

    private void EditPersonalInformation(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateSpot(CachedSpot));
    }

    private void WriteSpotReview(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateSpotPraise(CachedSpot));
    }
}