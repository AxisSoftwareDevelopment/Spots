namespace Spots;

public partial class CP_FollowedClientsView : ContentPage
{
    readonly FeedContext<Client> CurrentFeedContext = new();
	public CP_FollowedClientsView(List<Client> followedClients)
	{
        CurrentFeedContext.AddElements(followedClients);

		InitializeComponent();

        _colFollowedClientsCollectionView.BindingContext = CurrentFeedContext;
        _colFollowedClientsCollectionView.SelectionChanged += _colFollowedClientsCollectionView_SelectionChanged;
    }

    private async void _colFollowedClientsCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            if (SessionManager.CurrentSession?.Client?.UserID != null)
            {
                List<FollowRegister_Firebase> followRegisters = await DatabaseManager.FetchFollowRegisters(followerID: SessionManager.CurrentSession?.Client?.UserID,
                    followedID: ((Client)e.CurrentSelection[0]).UserID);
                await Navigation.PushAsync(new CP_UserProfile((Client)e.CurrentSelection[0], followRegisters.Count > 0));
            }
            _colFollowedClientsCollectionView.SelectedItem = null;
        }
    }
}