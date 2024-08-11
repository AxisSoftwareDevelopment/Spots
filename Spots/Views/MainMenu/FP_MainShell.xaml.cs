namespace Spots;

public partial class FP_MainShell : FlyoutPage
{
	public FP_MainShell(Client user)
    {
        SessionManager.StartSession(user);

        InitializeComponent();

        Detail = new NavigationPage(new TP_FeedViews(this));
        Flyout = new CP_SideUserMenu( Detail.Navigation, this );
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}