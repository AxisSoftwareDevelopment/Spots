using Spots.Models;
using Spots.ResourceManager;
using Spots.Utilities;
using Spots.Database;
using Android.Media.TV;

namespace Spots;

public partial class CP_InviteUserToTable : ContentPage
{
    private readonly FeedContext<Table> CurrentFeedContext = new();
    private Client CachedClient;
    private string[] ConfirmationLables = ["lbl_InviteToTable", "txt_TableInvitationSendConfirmation", "lbl_Ok", "lbl_Cancel"];
	public CP_InviteUserToTable(Client client)
	{
		CachedClient = client;
        ConfirmationLables = ResourceManagement.GetStringResources(Application.Current?.Resources, ConfirmationLables);

        InitializeComponent();

        BindingContext = CachedClient;

        // Tables Collection view
        _colTables.BindingContext = CurrentFeedContext;
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

    private async void _colTables_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            string tableID = ((Table)e.CurrentSelection[0]).TableID;
            string tableName = ((Table)e.CurrentSelection[0]).TableName;
            string formatedConfirmationMessage = string.Format(ConfirmationLables[1], CachedClient.FullName, tableName);
            if (await UserInterface.DisplayPopPup_Choice(ConfirmationLables[0], formatedConfirmationMessage, ConfirmationLables[2], ConfirmationLables[3]))
            {
                // Send Invitation
                Notification_TableInvite tableInvite = new("", DateTimeOffset.Now, Notification.NOTIFICATION_TYPE_TABLEINVITE, CachedClient.UserID, tableID, CachedClient.FullName, tableName, SessionManager.CurrentSession?.Client?.FullName?? "");
                await DatabaseManager.SaveNotificationData(tableInvite);
                await Navigation.PopAsync();
            }
            _colTables.SelectedItem = null;
        }
    }

    private static async Task<List<Table>> FetchTables(Table? lastItemFetched = null)
    {
        List<Table> retVal = [];

        if (SessionManager.CurrentSession?.Client != null)
        {
            try
            {
                retVal = await DatabaseManager.FetchTables_Filtered(tableMemberID: SessionManager.CurrentSession.Client.UserID, lastItem: lastItemFetched);
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
}