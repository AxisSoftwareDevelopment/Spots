using System.Collections.ObjectModel;

namespace Spots;

public partial class CV_DiscoverFeed : ContentView
{
	public CV_DiscoverFeed()
	{
        ObservableCollection<BusinessUser> businesses = new ObservableCollection<BusinessUser>
        {
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln",
            Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lalala", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)},
            new BusinessUser()
            { BrandName = "MacDonald's",
            BusinessName = "MacDonald's Lincoln street",
            Location = new FirebaseLocation("Lincoln #239 Cumbres", 0, 0)}
        };
        InitializeComponent();
		_colFeed.ItemsSource = businesses;
	}
}