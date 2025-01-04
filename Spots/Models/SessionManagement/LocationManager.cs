using Geohash;
using Plugin.Firebase.Firestore;

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
            if (location == null)
            {
                location = await Geolocation.Default.GetLocationAsync();
            }
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
        catch (FeatureNotEnabledException ex)
        {
            location = null;
            if (Application.Current != null)
            {
                string[] stringResources = ResourceManagement.GetStringResources(Application.Current.Resources, ["lbl_Error", "lbl_GeolocationDisabledError", "lbl_Ok"]);
                await UserInterface.DisplayPopUp_Regular(stringResources[0], ex.Message + "\n" + stringResources[1], stringResources[2]);
            }
        }
        catch
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

public class FirebaseLocation : IFirestoreObject
{
    private double _Latitude = 0;
    private double _Longitude = 0;

    [FirestoreProperty(nameof(Address))]
    public string Address { get; set; }

    [FirestoreProperty(nameof(Latitude))]
    public double Latitude { get; set; }

    [FirestoreProperty(nameof(Longitude))]
    public double Longitude { get; set;}

    [FirestoreProperty(nameof(Geohash))]
    public List<string> Geohash
    {
        get { return [LocationManager.Encoder.Encode(_Latitude, _Longitude)]; }
        private set { }
    }

    public FirebaseLocation()
    {
        Address = "";
        Latitude = 0;
        Longitude = 0;
    }

    public FirebaseLocation(string addr, double lat, double lng)
    {
        Address = addr;
        Latitude = lat;
        Longitude = lng;
    }

    public FirebaseLocation(Location location)
    {
        Address = "";
        Latitude = location.Latitude;
        Longitude = location.Longitude;
    }
}

