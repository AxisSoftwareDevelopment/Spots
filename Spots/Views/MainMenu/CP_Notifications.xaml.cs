using eatMeet.Database;
using eatMeet.Models;
using eatMeet.ResourceManager;
using eatMeet.Utilities;

namespace eatMeet;

public partial class CP_Notifications : ContentPage
{
    private readonly FeedContext<INotification> CurrentFeedContext = new();
    public CP_Notifications()
	{
		InitializeComponent();

        // Tables Collection view
        _colNotifications.BindingContext = CurrentFeedContext;
        _colNotifications.RemainingItemsThreshold = 1;
        _colNotifications.RemainingItemsThresholdReached += OnItemThresholdReached;
        _colNotifications.SelectionChanged += _colNotifications_SelectionChanged;
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
        CurrentFeedContext.RefreshFeed(await FetchNotifications());
    }

    private void OnItemThresholdReached(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            CurrentFeedContext.AddElements(await FetchNotifications(CurrentFeedContext.LastItemFetched));
        });
    }

    private async void _colNotifications_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            if (await ((INotification)e.CurrentSelection[0]).OnClickedEvent())
            {
                // Refresh Notification
                await RefreshFeed();
            }
            _colNotifications.SelectedItem = null;
        }
    }

    private static async Task<List<INotification>> FetchNotifications(INotification? lastItemFetched = null)
    {
        List<INotification> retVal = [];

        if (SessionManager.CurrentSession?.Client != null)
        {
            try
            {
                retVal = await DatabaseManager.FetchNotifications_Filtered(ownerID: SessionManager.CurrentSession.Client.UserID, lastNotification: lastItemFetched);
            }
            catch (Exception ex)
            {
                await UserInterface.DisplayPopUp_Regular("Unhandled Error", ex.Message, "Ok");
            }

        }

        return retVal;
    }
}