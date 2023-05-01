using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;

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
			.RegisterFirebaseServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((_,__) => {
                CrossFirebase.Initialize();
				FirebaseAuthImplementation.Initialize();
                return false;
            }));
#else
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CrossFirebase.Initialize(activity);
                FirebaseAuthImplementation.Initialize("443931860533-lvbmdbnge1tdc2dmqpvmqg511ag25cv5.apps.googleusercontent.com");
            }));
#endif
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }
}
