using Spots.Models;
using Spots.Database;

namespace Spots.FollowedClients;

public partial class CV_FollowedClientDataTemplate : ContentView
{
	public CV_FollowedClientDataTemplate()
	{
		InitializeComponent();

        _btnFollow.Clicked += _btnFollow_Clicked;
        _btnUnfollow.Clicked += _btnUnfollow_Clicked;
	}

    private async void _btnUnfollow_Clicked(object? sender, EventArgs e)
    {
        string followerID = SessionManager.CurrentSession?.Client?.UserID ?? "";
        string followedID = ((Client)BindingContext).UserID;
        if (followerID.Length > 0)
        {
            if (await DatabaseManager.Transaction_UpdateClientFollowedList(followerID, followedID, false))
            {
                _btnFollow.IsVisible = true;
                _btnUnfollow.IsVisible = false;
                SessionManager.CurrentSession?.Client?.Followed.Remove(followedID);
            }
        }
    }

    private async void _btnFollow_Clicked(object? sender, EventArgs e)
    {
        string followerID = SessionManager.CurrentSession?.Client?.UserID ?? "";
        string followedID = ((Client)BindingContext).UserID;
        if (followerID.Length > 0)
        {
            if (await DatabaseManager.Transaction_UpdateClientFollowedList(followerID, followedID, true))
            {
                _btnFollow.IsVisible = false;
                _btnUnfollow.IsVisible = true;
                SessionManager.CurrentSession?.Client?.Followed.Add(followedID);
            }
        }
    }
}