using eatMeet.Models;

namespace eatMeet;

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
            await ((Client)e.CurrentSelection[0]).OpenClientView(FP_MainShell.MainNavigation);
            _colFollowedClientsCollectionView.SelectedItem = null;
        }
    }
}