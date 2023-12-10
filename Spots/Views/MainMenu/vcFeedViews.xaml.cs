using Microsoft.Maui.Controls;
using Spots.Models.SessionManagement;
using Spots.Views.MainMenu.Feeds;

namespace Spots.Views.MainMenu;

public enum eFeedViewTabs
{
    MAIN_FEED,
    DISCOVERY_FEED
};

public partial class vcFeedViews : TabbedPage
{
    public vcFeedViews(FlyoutPage flyout)
	{
        InitializeComponent();

        NavigationPage.SetTitleView(this, new Navigation.cvFlyoutUserNavigationBar(flyout));


        _Tab1.Content = new cvMainFeed();
        _Tab2.Content = new cvDiscoverFeed();
        IconImageSource = "placeholder_logo.jpg";
    }
}