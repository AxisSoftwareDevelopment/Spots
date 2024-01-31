using Plugin.Firebase.Core.Exceptions;

namespace Spots;

public partial class CP_Login : ContentPage
{
	private bool _BusinessMode = false;
	public CP_Login()
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

            if (_BusinessMode)
                await LookForBusinessDataInDBAsync(email, password);
            else
                await LookForUserDataInDBAsync(email, password);
        }
        else
        {
            DisplayErrorSection(errorMsg);
        }

        SwitchLockViewState();
    }

    private async Task LookForBusinessDataInDBAsync(string email, string password)
    {
        try
        {
            BusinessUser business = await DatabaseManager.LogInBusinessAsync(email, password);

            if (business.userDataRetrieved)
            {
                // Check if the _user has updated its basic information yet
                string[] stringResources = ResourceManagement.GetStringResources(Application.Current.Resources, new string[] { "lbl_Welcome", "lbl_WelcomeToSpots", "lbl_Ok" });
                await Application.Current.MainPage.DisplayAlert(stringResources[0], stringResources[1], stringResources[2]);
                Application.Current.MainPage = new FP_MainShell(business);
            }
            else
            {
                // Setup User Data
                string[] strings = ResourceManagement.GetStringResources(Resources, new string[3] { "lbl_FirstTime", "txt_FirstTime", "lbl_Ok" });
                await Application.Current.MainPage.DisplayAlert(strings[0], strings[1], strings[2]);
                await Navigation.PushAsync(new CP_UpdateBusinessInformation(business, email, password, business.PhoneNumber, business.PhoneCountryCode));
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
                case FIRAuthError.EmailAlreadyInUse:
                    errorID = ex.Message.Split("->")[0].Trim();
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

    private async Task LookForUserDataInDBAsync(string email, string password)
    {
        try
        {
            User user = await DatabaseManager.LogInUserAsync(email, password);

            if (user.bUserDataRetrieved)
            {
                // Check if the _user has updated its basic information yet
                await Application.Current.MainPage.DisplayAlert("Welcome!", "Welcome to spots!", "OK");
                Application.Current.MainPage = new FP_MainShell(user);
            }
            else
            {
                // Setup User Data
                string[] strings = ResourceManagement.GetStringResources(Resources, new string[3] { "lbl_FirstTime", "txt_FirstTime", "lbl_Ok" });
                await Application.Current.MainPage.DisplayAlert(strings[0], strings[1], strings[2]);
                await Navigation.PushAsync(new CP_UpdateUserInformation(user, email, password));
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
                case FIRAuthError.EmailAlreadyInUse:
                    errorID = ex.Message.Split("->")[0].Trim();
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

    public void BtnRegisterOnClick(object sender, EventArgs e)
	{
        if(_BusinessMode)
            Navigation.PushAsync(new CP_RegisterBusiness());
        else
            Navigation.PushAsync(new CP_Register());
    }

    public void BtnBusinessModeOnClick(object sender, EventArgs e)
    {
        _BusinessMode = !_BusinessMode;
        HideErrorSection();
        LoadModeSpecifics();
    }

    private void LoadModeSpecifics()
    {
        if(_BusinessMode)
        {
            _btnChangeMode.SetDynamicResource(Button.TextProperty, "lbl_UserMode");
            _lblTitleLogIn.SetDynamicResource(Label.TextProperty, "lbl_BusinessLogIn");
        }
        else
        {
            _btnChangeMode.SetDynamicResource(Button.TextProperty, "lbl_BusinessMode");
            _lblTitleLogIn.SetDynamicResource(Label.TextProperty, "lbl_LogIn");
        }
    }

    private void SwitchLockViewState()
    {
        _btnLogIn.IsEnabled = !_btnLogIn.IsEnabled;
        _btnRegister.IsEnabled = !_btnRegister.IsEnabled;
        _btnChangeMode.IsEnabled = !_btnChangeMode.IsEnabled;
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