using Android.Media;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Spots;

public partial class CP_TableView : ContentPage
{
    private const string STATE_COLOR_SITTING = "Green";
    private const string STATE_COLOR_AWAY = "Red";
    private string[] MemberStateLables = ["lbl_TableState_Sitting", "lbl_TableState_Away"];
    private string[] CurrentStateLables = ["lbl_CurrentTableState_Sitting", "lbl_CurrentTableState_Away"];
    private string[] InteractionLables = ["lbl_TableInteract_StanUp", "lbl_TableInteract_Sit"];
    private Table CachedTable;
    private readonly FeedContext<TableMember> CurrentFeedContext = new();

    public CP_TableView(Table table)
	{
		CachedTable = table;
        MemberStateLables = ResourceManagement.GetStringResources(Application.Current?.Resources, MemberStateLables);
        CurrentStateLables = ResourceManagement.GetStringResources(Application.Current?.Resources, CurrentStateLables);
        InteractionLables = ResourceManagement.GetStringResources(Application.Current?.Resources, InteractionLables);

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        BindingContext = CachedTable;

        _BorderTablePicture.HeightRequest = profilePictureDimensions;
        _BorderTablePicture.WidthRequest = profilePictureDimensions;

        Location spotLocation = new(table.Location.Latitude, table.Location.Longitude);
        _cvMiniMap.Pins.Clear();
        _cvMiniMap.MoveToRegion(new MapSpan(spotLocation, 0.01, 0.01));
        _cvMiniMap.Pins.Add(new Pin() { Label = table.Location.Address, Location = spotLocation });
        _cvMiniMap.HeightRequest = profilePictureDimensions;

        _entryAddress.IsVisible = true;

        // Members List Collection View
        _colMembers.BindingContext = CurrentFeedContext;
        _colMembers.SelectionChanged += _colMembers_SelectionChanged;
        Task.Run(() =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshMembersList();
            });
        });
        

        // Table state evaluation
        bool userIsSitting = CachedTable.SittingMembers.Contains(SessionManager.CurrentSession?.Client?.UserID ?? "NULL");
        _btnInteractWithTable.Text = userIsSitting ? InteractionLables[0] : InteractionLables[1];
        _lblCurrentState.Text = userIsSitting ? CurrentStateLables[0] : CurrentStateLables[1];
        _lblCurrentState.SetValue(BackgroundColorProperty, userIsSitting ? STATE_COLOR_SITTING : STATE_COLOR_AWAY);

        _btnInteractWithTable.Clicked += _btnInteractWithTable_Clicked;
        _btnAbandonTable.Clicked += _btnAbandonTable_Clicked;
    }

    private async void _btnAbandonTable_Clicked(object? sender, EventArgs e)
    {
        if( await UserInterface.DisplayPopPup_Choice("Are you sure?", "Abandoning the table means you will no longer have access to it, unless you receive an invitation by another member.",
            "Abandon", "Cancel"))
        {

        }
    }

    private async void _btnInteractWithTable_Clicked(object? sender, EventArgs e)
    {
        bool userIsSitting = CachedTable.SittingMembers.Contains(SessionManager.CurrentSession?.Client?.UserID ?? "NULL");
        if(userIsSitting)
        {
            CachedTable.SittingMembers.Remove(SessionManager.CurrentSession?.Client?.UserID);
        }
        else
        {
            CachedTable.SittingMembers.Add(SessionManager.CurrentSession?.Client?.UserID);
        }
        ChangeSittingStateUI(!userIsSitting);

        await RefreshMembersList();
    }

    private async Task RefreshMembersList()
    {
        CurrentFeedContext.RefreshFeed(await FetchMembers());
    }

    private async Task<List<TableMember>> FetchMembers()
    {
        List<TableMember> retVal = [];

        if (SessionManager.CurrentSession?.Client != null)
        {
            try
            {
                List<Client> clients = await DatabaseManager.FetchClientsByID(CachedTable.TableMembers);
                foreach (Client client in clients)
                {
                    retVal.Add(new TableMember(client, CachedTable.SittingMembers.Contains(client.UserID), MemberStateLables));
                }
            }
            catch (Exception ex)
            {
                await UserInterface.DisplayPopUp_Regular("Unhandled Error", ex.Message, "Ok");
            }

        }

        return retVal;
    }

    private void _colMembers_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count > 0)
        {
            ((TableMember)e.CurrentSelection[0]).LaunchClientView();
            _colMembers.SelectedItem = null;
        }
    }

    private void ChangeSittingStateUI(bool bIsSitting)
    {
        _btnInteractWithTable.Text = bIsSitting ? InteractionLables[0] : InteractionLables[1];
        _lblCurrentState.Text = bIsSitting ? CurrentStateLables[0] : CurrentStateLables[1];
        _lblCurrentState.SetValue(BackgroundColorProperty, bIsSitting ? STATE_COLOR_SITTING : STATE_COLOR_AWAY);
    }

    private class TableMember
    {
        private const string STATE_COLOR_SITTING = "Green";
        private const string STATE_COLOR_AWAY = "Red";
        private Client CachedClient;
        public string Name { get; set; }
        public string State { get; set; }
        public string StateColor { get; set; }
        public ImageSource ProfilePictureSource { get; set; }

        public TableMember(Client client, bool sitting, string[] stateLbls)
        {
            CachedClient = client;
            Name = client.FullName;
            State = sitting ? stateLbls[0] : stateLbls[1];
            StateColor = sitting ? STATE_COLOR_SITTING : STATE_COLOR_AWAY;
            ProfilePictureSource = client.ProfilePictureSource;
        }

        public void LaunchClientView()
        {
            FP_MainShell.MainNavigation?.PushAsync(new CP_UserProfile(CachedClient));
        }
    }
}