using Spots.Models.DatabaseManagement;
using Spots.Models.SessionManagement;
using Spots.Views.MainMenu;
using System.Globalization;

namespace Spots.Views.Users;

public partial class vwUpdateBusinessInformation : ContentPage
{
    private User _user;
    private bool _userIsEmpty;
    private string _password, _email;
    private bool _birhtdateSelected = false;

    public vwUpdateBusinessInformation(User user, string email = null, string password = null)
    {
        _user = user;
        _userIsEmpty = password != null && email != null && !user.userDataRetrieved;
        _password = password;
        _email = email;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;

        if (_userIsEmpty)
            NavigationPage.SetHasBackButton(this, false);

        InitializeControllers();
    }

    protected override bool OnBackButtonPressed()
    {
        if (_userIsEmpty)
            return true;
        else
            return base.OnBackButtonPressed();
    }

    private void SkipOnClick(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void SaveOnCLickAsync(object sender, EventArgs e)
    {
        // We update the _user data
        string brandName = ToTitleCase(_entryBrandName.Text.Trim());
        string BusinessName = ToTitleCase(_entryBusinessName.Text.Trim());
        string description = _editorDescription.Text.Trim();
        string phoneNumber = _entryPhoneNumber.Text;
        string phoneCountryCode = _entryPhoneCountryCode.Text;

        bool thereAreEmptyFields = brandName.Length == 0 ||
                            BusinessName.Length == 0 ||
                            (!_birhtdateSelected && _userIsEmpty);
        bool descriptionUnder150Chars = description.Length <= 150;
        bool validPhoneNumber = (phoneNumber.Length == 10 && phoneCountryCode.Length == 2) || (phoneNumber.Length == 0 && phoneCountryCode.Length == 0);

        if (!thereAreEmptyFields && descriptionUnder150Chars && validPhoneNumber)
        {
            HideErrorSection();

            if (_userIsEmpty)
                _user.email = _email;
            _user.firstName = brandName;
            _user.lastName = BusinessName;
            //_user.profilePictureAddress
            _user.phoneNumber = phoneNumber;
            _user.phoneCountryCode = phoneCountryCode;
            _user.description = description;

            if (await DatabaseManager.SaveUserDataAsync(_user))
            {
                _user.userDataRetrieved = true;
                await Application.Current.MainPage.DisplayAlert("Success", "Your information has been updated. Way to go!", "OK");
                // If the user was empty, it meas we came from the log in.
                if (_userIsEmpty)
                {
                    // We then have to log in and go to main page.
                    await DatabaseManager.LogInWithEmailAndPasswordAsync(_email, _password, getUser: false);
                    Application.Current.MainPage = new vwMainShell(_user);
                }
                else
                {
                    // If the user was just updating information, then we just pop the page from navigation
                    CurrentSession.currentUser.UpdateUserData(_user);
                    await Navigation.PopAsync();
                }
            }
        }
        else
        {
            string errorMessageID = "txt_Error_UnkownError";

            #region Error message calculation
            if (thereAreEmptyFields)
            {
                errorMessageID = "txt_RegisterError_EmptyFields";
            }
            else if (!birthdateIsValid)
            {
                errorMessageID = "txt_UserInfoError_InvalidBirthdate";
            }
            else if (!descriptionUnder150Chars)
            {
                errorMessageID = "txt_UserInfoError_DescriptionTooLong";
            }
            else if (!validPhoneNumber)
            {
                errorMessageID = "txt_UserInfoError_InvalidPhoneNumber";
            }
            #endregion

            DisplayErrorSection(errorMessageID);
        }
    }

    #region Utilities
    private void InitializeControllers()
    {
        // Load _user data
        _entryBrandName.Text = _user.firstName;
        _entryBusinessName.Text = _user.lastName;
        _entryPhoneNumber.Text = _user.phoneNumber;
        _entryPhoneCountryCode.Text = _user.phoneCountryCode;
        _editorDescription.Text = _user.description;
        // Initialize BirthDate field
        if (_userIsEmpty)
        {
            
        }
        else
        {
            
        }
    }

    private void DisplayErrorSection(string errorID)
    {
        _lblRegisterError.SetDynamicResource(Label.TextProperty, errorID);
        _lblRegisterError.IsVisible = true;
    }

    private void HideErrorSection()
    {
        _lblRegisterError.IsVisible = false;
    }

    private string ToTitleCase(string inputText)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inputText.ToLower());
    }
    #endregion
}