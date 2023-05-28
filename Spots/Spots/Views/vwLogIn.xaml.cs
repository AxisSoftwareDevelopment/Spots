using Spots.Models.SessionManagement;
using Spots.Models.DatabaseManagement;
using Plugin.Firebase.Core.Exceptions;
using Spots.Views.MainMenu;
using Spots.Views.Users;
using Spots.Models.ResourceManagement;

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

        if (CredentialsAreValid(email, password, out string errorMsg))
        {
            HideErrorSection();
            // Look for _user in database
            try
            {
                User user = await DatabaseManager.LogInWithEmailAndPasswordAsync(email, password);

                if (user.userDataRetrieved)
                {
                    // Check if the _user has updated its basic information yet
                    await Application.Current.MainPage.DisplayAlert("Welcome!", "Welcome to spots!", "OK");
                    Application.Current.MainPage = new vwMainShell( user );
                }
                else
                {
                    // Setup User Data
                    string[] strings = ResourceManagement.GetStringResources(Resources, new string[3] { "lbl_FirstTime", "txt_FirstTime", "lbl_Ok" });
                    await Application.Current.MainPage.DisplayAlert(strings[0], strings[1], strings[2]);
                    await Navigation.PushAsync(new vwUpdateUserInformation(user, email, password));
                }
            }
            catch (FirebaseAuthException ex)
            {
                #region Error Message Calculation
                string errorID;
                switch (ex.Reason)
                {
                    case FIRAuthError.InvalidEmail:
                        errorID = "txt_LogInError_WrongCredentials";
                        break;
                    case FIRAuthError.WrongPassword:
                        errorID = "txt_LogInError_WrongCredentials";
                        break;
                    case FIRAuthError.InvalidCredential:
                        errorID = "txt_LogInError_InvalidCredential";
                        break;
                    case FIRAuthError.UserNotFound:
                        errorID = "txt_LogInError_WrongCredentials";
                        break;
                    default:
                        errorID = "txt_LogInError_Undefined";
                        break;
                }
                #endregion

                DisplayErrorSection(errorID);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Unhandled Error", ex.Message, "OK");
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
        Navigation.PushAsync(new vwRegister());
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