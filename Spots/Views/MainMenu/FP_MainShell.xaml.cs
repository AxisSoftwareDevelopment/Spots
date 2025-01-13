namespace Spots;

public partial class FP_MainShell : FlyoutPage
{
    public static INavigation? MainNavigation;
    public static FlyoutPage? MainFlyout;
	public FP_MainShell(Client user)
    {
        SessionManager.StartSession(user);

        InitializeComponent();

        Detail = new NavigationPage(new TP_FeedViews(this));
        Flyout = new CP_SideUserMenu();

        MainNavigation = Detail.Navigation;
        MainFlyout = this;
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}