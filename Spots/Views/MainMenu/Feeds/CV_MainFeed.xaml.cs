using System.ComponentModel;

namespace Spots;

public partial class CV_MainFeed : ContentView, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;
	private FeedContext<SpotPraise> CurrentFeedContext = new();
	private uint count = 0;

	public CV_MainFeed()
	{
		InitializeComponent();

		_colFeed.BindingContext = CurrentFeedContext;
		_refreshView.Command = new Command(() =>
		{
			RefreshFeed();
			_refreshView.IsRefreshing = false;
		});
		_colFeed.RemainingItemsThreshold = 1;
		_colFeed.RemainingItemsThresholdReached += OnItemThresholdReached;
		RefreshFeed();
	}

	private void RefreshFeed()
	{
		count = 0;
		CurrentFeedContext.RefreshFeed(FetchPraises(0,0));
    }

	private void OnItemThresholdReached(object? sender, EventArgs e)
	{
        CurrentFeedContext.AddElements(FetchPraises(0, 0));
    }

	private List<SpotPraise> FetchPraises(uint startIndex, uint endIndex, List<uint>? alreadyFetchedPrisesIDs = null)
	{
		return [new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
			new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers! Here is a picture:", pictureAddress: "placeholder_logo.jpg"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!"),
            new SpotPraise("Test", "Gaston TV", "Gaston TV", "MC Donalds", count++.ToString() + "MC Donalds", new DateTimeOffset(), comment: "Very Good Burgers!")];
	}
}