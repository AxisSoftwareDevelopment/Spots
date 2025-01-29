using Spots.Models;
using Spots.Database;

namespace Spots;

public partial class CV_MainFeed : ContentView
{
	private readonly FeedContext<SpotPraise> CurrentFeedContext = new();

	public CV_MainFeed()
	{
		InitializeComponent();

		_colFeed.BindingContext = CurrentFeedContext;
		_refreshView.Command = new Command(async () =>
		{
			await RefreshFeed();
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
            Navigation.PushAsync(new CP_SpotPraise((SpotPraise)e.CurrentSelection[0]));
			_colFeed.SelectedItem = null;
        }
    }

    private void _btnWritePraise_Clicked(object? sender, EventArgs e)
    {
		Navigation.PushAsync(new CP_UpdateSpotPraise());
    }

    private async Task RefreshFeed()
	{
		CurrentFeedContext.RefreshFeed(await FetchPraises());
    }

	private void OnItemThresholdReached(object? sender, EventArgs e)
	{
		Task.Run(async () =>
		{
			CurrentFeedContext.AddElements(await FetchPraises(CurrentFeedContext.LastItemFetched));
		});
    }

	private async Task<List<SpotPraise>> FetchPraises(SpotPraise? lastItemFetched = null)
	{
		List<SpotPraise> retVal = [];

		if(SessionManager.CurrentSession?.Client != null)
		{
			retVal = await DatabaseManager.FetchSpotPraises_FromFollowedClients(SessionManager.CurrentSession.Client, lastItemFetched);
        }

		return retVal;
	}

    private async void LikeButtonClicked(object sender, EventArgs e)
    {
        if (SessionManager.CurrentSession?.Client != null)
        {
            bool? likedState = await ((SpotPraise)((Button)sender).BindingContext).LikeSwitch(SessionManager.CurrentSession.Client.UserID);

            if (likedState != null)
            {
                if ((bool)likedState)
                {
                    ((SpotPraise)((Button)sender).BindingContext).Likes.Add(SessionManager.CurrentSession.Client.UserID);
                    ((SpotPraise)((Button)sender).BindingContext).LikesCount++;
                }
                else
                {
                    ((SpotPraise)((Button)sender).BindingContext).Likes.Remove(SessionManager.CurrentSession.Client.UserID);
                    ((SpotPraise)((Button)sender).BindingContext).LikesCount--;
                }
            }
        }
    }
}