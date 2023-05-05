using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vwMainShell : FlyoutPage
{
	public vwMainShell(User user = null)
    {
        InitializeComponent();

        Flyout = new vcSideUserMenu();
        Detail = new vcFeedViews(this);
        NavigationPage.SetHasNavigationBar(this, false);

        if (user != null)
            CurrentSession.StartSession(user);
    }
}