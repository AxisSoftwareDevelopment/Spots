using Spots.Models.DatabaseManagement;
using Spots.Views;
using Spots.Views.MainMenu;

namespace Spots;

public partial class App : Application
{
	public App()
	{
        InitializeComponent();

        if (DatabaseManager.firebaseAuth.CurrentUser != null)
            MainPage = new vwMainShell();
        else
            MainPage = new NavigationPage(new vwLogIn());
    }
}
