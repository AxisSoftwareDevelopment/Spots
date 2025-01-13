namespace Spots;

public partial class CP_SideUserMenu : ContentPage
{
    private readonly FeedContext<Table> CurrentFeedContext = new();
    public CP_SideUserMenu()
    {
        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;
        TapGestureRecognizer layoutTapRecognizer = new();
        layoutTapRecognizer.Tapped += (s, e) => {};

        InitializeComponent();
        BindingContext = SessionManager.CurrentSession?.Client;
        // We add a gesture recognizer to avoid clickthrough behaviour.
        // this might be fixed in the future and no longer necessary.
        _LayoutView.GestureRecognizers.Add( layoutTapRecognizer );
        //CurrentSession.OnSessionModeChanged += CurrentSession_OnSessionModeChanged;

        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _FrameProfilePicture.HeightRequest = profilePictureDimensions;

        // Tables Collection view
        _colTables.BindingContext = CurrentFeedContext;
        _refreshView.Command = new Command(async () =>
        {
            await RefreshFeed();
            _refreshView.IsRefreshing = false;
        });
        _colTables.RemainingItemsThreshold = 1;
        _colTables.RemainingItemsThresholdReached += OnItemThresholdReached;
        _colTables.SelectionChanged += _colTables_SelectionChanged;
        Task.Run(() =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshFeed();
            });
        });
    }

    private void AddTableOnClicked(object sender, EventArgs e)
    {
        if (FP_MainShell.MainFlyout != null)
        {
            FP_MainShell.MainFlyout.IsPresented = false;
        }
        FP_MainShell.MainNavigation?.PushAsync(new CP_UpdateTable());
    }

    private void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
    {
        if (FP_MainShell.MainFlyout != null)
        {
            FP_MainShell.MainFlyout.IsPresented = false;
        }

        if (SessionManager.CurrentSession != null)
        {
            if (SessionManager.CurrentSession.Client != null)
            {
                FP_MainShell.MainNavigation?.PushAsync(new CP_UserProfile(SessionManager.CurrentSession.Client));
            }
        }
    }

    private void PreferencesOnClicked(object sender, EventArgs e)
    {
        if (FP_MainShell.MainFlyout != null)
        {
            FP_MainShell.MainFlyout.IsPresented = false;
        }

        FP_MainShell.MainNavigation?.PushAsync(new CP_AppPreferences());
    }

    private async void LogOutOnClickedAsync(object sender, EventArgs e)
    {
        if (await UserInterface.DisplayPopPup_Choice("Log Out", "Are you sure?", "Yes", "Cancel"))
            SessionManager.CloseSession(shouldUpdateMainPage: true);
    }

    private async Task RefreshFeed()
    {
        CurrentFeedContext.RefreshFeed(await FetchTables());
    }

    private void OnItemThresholdReached(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            CurrentFeedContext.AddElements(await FetchTables(CurrentFeedContext.LastItemFetched));
        });
    }

    private static async Task<List<Table>> FetchTables(Table? lastItemFetched = null)
    {
        List<Table> retVal = [];

        if (SessionManager.CurrentSession?.Client != null)
        {
            try
            {
                retVal = await DatabaseManager.FetchTables_Filtered(tableOwnerID: SessionManager.CurrentSession.Client.UserID, lastItem: lastItemFetched);
                //return [new() { OnlineMembers= ["1", "2"], TableID = "1", TableMembers= ["1", "2", "3"], TableName="Test1" },
                //new() { OnlineMembers= [], TableID = "2", TableMembers= ["1", "2", "3"], TableName="Test2" }];
            }
            catch (Exception ex)
            {
                await UserInterface.DisplayPopUp_Regular("Unhandled Error", ex.Message, "Ok");
            }

        }

        return retVal;
    }

    private void _colTables_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            FP_MainShell.MainNavigation?.PushAsync(new CP_TableView((Table)e.CurrentSelection[0]));
            if (FP_MainShell.MainFlyout != null)
            {
                FP_MainShell.MainFlyout.IsPresented = false;
            }
            _colTables.SelectedItem = null;
        }
    }
}