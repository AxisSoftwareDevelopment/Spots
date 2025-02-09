#if ANDROID
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
#endif

namespace Spots;

public partial class TP_FeedViews : Microsoft.Maui.Controls.TabbedPage
{
	public TP_FeedViews()
	{
        InitializeComponent();

#if ANDROID
        On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
#endif

        NavigationPage.SetTitleView(this, new CV_FlyoutUserNavigationBar());

        //while(SessionManager.CurrentSession == null || SessionManager.CurrentSession.Spot == null) { }

        _Tab1.Content = new CV_MainFeed();
        _Tab2.Content = new CV_DiscoverFeed();
        IconImageSource = "logolong.png";
    }
}