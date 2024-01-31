namespace Spots;

public partial class FP_MainShell : FlyoutPage
{
	public FP_MainShell(User user)
    {
        InitializeComponent();

        Detail = new NavigationPage(new TP_FeedViews(this));
        Flyout = new CP_SideUserMenu( Detail.Navigation, this );

        CurrentSession.StartSession(user);
    }

    public FP_MainShell(BusinessUser user)
    {
        InitializeComponent();

        Detail = new NavigationPage(new TP_FeedViews(this));
        Flyout = new CP_SideUserMenu(Detail.Navigation, this);

        CurrentSession.StartSession(user);
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}