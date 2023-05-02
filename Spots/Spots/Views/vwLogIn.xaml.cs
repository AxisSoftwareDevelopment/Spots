using Spots.Models.SessionManagement;
using Spots.Models.DatabaseManagement;

namespace Spots.Views;
public partial class vwLogIn : ContentPage
{
	public vwLogIn()
	{
		InitializeComponent();
        Resources = Application.Current.Resources;
	}

	private async void BtnLogInOnClickAsync(object sender, EventArgs e)
	{
        SwitchLockViewState();

        string email = _entryEmail.Text;
        string password = _entryPassword.Text;
        string errorMsg;

        if (CredentialsAreValid(email, password, out errorMsg))
        {
            HideErrorSection();
            // Look for user in database
            try
            {
                User user = await DatabaseManager.LogInAsync(email, password);

                if (user.userID != null && user.userID.Length > 0)
                {
                    CurrentSession.StartSession(user);
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    DisplayErrorSection("txt_LogInError_WrongCredentials");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message.ToString(), "OK");
            }
        }
        else
        {
            DisplayErrorSection(errorMsg);
        }

        SwitchLockViewState();
    }

	public void BtnRegisterOnClick(object sender, EventArgs e)
	{

	}

    private void SwitchLockViewState()
    {
        _btnLogIn.IsEnabled = !_btnLogIn.IsEnabled;
        _btnRegister.IsEnabled = !_btnRegister.IsEnabled;
    }

    private void DisplayErrorSection(string errorID)
    {
        _lblSignInError.SetDynamicResource(Label.TextProperty, errorID);
        _lblSignInError.IsVisible = true;
    }

    private void HideErrorSection()
    {
        _lblSignInError.IsVisible = false;
    }

    private bool CredentialsAreValid(string email, string password, out string errorMessage)
    {
        bool credentialsAreValid = true;
        errorMessage = "";
        if (email.Length == 0 || password.Length == 0)
        {
            credentialsAreValid = false;
            errorMessage = "txt_LogInError_EmptyEntry";
        }
        return credentialsAreValid;
    }
}