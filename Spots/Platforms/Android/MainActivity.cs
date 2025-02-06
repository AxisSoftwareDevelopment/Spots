using Android.App;
using Android.Content.PM;
using Android.Content;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.DynamicLinks;
using Android.OS;

namespace Spots;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
    }
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CreateNotificationChannelIfNeeded();
        if (Intent != null)
            HandleIntent(Intent);
    }
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        if (intent != null)
            HandleIntent(intent);
    }

    private static void HandleIntent(Intent intent)
    {
        FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        //FirebaseDynamicLinksImplementation.HandleDynamicLinkAsync(intent).Ignore();
    }
    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel();
        }
    }
    private void CreateNotificationChannel()
    {
        var channelId = $"{PackageName}.general";
        var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
        notificationManager.CreateNotificationChannel(channel);
        FirebaseCloudMessagingImplementation.ChannelId = channelId;
        //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.ic_push_small;
    }
}