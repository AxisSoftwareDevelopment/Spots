using Geohash;
using System.Reflection.Metadata;

namespace Spots;

public static class LocationManager
{
    public static Location? CurrentLocation { get; private set; }
    public static Geohasher Encoder { get; private set; } = new();

    public static async Task<Location?> GetUpdatedLocaionAsync()
    {
        CurrentLocation = await GetLocation();
        return CurrentLocation;
    }

    public static async Task UpdateLocationAsync()
    {
        CurrentLocation = await GetLocation();
    }

    private static async Task<Location?> GetLocation()
    {
        Location? location;
        try
        {
            location = await Geolocation.Default.GetLastKnownLocationAsync();
        }
        catch (PermissionException)
        {
            bool permissionGranted = false;
            while (!permissionGranted)
            {
                PermissionStatus locationWhenInUse = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                permissionGranted = locationWhenInUse == PermissionStatus.Granted;
            }
            location = await GetLocation();
        }
        catch (Exception ex)
        {
            location = null;
            if(Application.Current != null)
            {
                string[] stringResources = ResourceManagement.GetStringResources(Application.Current.Resources, ["lbl_Error", "lbl_GeolocationError", "lbl_Ok"]);
                await UserInterface.DisplayPopUp_Regular(stringResources[0], stringResources[1], stringResources[2]);
            }
        }
        return location;
    }
}
