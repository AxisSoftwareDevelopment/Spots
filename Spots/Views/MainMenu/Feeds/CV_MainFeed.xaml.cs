using System.ComponentModel;

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
		if(SessionManager.CurrentSession?.Client != null)
		{
            return await DatabaseManager.FetchSpotPraises_FromFollowedClients(SessionManager.CurrentSession.Client, lastItemFetched);
        }
		else
		{
			return [];
		}
	}
}