namespace Spots;

public partial class CV_DiscoverFeed : ContentView
{
    private readonly object _syncFeed = new object();
    private readonly FeedContext<Spot> CurrentFeedContext = new();
    private FirebaseLocation? CurrentLocation;
	public CV_DiscoverFeed()
	{
        
        InitializeComponent();

        if(LocationManager.CurrentLocation != null)
        {
            CurrentLocation = new FirebaseLocation(LocationManager.CurrentLocation);
        }
        else
        {
            LocationManager.UpdateLocationAsync().ConfigureAwait(false);
        }
        _colFeed.BindingContext = CurrentFeedContext;
        _refreshView.Command = new Command(async () =>
        {
            await Task.Run(RefreshFeed);
            _refreshView.IsRefreshing = false;
        });
        _colFeed.RemainingItemsThreshold = 1;
        _colFeed.RemainingItemsThresholdReached += OnItemThresholdReached;
        _colFeed.SelectionChanged += _colFeed_SelectionChanged;

        Task.Run(() =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshFeed();
            });
        });
    }

    private void _colFeed_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.Count > 0)
        {
            Spot spot = (Spot)e.CurrentSelection[0];
            Navigation.PushAsync(new CP_BusinessProfile(spot));
            _colFeed.SelectedItem = null;
        }
    }

    private async Task RefreshFeed()
    {
        if(CurrentLocation != null)
        {
            CurrentFeedContext.RefreshFeed(await DatabaseManager.FetchSpotsNearby_Async(CurrentLocation, 1));
        }
        else
        {
            Location? location = await LocationManager.GetUpdatedLocaionAsync();
            if(location != null)
            {
                CurrentLocation = new FirebaseLocation(location);
                CurrentFeedContext.RefreshFeed(await DatabaseManager.FetchSpotsNearby_Async(CurrentLocation, 1));
            }
            else
            {
                // TODO: Create strings
                await UserInterface.DisplayPopUp_Regular("Error", "Couldn't calculate your location", "ok");
            }
        }
    }

    private async void OnItemThresholdReached(object? sender, EventArgs e)
    {
        if (CurrentLocation != null)
        {
            CurrentFeedContext.AddElements(await DatabaseManager.FetchSpotsNearby_Async(CurrentLocation, 1, CurrentFeedContext.LastItemFetched));
        }
        else
        {
            Location? location = await LocationManager.GetUpdatedLocaionAsync();
            if (location != null)
            {
                CurrentLocation = new FirebaseLocation(location);
                CurrentFeedContext.AddElements(await DatabaseManager.FetchSpotsNearby_Async(CurrentLocation, 1, CurrentFeedContext.LastItemFetched));
            }
            else
            {
                // TODO: Create strings
                await UserInterface.DisplayPopUp_Regular("Error", "Couldnt calculate your location", "ok");
            }
        }
    }
}