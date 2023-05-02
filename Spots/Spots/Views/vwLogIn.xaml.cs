using Spots.Models.SessionManagement;
using Spots.Models.DatabaseManagement;
using Plugin.Firebase.Core;
using Plugin.Firebase.Core.Exceptions;

namespace Spots.Views;
public partial class vwLogIn : ContentPage
{
    private const int RESOURCE_INDEX_CURRENTLANGUAGE = 1;
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
                User user = await DatabaseManager.LogInWithEmailAndPasswordAsync(email, password);

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
            catch (FirebaseAuthException ex)
            {
                string errorID;
                switch (ex.Reason)
                {
                    case FIRAuthError.InvalidEmail:
                        errorID = "txt_Error_InvalidEmail";
                        break;
                    case FIRAuthError.WrongPassword:
                        errorID = "txt_Error_WrongPassword";
                        break;
                    case FIRAuthError.InvalidCredential:
                        errorID = "txt_Error_InvalidCredential";
                        break;
                    case FIRAuthError.UserNotFound:
                        errorID = "txt_Error_UserNotFound";
                        break;
                    default:
                        errorID = "txt_Error_Undefined";
                        break;
                }
                object message = "";
                Resources.MergedDictionaries.ElementAt(RESOURCE_INDEX_CURRENTLANGUAGE).TryGetValue(errorID, out message);

                await Application.Current.MainPage.DisplayAlert("Error", (string)message, "OK");
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