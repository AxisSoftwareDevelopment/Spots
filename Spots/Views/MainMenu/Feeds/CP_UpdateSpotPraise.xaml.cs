using Microsoft.Extensions.Options;

namespace Spots;

public partial class CP_UpdateSpotPraise : ContentPage
{
	private SpotPraise? MainSpotPraise;
    private readonly FeedContext<Spot> SearchBoxContext = new();
    private ImageFile? _AttachmentFile;
    private List<string>? _SpotPraisers = null;
    public CP_UpdateSpotPraise(SpotPraise? spotPraise = null)
	{
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        LoadSpotPraise(spotPraise);

        _colSearchBarCollectionView.BindingContext = SearchBoxContext;
        _colSearchBarCollectionView.MaximumHeightRequest = profilePictureDimensions * 2;
        _colSearchBarCollectionView.SelectionChanged += _colSearchBarCollectionView_SelectionChanged;
        _entrySpotSearchBar.TextChanged += _entrySpotSearchBar_TextChanged;

        _FrameSpotPicture.HeightRequest = profilePictureDimensions;
        _FrameSpotPicture.WidthRequest = profilePictureDimensions;

        _btnSave.Clicked += _btnSave_Clicked;
    }

    public CP_UpdateSpotPraise(Spot spot)
    {
        MainSpotPraise = null;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        _colSearchBarCollectionView.BindingContext = SearchBoxContext;
        _colSearchBarCollectionView.MaximumHeightRequest = profilePictureDimensions * 2;
        _colSearchBarCollectionView.SelectionChanged += _colSearchBarCollectionView_SelectionChanged;
        _entrySpotSearchBar.TextChanged += _entrySpotSearchBar_TextChanged;

        _FrameSpotPicture.HeightRequest = profilePictureDimensions;
        _FrameSpotPicture.WidthRequest = profilePictureDimensions;

        _btnSave.Clicked += _btnSave_Clicked;

        LoadSelectedSpot(spot);

        _entrySpotSearchBar.IsVisible = false;
        _colSearchBarCollectionView.IsVisible = false;
    }
    private void LoadSpotPraise(SpotPraise? praise)
    {
        MainSpotPraise = praise;

        if(MainSpotPraise != null)
        {
            _lblBrand.Text = MainSpotPraise.SpotFullName;
            _editorDescription.Text = MainSpotPraise.Comment;
            _imgAttachmentImage.Source = MainSpotPraise.AttachedPicture;
            _SpotImage.Source = MainSpotPraise.SpotProfilePicture;
        }
    }

    private void _colSearchBarCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _entrySpotSearchBar.Text = "";
        LoadSelectedSpot((Spot)e.CurrentSelection[0]);
    }

    private async void _entrySpotSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        await RefreshSearchResults(_entrySpotSearchBar.Text);
        if(_entrySpotSearchBar.Text.Length > 0 && SearchBoxContext.ItemSource.Count > 0)
        {
            _colSearchBarCollectionView.IsVisible = true;
        }
        else
        {
            _colSearchBarCollectionView.IsVisible = false;
        }
    }

    private async Task RefreshSearchResults(string searchInput)
    {
        try
        {
            if (searchInput.Length > 0)
            {
                List<Spot> spotList = await DatabaseManager.FetchSpots_ByNameAsync(searchInput, SessionManager.CurrentSession?.User?.UserID ?? "");
                SearchBoxContext.RefreshFeed(spotList);
            }
            else
            {
                SearchBoxContext.RefreshFeed([]);
            }
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Error", ex.Message, "OK");
        }
    }

    public async void LoadImageOnClickAsync(object sender, EventArgs e)
    {
        ImageFile? image = await ImageManagement.PickImageFromInternalStorage();

        if (image != null)
        {
            _imgAttachmentImage.Source = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(image.Bytes ?? []));
            _AttachmentFile = image;
        }
    }

    private void LoadSelectedSpot(Spot spotSelected)
    {
        _lblBrand.Text = spotSelected.FullName;
        _SpotImage.Source = spotSelected.ProfilePictureSource;
        MainSpotPraise = new("", SessionManager.CurrentSession?.User?.UserID ?? "", SessionManager.CurrentSession?.User?.FullName ?? "", spotSelected.UserID, spotSelected.SpotName, DateTimeOffset.Now);
        _SpotPraisers = spotSelected.Praisers;
    }

    private async void _btnSave_Clicked(object? sender, EventArgs e)
    {
        LockInputs();
        if(MainSpotPraise != null)
        {
            MainSpotPraise.Comment = _editorDescription.Text.Trim();

            bool addToSpotPraises = !_SpotPraisers?.Contains(MainSpotPraise.AuthorID) ?? true;

            if(await DatabaseManager.SaveSpotPraiseData(MainSpotPraise, addToSpotPraises, _AttachmentFile))
            {
                await Navigation.PopAsync();
            }
        }
        UnlockInputs();
    }

    private void LockInputs()
    {
        _btnLoadImage.IsEnabled = false;
        _btnSave.IsEnabled = false;
        _editorDescription.IsEnabled = false;
        _entrySpotSearchBar.IsEnabled = false;
    }

    private void UnlockInputs()
    {
        _btnLoadImage.IsEnabled = true;
        _btnSave.IsEnabled = true;
        _editorDescription.IsEnabled = true;
        _entrySpotSearchBar.IsEnabled = true;
    }
}