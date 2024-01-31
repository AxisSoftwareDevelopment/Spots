#if ANDROID
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
#endif

namespace Spots;

public partial class TP_FeedViews : Microsoft.Maui.Controls.TabbedPage
{
	public TP_FeedViews(FlyoutPage flyout)
	{
        InitializeComponent();

#if ANDROID
        On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
#endif

        NavigationPage.SetTitleView(this, new CV_FlyoutUserNavigationBar(flyout));

        _Tab1.Content = new CV_MainFeed();
        _Tab2.Content = new CV_DiscoverFeed();
        IconImageSource = "placeholder_logo.jpg";
    }
}