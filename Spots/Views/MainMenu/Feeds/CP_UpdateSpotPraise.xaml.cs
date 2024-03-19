namespace Spots;

public partial class CP_UpdateSpotPraise : ContentPage
{
	private SpotPraise? MainSpotPraise;
    private readonly FeedContext<BusinessUser> SearchBoxContext = new();
    public CP_UpdateSpotPraise(SpotPraise? spotPraise = null)
	{
		MainSpotPraise = spotPraise;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        _colSearchBarCollectionView.BindingContext = SearchBoxContext;
        _colSearchBarCollectionView.MaximumHeightRequest = profilePictureDimensions * 2;
        _entrySpotSearchBar.TextChanged += _entrySpotSearchBar_TextChanged;

        _FrameSpotPicture.HeightRequest = profilePictureDimensions;
        _FrameSpotPicture.WidthRequest = profilePictureDimensions;
    }

    private void _entrySpotSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshSearchResults(_entrySpotSearchBar.Text);
    }

    private void RefreshSearchResults(string searchInput)
    {
        if(searchInput.Length > 0)
        {
            Task.Run(async () =>
            {
                SearchBoxContext.RefreshFeed(await FetchBussinesses(searchInput));
            });
        }
        else
        {
            SearchBoxContext.RefreshFeed([]);
        }
    }

    private async Task<List<BusinessUser>> FetchBussinesses(string searchInput)
    {
        List<BusinessUser> bussinesses = [];
        foreach(char c in searchInput)
        {
            bussinesses.Add(new(c.ToString(), c.ToString(), c.ToString(), c.ToString()));
        }
        return bussinesses;
    }
}