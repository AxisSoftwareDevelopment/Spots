namespace Spots;

public partial class CP_UpdateSpotPraise : ContentPage
{
	private SpotPraise? MainSpotPraise;
    private readonly FeedContext<BusinessUser> SearchBoxContext = new();
    private ImageSource? _AttachmentSource;
    private ImageFile? _AttachmentFile;
    private bool _AttachmentChanged = false;
    public CP_UpdateSpotPraise(SpotPraise? spotPraise = null)
	{
		MainSpotPraise = spotPraise;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        _colSearchBarCollectionView.BindingContext = SearchBoxContext;
        _colSearchBarCollectionView.MaximumHeightRequest = profilePictureDimensions * 2;
        _colSearchBarCollectionView.SelectionChanged += _colSearchBarCollectionView_SelectionChanged;
        _entrySpotSearchBar.TextChanged += _entrySpotSearchBar_TextChanged;

        _FrameSpotPicture.HeightRequest = profilePictureDimensions;
        _FrameSpotPicture.WidthRequest = profilePictureDimensions;
    }

    private void _colSearchBarCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _entrySpotSearchBar.Text = "";
        LoadSelectedSpot((BusinessUser)e.CurrentSelection[0]);
    }

    private void _entrySpotSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshSearchResults(_entrySpotSearchBar.Text));
    }

    private async Task RefreshSearchResults(string searchInput)
    {
        if(searchInput.Length > 0)
        {
            SearchBoxContext.RefreshFeed(await FetchBussinesses(searchInput));
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

    public async void LoadImageOnClickAsync(object sender, EventArgs e)
    {
        ImageFile? image = await ImageManagement.PickImageFromInternalStorage();

        if (image != null)
        {
            _imgAttachmentImage.Source = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(image.Bytes ?? []));
            _AttachmentFile = image;
            _AttachmentChanged = true;
        }
    }

    private void LoadSelectedSpot(BusinessUser spotSelected)
    {
        _lblBrand.Text = spotSelected.BrandName + " - " + spotSelected.BusinessName;
        _SpotImage.Source = spotSelected.ProfilePictureSource;
        MainSpotPraise = new("", CurrentSession.currentUser.UserID, CurrentSession.currentUser.FullName, spotSelected.UserID, spotSelected.BusinessName, DateTimeOffset.Now);
    }

}