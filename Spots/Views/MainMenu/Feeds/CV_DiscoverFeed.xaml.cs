using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spots;

public partial class CV_DiscoverFeed : ContentView, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;
    private FeedContext<BusinessUser> CurrentFeedContext = new();
    private uint count = 0;
	public CV_DiscoverFeed()
	{
        
        InitializeComponent();

        _colFeed.BindingContext = CurrentFeedContext;
        _refreshView.Command = new Command(() =>
        {
            RefreshFeed();
            _refreshView.IsRefreshing = false;
        });
        _colFeed.RemainingItemsThreshold = 1;
        _colFeed.RemainingItemsThresholdReached += OnItemThresholdReached;
        RefreshFeed();
    }

    private void RefreshFeed()
    {
        count = 0;
        CurrentFeedContext.RefreshFeed(FetchBussinesses(0, 0));
    }

    private void OnItemThresholdReached(object? sender, EventArgs e)
    {
        CurrentFeedContext.AddElements(FetchBussinesses(0, 0));
    }

    private List<BusinessUser> FetchBussinesses(uint startIndex, uint endIndex, List<uint>? alreadyFetchedPrisesIDs = null)
    {
        return [
            new BusinessUser()
            {
                BrandName = count++.ToString()+"MacDonald's",
                BusinessName = "MacDonald's Lincoln street",
                Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)
            },
            new BusinessUser()
            {
                BrandName = count++.ToString() + "MacDonald's",
                BusinessName = "MacDonald's Lincoln street",
                Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)
            },
            new BusinessUser()
            {
                BrandName = count++.ToString() + "MacDonald's",
                BusinessName = "MacDonald's Lincoln street",
                Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)
            },
            new BusinessUser()
            {
                BrandName = count++.ToString() + "MacDonald's",
                BusinessName = "MacDonald's Lincoln street",
                Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)
            },
            new BusinessUser()
            {
                BrandName = count++.ToString() + "MacDonald's",
                BusinessName = "MacDonald's Lincoln street",
                Location = new FirebaseLocation("Lincoln #239 Cumbres lalala lalala lala", 0, 0)
            },
        ];
    }
}