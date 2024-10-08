namespace Spots;

public partial class CP_SearchPage : ContentPage
{
    private enum ESearchFocus
    {
        CLIENT,
        SPOT
    };
    private ESearchFocus CurrentFilterApplyed = ESearchFocus.CLIENT;
	private FeedContext<object> SearchResultsListContext = new();
	public CP_SearchPage()
	{
		InitializeComponent();
		
		_colSearchBarCollectionView.BindingContext = SearchResultsListContext;
        _colSearchBarCollectionView.SelectionChanged += _colSearchBarCollectionView_SelectionChanged;
        _entrySearchTerms.TextChanged += _entrySearchTerms_TextChanged;

        _rbtnClientFilter.CheckedChanged += _rbtnClientFilter_CheckedChanged;
        _rbtnSpotFilet.CheckedChanged += _rbtnSpotFilet_CheckedChanged;
	}

    private void _rbtnClientFilter_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CurrentFilterApplyed = ESearchFocus.CLIENT;
            _entrySearchTerms_TextChanged(null, new TextChangedEventArgs("", _entrySearchTerms.Text));
        }
    }

    private void _rbtnSpotFilet_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if(e.Value)
        {
            CurrentFilterApplyed = ESearchFocus.SPOT;
            _entrySearchTerms_TextChanged(null, new TextChangedEventArgs("", _entrySearchTerms.Text));
        }
    }

    private async void _entrySearchTerms_TextChanged(object? sender, TextChangedEventArgs e)
    {
        await RefreshSearchResults(e.NewTextValue);

        if(e.NewTextValue != null && e.NewTextValue.Length > 0 && SearchResultsListContext.ItemSource.Count > 0)
        {
            _frameSearchResults.IsVisible = true;
        }
        else
        {
            _frameSearchResults.IsVisible = false;
        }
    }

    private void _colSearchBarCollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.Count > 0)
        {
            if (CurrentFilterApplyed == ESearchFocus.CLIENT)
            {
                Navigation.PushAsync(new CP_UserProfile((Client)e.CurrentSelection[0]));
            }
            else
            {
                Navigation.PushAsync(new CP_SpotView((Spot)e.CurrentSelection[0]));
            }
            _colSearchBarCollectionView.SelectedItem = null;
        }
    }

    private async Task RefreshSearchResults(string? searchInput)
    {
        try
        {
            if (searchInput?.Length > 0)
            {
                List<object> list = [];
                if(CurrentFilterApplyed == ESearchFocus.CLIENT)
                {
                    List<Client> spots = await DatabaseManager.FetchClients_ByNameAsync(searchInput, SessionManager.CurrentSession?.Client?.UserID ?? "");
                    spots.ForEach(list.Add);
                }
                else
                {
                    List<Spot> spots = await DatabaseManager.FetchSpots_ByNameAsync(searchInput, "");
                    spots.ForEach(list.Add);
                }
                
                SearchResultsListContext.RefreshFeed(list);
            }
            else
            {
                SearchResultsListContext.RefreshFeed([]);
            }
        }
        catch (Exception ex)
        {
            await UserInterface.DisplayPopUp_Regular("Unhandled Error", ex.Message, "OK");
        }
    }
}