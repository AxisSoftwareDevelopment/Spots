using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Spots;

public partial class CV_DiscoverFeed : ContentView
{
    private readonly object _syncFeed = new object();
    private readonly FeedContext<Spot> CurrentFeedContext = new();
    private FirebaseLocation? CurrentLocation;
    private uint FilterAnimationLength = 300;
    private double FilterSectionHeight = 410;
	public CV_DiscoverFeed()
	{
        InitializeComponent();

        _FiltersLayout.Animate("ExpandCollapse", new Animation(
            x => _FiltersLayout.HeightRequest = x,
            0,
            0),
            length: 0);
        string[] locationOptions = ResourceManagement.GetStringResources(Application.Current.Resources, ["lbl_1km", "lbl_3km", "lbl_5km"]);
        foreach(string option in locationOptions)
        {
            _PickerLocationOptions.Items.Add(option);
        }
        _PickerLocationOptions.SelectedIndex = 0;

        TapGestureRecognizer filterSectionTapGestureRecognizer = new();
        filterSectionTapGestureRecognizer.Tapped += CV_DiscoverFeed_Tapped;
        _FiltersHeaderBar.GestureRecognizers.Add(filterSectionTapGestureRecognizer);

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;

        if (LocationManager.CurrentLocation != null)
        {
            CurrentLocation = new FirebaseLocation(LocationManager.CurrentLocation);
            _cvMiniMap.Pins.Clear();
            _cvMiniMap.MoveToRegion(new MapSpan(LocationManager.CurrentLocation, 0.01, 0.01));
            _cvMiniMap.Pins.Add(new Pin() { Label = "", Location = LocationManager.CurrentLocation });
        }
        else
        {
            LocationManager.UpdateLocationAsync().ConfigureAwait(false);
        }

        _cvMiniMap.HeightRequest = displayInfo.Height * 0.025;

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

    private void _cvMiniMap_MapClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_MapLocationSelector(() => _cvMiniMap.VisibleRegion, SetMiniMapVisibleArea, "Selected Area"));
    }

    private void SetMiniMapVisibleArea(MapSpan mapSpan, string address)
    {
        _cvMiniMap.MoveToRegion(mapSpan);
        _cvMiniMap.Pins.Clear();
        _cvMiniMap.Pins.Add(new Pin() { Label = address, Location = mapSpan.Center });
        _radioSelectedLoc.IsChecked = true;
    }

    private void CV_DiscoverFeed_Tapped(object? sender, TappedEventArgs e)
    {
        _Filters_ExpandCollapse.Rotation = _Filters_ExpandCollapse.Rotation > 0 ? 0 : 180;
        _FiltersLayout.Animate("ExpandCollapse", new Animation(
            x =>_FiltersLayout.HeightRequest = x,
            _Filters_ExpandCollapse.Rotation > 0 ? FilterSectionHeight : 0,
            _Filters_ExpandCollapse.Rotation > 0 ? 0 : FilterSectionHeight,
            easing: Easing.SinOut),
            length: FilterAnimationLength);
    }

    private void _colFeed_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.Count > 0)
        {
            Spot spot = (Spot)e.CurrentSelection[0];
            Navigation.PushAsync(new CP_SpotView(spot));
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
                await UserInterface.DisplayPopUp_Regular("Error", "Couldn't calculate your location.", "ok");
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