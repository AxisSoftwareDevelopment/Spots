using System.ComponentModel;

namespace Spots;

public partial class CV_MainFeed : ContentView, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;
	private readonly FeedContext<SpotPraise> CurrentFeedContext = new();
	private uint count = 0;

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
        MainThread.BeginInvokeOnMainThread(async () => await RefreshFeed());
	}

	private async Task RefreshFeed()
	{
		count = 0;
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
		return [new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
			new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers! Here is a picture:", pictureAddress: "placeholder_logo.jpg"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!")];

		//return await DatabaseManager.FetchSpotPraises(lastItemFetched);
	}
}