using Spots.Models.SessionManagement;

namespace Spots.Views.MainMenu;

public partial class vwMainShell : FlyoutPage
{
	public vwMainShell(User user = null)
    {
        InitializeComponent();
        
        Flyout = new vcSideUserMenu();
        Detail = new NavigationPage(new vcFeedViews(this));

        

        if (user != null)
            CurrentSession.StartSession(user);
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}