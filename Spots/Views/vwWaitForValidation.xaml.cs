using Microsoft.Maui.ApplicationModel;
using Spots.Models.DatabaseManagement;
using Spots.Views.MainMenu;

namespace Spots.Views;

public partial class vwWaitForValidation : ContentPage
{
	public vwWaitForValidation()
	{
		InitializeComponent();

        MauiProgram.SignInValidated += SwitchToMainView; 
	}

    private void SwitchToMainView(object sender, bool signInIsValidated)
    {
        MainThread.BeginInvokeOnMainThread( () =>
        {
            if (signInIsValidated)
                Application.Current.MainPage = new vwMainShell(DatabaseManager.firebaseAuth.CurrentUser.DisplayName.Equals("Business"));
            else
                Application.Current.MainPage = new NavigationPage(new vwLogIn());
        }
        );
    }
}