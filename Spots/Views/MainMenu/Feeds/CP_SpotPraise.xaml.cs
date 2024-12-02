namespace Spots;

public partial class CP_SpotPraise : ContentPage
{
    private readonly SpotPraise _SpotPraise;
	public CP_SpotPraise(SpotPraise praise)
	{
        _SpotPraise = praise;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        _FrameSpotPicture.HeightRequest = profilePictureDimensions;
        _FrameSpotPicture.WidthRequest = profilePictureDimensions;
		
        _FrameAuthorPicture.HeightRequest = profilePictureDimensions / 4;
        _FrameAuthorPicture.WidthRequest = profilePictureDimensions / 4;

        _imgAttachmentImage.MaximumHeightRequest = profilePictureDimensions * 2;

        _btnEdit.Clicked += _btnEdit_Clicked;
        _imgAuthorPicture.Clicked += _imgAuthorPicture_Clicked;
        _imgSpotImage.Clicked += _imgSpotImage_Clicked;

        LoadSpotPraiseInformation();

        if (praise.AuthorID == SessionManager.CurrentSession?.Client?.UserID)
        {
            _btnEdit.IsVisible = true;
        }
        else
        {
            _btnEdit.IsVisible = false;
        }

    }

    private async void _imgSpotImage_Clicked(object? sender, EventArgs e)
    {
        LockView();
        Spot spot = await DatabaseManager.GetSpotDataAsync(_SpotPraise.SpotID);
        await Navigation.PushAsync(new CP_SpotView(spot));
        UnlockView();
    }

    private async void _imgAuthorPicture_Clicked(object? sender, EventArgs e)
    {
        LockView();
        Client client = await DatabaseManager.GetClientDataAsync(_SpotPraise.AuthorID);
        if (SessionManager.CurrentSession?.Client?.UserID != null)
        {
            List<FollowRegister_Firebase> followRegisters = await DatabaseManager.FetchFollowRegisters(followerID: SessionManager.CurrentSession?.Client?.UserID, followedID: client.UserID);
            await Navigation.PushAsync(new CP_UserProfile(client, followRegisters.Count > 0));
        }
        UnlockView();
    }

    private void _btnEdit_Clicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateSpotPraise(_SpotPraise));
    }

    private void LoadSpotPraiseInformation()
	{
		_lblAuthorName.Text = _SpotPraise.AuthorFullName;
		_lblBrand.Text = _SpotPraise.SpotFullName;
		_lblComment.Text = _SpotPraise.Comment;

        _imgAuthorPicture.Source = _SpotPraise.AuthorProfilePicture;
        _imgSpotImage.Source = _SpotPraise.SpotProfilePicture;
        _imgAttachmentImage.Source = _SpotPraise.AttachedPicture;

		_btnEdit.IsVisible = _SpotPraise.AuthorID.Equals(SessionManager.CurrentSession?.Client?.UserID);
	}

    private void LockView()
    {
        _btnEdit.IsEnabled = false;
        _imgAuthorPicture.IsEnabled = false;
        _imgSpotImage.IsEnabled = false;
    }

    private void UnlockView()
    {
        _btnEdit.IsEnabled = true;
        _imgAuthorPicture.IsEnabled = true;
        _imgSpotImage.IsEnabled = true;
    }
}