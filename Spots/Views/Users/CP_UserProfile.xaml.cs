namespace Spots;

public partial class CP_UserProfile : ContentPage
{
	private Client user;
	public CP_UserProfile(Client _user)
	{
		user = _user;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();
		BindingContext = user;

		_FrameProfilePicture.HeightRequest = profilePictureDimensions;
		_FrameProfilePicture.WidthRequest = profilePictureDimensions;

		if (user.UserID != SessionManager.CurrentSession?.User?.UserID)
		{
			_btnEdit.IsVisible = false;
			if(SessionManager.CurrentSession?.Client?.FollowedClients.Contains(user.UserID) ?? false)
			{
				_btnFollow.IsVisible = false;
				_btnUnfollow.IsVisible = true;
			}
			else
			{
                _btnFollow.IsVisible = false;
                _btnUnfollow.IsVisible = true;
            }
		}
		else
		{
            _btnEdit.IsVisible = true;
            _btnFollow.IsVisible = false;
			_btnUnfollow.IsVisible = false;
		}
	}

	private void EditPersonalInformation_OnClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new CP_UpdateUserInformation(user));
	}

    private void FollowClient_OnClicked(object sender, EventArgs e)
    {
        
    }

    private void UnfollowClient_OnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CP_UpdateUserInformation(user));
    }
}