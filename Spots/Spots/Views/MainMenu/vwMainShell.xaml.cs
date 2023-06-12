using Spots.Models.SessionManagement;
using Spots.Views.Users;

namespace Spots.Views.MainMenu;

public partial class vwMainShell : FlyoutPage
{
	public vwMainShell(User user)
    {
        InitializeComponent();

        Detail = new NavigationPage(new vcFeedViews(this));
        Flyout = new vcSideUserMenu( Detail.Navigation, this );

        CurrentSession.StartSession(user);
    }

    public vwMainShell(BusinessUser user)
    {
        InitializeComponent();

        Detail = new NavigationPage(new vcFeedViews(this));
        Flyout = new vcSideUserMenu(Detail.Navigation, this);

        CurrentSession.StartSession(user);
    }

    public vwMainShell(bool userIsBusiness)
    {
        InitializeComponent();

        if(userIsBusiness)
        {
            Detail = new NavigationPage(new vcFeedViews(this));
            Flyout = new vcSideUserMenu(Detail.Navigation, this);
        }
        else
        {
            Detail = new NavigationPage(new vcFeedViews(this));
            Flyout = new vcSideUserMenu(Detail.Navigation, this);
        }
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}