using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Spots.Models.DatabaseManagement;
using Spots.Models.SessionManagement;
using Microsoft.Maui;
using Spots.Views.MainMenu;

using Spots.Views;
using Spots.Models.ResourceManagement;
using static Microsoft.Maui.ApplicationModel.Permissions;



#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#else
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace Spots;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        Task.Run( async () =>
        {
            
            bool userSignedIn = await DatabaseManager.ValidateCurrentSession();
            // Load First View
            MainThread.BeginInvokeOnMainThread( async () =>
            {
                await LocationManager.UpdateLocationAsync();
                //await ValidatePermissions();
                if (userSignedIn)
                    Application.Current.MainPage = new vwMainShell( DatabaseManager.firebaseAuth.CurrentUser.DisplayName.Equals("Business") );
                else
                    Application.Current.MainPage = new NavigationPage( new vwLogIn() );
            });
        });

        return builder.Build();
	}

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((_,__) => {
                CrossFirebase.Initialize();
                return false;
            }));
#else
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CrossFirebase.Initialize(activity);
            }));
#endif
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }

    //private static async Task ValidatePermissions()
    //{
    //    while (!InternetPermissionsAreValid())
    //    {
    //        string[] stringResources = ResourceManagement.GetStringResources(Application.Current.Resources, new string[] { "lbl_ConnectionError", "txt_ConnectionError", "lbl_Retry" });
    //        await Application.Current.MainPage.DisplayAlert(stringResources[0], stringResources[1], stringResources[2]);

    //    }

    //    bool geolocationAllowed = await GeolocationPermissionsAreValidAsync();
    //    while (!geolocationAllowed)
    //    {
    //        geolocationAllowed = await AskForGeolocationPermissions();
    //    }
    //}

    //public static bool InternetPermissionsAreValid()
    //{
    //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;

    //    return accessType == NetworkAccess.Internet;
    //}

    //public static async Task<bool> GeolocationPermissionsAreValidAsync()
    //{
    //    PermissionStatus locationWhenInUse = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    //    PermissionStatus locationAlways = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

    //    return locationWhenInUse == PermissionStatus.Granted
    //        || locationAlways == PermissionStatus.Granted;
    //}

    //public static async Task<bool> AskForGeolocationPermissions()
    //{
    //    PermissionStatus locationWhenInUse = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    //    PermissionStatus locationAlways = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

    //    return locationWhenInUse == PermissionStatus.Granted
    //       || locationAlways == PermissionStatus.Granted;
    //}
}
