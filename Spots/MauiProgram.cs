using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;

#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#else
using Plugin.Firebase.Bundled.Platforms.Android;
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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.RegisterFirebaseServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                return false;
            }));
#elif ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) => {
                CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings());
            }));
#endif
        });

		builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        RunSessionValidation();

        return builder;
    }

    private static CrossFirebaseSettings CreateCrossFirebaseSettings()
    {
        CrossFirebaseSettings settings = new(
            isAnalyticsEnabled: true,
            isAuthEnabled: true,
            isCloudMessagingEnabled: false,
            isDynamicLinksEnabled: false,
            isFirestoreEnabled: true,
            isFunctionsEnabled: true,
            isRemoteConfigEnabled: true,
            isStorageEnabled: true,
            googleRequestIdToken: "443931860533-lvbmdbnge1tdc2dmqpvmqg511ag25cv5.apps.googleusercontent.com");

        return settings;
    }

    private static void RunSessionValidation()
    {
        Task.Run( () => 
        {
            // Load First View
            MainThread.BeginInvokeOnMainThread( async () => 
            {
                if(Application.Current == null)
                {
                    return;
                }
                
                bool userSignedIn = await DatabaseManager.ValidateCurrentSession(); 
                await LocationManager.UpdateLocationAsync(); 
                //await ValidatePermissions(); 
                if (userSignedIn && SessionManager.CurrentSession != null) 
                {
                    if (SessionManager.SessionMode == SessionModes.UserSession && SessionManager.CurrentSession.Client != null)
                    {
                        Application.Current.MainPage = new FP_MainShell(SessionManager.CurrentSession.Client);
                    }
                    else if (SessionManager.CurrentSession.Spot != null)
                    {
                        Application.Current.MainPage = new FP_MainShell(SessionManager.CurrentSession.Spot);
                    }
                }
                else
                {
                    await DatabaseManager.LogOutAsync();
                    Application.Current.MainPage = new NavigationPage(new CP_Login());
                }
            }); 
        }); 
    }
}
