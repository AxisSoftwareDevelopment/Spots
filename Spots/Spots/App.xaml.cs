using Spots.Models.DatabaseManagement;
using Spots.Models.SessionManagement;
using Spots.Views;

namespace Spots;

public partial class App : Application
{
	public App()
	{
        InitializeComponent();

        if (DatabaseManager.firebaseAuth.CurrentUser != null)
            MainPage = new NavigationPage(new AppShell());
        else
            MainPage = new NavigationPage(new vwLogIn());
    }
}
