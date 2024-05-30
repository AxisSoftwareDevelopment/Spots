namespace Spots;

public partial class CP_FollowedClientsView : ContentPage
{
	FeedContext<Client> CurrentFeedContext = new();
	public CP_FollowedClientsView(List<Client> followedClients)
	{
        CurrentFeedContext.AddElements(followedClients);

		InitializeComponent();

        _colFollowedClientsCollectionView.BindingContext = CurrentFeedContext;
        _colFollowedClientsCollectionView.SelectionChanged += _colFollowedClientsCollectionView_SelectionChanged;
    }

    private void _colFollowedClientsCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            Navigation.PushAsync(new CP_UserProfile((Client)e.CurrentSelection[0]));
            _colFollowedClientsCollectionView.SelectedItem = null;
        }
    }
}