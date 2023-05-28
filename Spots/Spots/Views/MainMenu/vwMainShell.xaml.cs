using Spots.Models.SessionManagement;
using Spots.Views.Users;

namespace Spots.Views.MainMenu;

public partial class vwMainShell : FlyoutPage
{
	public vwMainShell(User user = null)
    {
        InitializeComponent();

        Detail = new NavigationPage(new vcFeedViews(this));
        Flyout = new vcSideUserMenu( Detail.Navigation, this );

        if (user != null)
            CurrentSession.StartSession(user);
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}