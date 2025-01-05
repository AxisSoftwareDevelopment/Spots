using Java.Util;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Spots;

public partial class CV_DiscoverFeed : ContentView
{
    private static readonly double FILTER_SECTION_HEIGHT_MAX = 350;
    private static readonly double FILTER_SECTION_HEIGHT_MIN = 290;
    private static readonly uint FILTER_ANIMATION_LENGTH = 300;
    private DiscoveryPageFilters Filters = new();
    private readonly FeedContext<object> CurrentFeedContext = new();
    private FirebaseLocation? CurrentLocation;
    private FirebaseLocation? SelectedLocation;
    private double FilterSectionHeight = FILTER_SECTION_HEIGHT_MIN;
	public CV_DiscoverFeed()
	{
        InitializeComponent();

        #region Filters
        // Set animations
        _FiltersLayout.Animate("ExpandCollapse", new Animation(
            x => _FiltersLayout.HeightRequest = x,
            0,
            0),
            length: 0);
        if(Application.Current != null)
        {
            string[] locationOptions = ResourceManagement.GetStringResources(Application.Current.Resources, ["lbl_1km", "lbl_3km", "lbl_5km"]);
            foreach (string option in locationOptions)
            {
                _PickerLocationOptions.Items.Add(option);
            }
            _PickerLocationOptions.SelectedIndex = 0;
        }

        TapGestureRecognizer filterSectionTapGestureRecognizer = new();
        filterSectionTapGestureRecognizer.Tapped += CV_DiscoverFeed_FiltersTapped;
        _FiltersHeaderBar.GestureRecognizers.Add(filterSectionTapGestureRecognizer);

        // Minimap
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;

        if (LocationManager.CurrentLocation != null)
        {
            CurrentLocation = SelectedLocation = new FirebaseLocation(LocationManager.CurrentLocation);
            _cvMiniMap.Pins.Clear();
            _cvMiniMap.MoveToRegion(new MapSpan(LocationManager.CurrentLocation, 0.01, 0.01));
            _cvMiniMap.Pins.Add(new Pin() { Label = "", Location = LocationManager.CurrentLocation });
        }
        else
        {
            LocationManager.UpdateLocationAsync().ConfigureAwait(false);
        }
        _cvMiniMap.HeightRequest = displayInfo.Height * 0.025;

        // Filter zones
        _radioClientsFilter.CheckedChanged += _radioSubjectFilter_CheckedChanged;
        _radioSpotsFilter.CheckedChanged += _radioSubjectFilter_CheckedChanged;
        _radioSpotReviewsFilter.CheckedChanged += _radioSubjectFilter_CheckedChanged;
        _slayoutTimeFilters.IsVisible = false;
        _slayoutOrderFilters.IsVisible = false;
        #endregion

        #region CollectionView
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
        #endregion
    }

    private void _radioSubjectFilter_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if(e.Value)
        {
            double previousHeight = FilterSectionHeight;
            if (_radioClientsFilter.IsChecked)
            {
                _slayoutOrderFilters.IsVisible = false;
                FilterSectionHeight = FILTER_SECTION_HEIGHT_MIN;
            }
            else if (_radioSpotsFilter.IsChecked)
            {
                _slayoutOrderFilters.IsVisible = false;
                FilterSectionHeight = FILTER_SECTION_HEIGHT_MIN;
            }
            else if(_radioSpotReviewsFilter.IsChecked)
            {
                _slayoutOrderFilters.IsVisible = true;
                FilterSectionHeight = FILTER_SECTION_HEIGHT_MAX;
            }

            if(previousHeight != FilterSectionHeight)
            {
                _FiltersLayout.Animate("ExpandCollapse", new Animation(
                    x => _FiltersLayout.HeightRequest = x,
                    _radioSpotReviewsFilter.IsChecked ? FILTER_SECTION_HEIGHT_MIN : FILTER_SECTION_HEIGHT_MAX,
                    _radioSpotReviewsFilter.IsChecked ? FILTER_SECTION_HEIGHT_MAX : FILTER_SECTION_HEIGHT_MIN,
                    easing: Easing.SinOut),
                    length: FILTER_ANIMATION_LENGTH);
            }
        }
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
        SelectedLocation = new(new(mapSpan.LatitudeDegrees, mapSpan.LongitudeDegrees));
    }

    private void CV_DiscoverFeed_FiltersTapped(object? sender, TappedEventArgs e)
    {
        _Filters_ExpandCollapse.Rotation = _Filters_ExpandCollapse.Rotation > 0 ? 0 : 180;
        _FiltersLayout.Animate("ExpandCollapse", new Animation(
            x =>_FiltersLayout.HeightRequest = x,
            _Filters_ExpandCollapse.Rotation > 0 ? FilterSectionHeight : 0,
            _Filters_ExpandCollapse.Rotation > 0 ? 0 : FilterSectionHeight,
            easing: Easing.SinOut),
            length: FILTER_ANIMATION_LENGTH);
    }

    private async void _colFeed_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.Count > 0)
        {
            if(e.CurrentSelection[0] is Spot spot)
            {
                await Navigation.PushAsync(new CP_SpotView(spot));
                _colFeed.SelectedItem = null;
            }
            else if (e.CurrentSelection[0] is Client client)
            {
                if (SessionManager.CurrentSession?.Client?.UserID != null)
                {
                    List<FollowRegister_Firebase> followRegisters = await DatabaseManager.FetchFollowRegisters(followerID: SessionManager.CurrentSession?.Client?.UserID,
                        followedID: client.UserID);
                    await Navigation.PushAsync(new CP_UserProfile(client, followRegisters.Count > 0));
                }
                _colFeed.SelectedItem = null;
            }
            else if (e.CurrentSelection[0] is SpotPraise praise)
            {
                await Navigation.PushAsync(new CP_SpotPraise(praise));
                _colFeed.SelectedItem = null;
            }
        }
    }

    private async Task RefreshFeed()
    {
        if (CurrentLocation != null && SelectedLocation != null)
        {
            
            CurrentFeedContext.RefreshFeed(await DatabaseManager.FetchDiscoveryPageItems(Filters, CurrentLocation, SelectedLocation));
        }
        else if (Filters.Location == DiscoveryPageFilters.FILTER_LOCATION.CURRENT)
        {
            Location? newLocation = await LocationManager.GetUpdatedLocaionAsync();
            if(newLocation != null)
            {
                CurrentLocation = new FirebaseLocation(newLocation);
                SelectedLocation ??= CurrentLocation;
                CurrentFeedContext.RefreshFeed(await DatabaseManager.FetchDiscoveryPageItems(Filters, CurrentLocation, SelectedLocation));
            }
            else
            {
                // TODO: Create strings
                await UserInterface.DisplayPopUp_Regular("Error", "Couldn't calculate your location.", "ok");
            }
        }
        else
        {
            // TODO: Create strings
            await UserInterface.DisplayPopUp_Regular("Error", "No location selected.", "ok");
        }
    }

    private async void OnItemThresholdReached(object? sender, EventArgs e)
    {
        if (CurrentLocation != null && SelectedLocation != null)
        {
            CurrentFeedContext.AddElements(await DatabaseManager.FetchDiscoveryPageItems(Filters, CurrentLocation, SelectedLocation, CurrentFeedContext.LastItemFetched));
        }
        else
        {
            Location? newLocation = await LocationManager.GetUpdatedLocaionAsync();
            if (newLocation != null)
            {
                CurrentLocation = new FirebaseLocation(newLocation);
                SelectedLocation ??= CurrentLocation;
                CurrentFeedContext.AddElements(await DatabaseManager.FetchDiscoveryPageItems(Filters, CurrentLocation, SelectedLocation, CurrentFeedContext.LastItemFetched));
            }
            else
            {
                // TODO: Create strings
                await UserInterface.DisplayPopUp_Regular("Error", "Couldnt calculate your location", "ok");
            }
        }
    }

    private void _btnApply_Clicked(object sender, EventArgs e)
    {
        // Gather selected filters
        DiscoveryPageFilters.FILTER_SUBJECT subjectSelected = _radioSpotsFilter.IsChecked ? DiscoveryPageFilters.FILTER_SUBJECT.SPOTS
            : _radioClientsFilter.IsChecked ? DiscoveryPageFilters.FILTER_SUBJECT.CLIENTS : DiscoveryPageFilters.FILTER_SUBJECT.SPOT_PRAISES;
        DiscoveryPageFilters.FILTER_LOCATION locationSelected = _radioCurrentLoc.IsChecked ? DiscoveryPageFilters.FILTER_LOCATION.CURRENT
            : DiscoveryPageFilters.FILTER_LOCATION.SELECTED;
        DiscoveryPageFilters.FILTER_AREA areaSelected = (DiscoveryPageFilters.FILTER_AREA)_PickerLocationOptions.SelectedIndex;
        DiscoveryPageFilters.FILTER_TIME timeSelected = _radio1month.IsChecked ? DiscoveryPageFilters.FILTER_TIME.PAST_MONTH
            : _radio6month.IsChecked ? DiscoveryPageFilters.FILTER_TIME.PAST_6_MONTHS
            : _radio1year.IsChecked ? DiscoveryPageFilters.FILTER_TIME.PAST_YEAR
            : DiscoveryPageFilters.FILTER_TIME.ALL_TIME;
        DiscoveryPageFilters.FILTER_ORDER orderSelected = _radioPopular.IsChecked ? DiscoveryPageFilters.FILTER_ORDER.POPULARITY
            : DiscoveryPageFilters.FILTER_ORDER.RECENT;

        // Update filters
        Filters.Subject = subjectSelected;
        Filters.Location = locationSelected;
        Filters.Area = areaSelected;
        Filters.Time = timeSelected;
        Filters.Order = orderSelected;
    }
}

public class DiscoveryPageFilters
{
    public enum FILTER_SUBJECT
    {
        SPOTS,
        CLIENTS,
        SPOT_PRAISES
    }
    public enum FILTER_LOCATION
    {
        CURRENT,
        SELECTED
    }
    public enum FILTER_AREA
    {
        ONE_KM = 1,
        THREE_KM = 3,
        FIVE_KM = 5
    }
    public enum FILTER_TIME
    {
        PAST_MONTH,
        PAST_6_MONTHS,
        PAST_YEAR,
        ALL_TIME
    }
    public enum FILTER_ORDER
    {
        POPULARITY,
        RECENT
    }

    public FILTER_SUBJECT Subject = FILTER_SUBJECT.SPOTS;
    public FILTER_LOCATION Location = FILTER_LOCATION.CURRENT;
    public FILTER_AREA Area = FILTER_AREA.ONE_KM;
    public FILTER_TIME Time = FILTER_TIME.ALL_TIME;
    public FILTER_ORDER Order = FILTER_ORDER.POPULARITY;
}