using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vwMainShell : FlyoutPage
{
	public vwMainShell(User user = null)
    {
        InitializeComponent();

        NavigationPage.SetTitleView(this, new Navigation.cvFlyoutUserNavigationBar(this));
        Flyout = new vcSideUserMenu();
        Detail = new vwLogIn();

        if (user != null)
            CurrentSession.StartSession(user);
    }
}