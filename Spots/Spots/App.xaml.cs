using Spots.Models.DatabaseManagement;
using Spots.Views;
using Spots.Views.MainMenu;

namespace Spots;

public partial class App : Application
{
	public App()
	{
        InitializeComponent();

        MainPage = new vwWaitForValidation();
    }
}
