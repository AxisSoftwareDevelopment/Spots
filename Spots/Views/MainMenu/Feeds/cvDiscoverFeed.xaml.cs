using Spots.Models.SessionManagement;
using System.Collections.ObjectModel;

namespace Spots.Views.MainMenu.Feeds;

public partial class cvDiscoverFeed : ContentView
{
	public cvDiscoverFeed()
	{
        ObservableCollection<BusinessUser> businesses = new ObservableCollection<BusinessUser>
        {
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres lalala lalala lala"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"},
            new BusinessUser()
            { brandName = "MacDonald's",
            businessName = "MacDonald's Lincoln street",
            location = "Lincoln #239 Cumbres"}
        };
        InitializeComponent();
		_colFeed.ItemsSource = businesses;
	}
}