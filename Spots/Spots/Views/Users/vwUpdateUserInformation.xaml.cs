using Spots.Models.DatabaseManagement;
using Spots.Models.SessionManagement;
using Spots.Views.MainMenu;
using System.Globalization;

namespace Spots.Views.Users;

public partial class vwUpdateUserInformation : ContentPage
{
	private User _user;
	private bool _userIsEmpty;
    private string _password, _email;
    private bool _birhtdateSelected = false;

    public vwUpdateUserInformation(User user, string email = null, string password = null)
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
        {
            _boolHasBackButton = false;
        }

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
        string firstName = ToTitleCase(_entryFirstName.Text.Trim());
        string lastName = ToTitleCase(_entryLastName.Text.Trim());
        string description = _editorDescription.Text.Trim();
        string phoneNumber = _entryPhoneNumber.Text;
        string phoneCountryCode = _entryPhoneCountryCode.Text;
        DateTimeOffset birthdate = _dateBirthdate.Date;

        bool thereAreEmptyFields = firstName.Length == 0 ||
                            lastName.Length == 0 ||
                            (!_birhtdateSelected && _userIsEmpty);
        bool descriptionUnder150Chars = description.Length <= 150;
        bool validPhoneNumber = (phoneNumber.Length == 10 && phoneCountryCode.Length == 2) || (phoneNumber.Length == 0 && phoneCountryCode.Length == 0);
        bool birthdateIsValid = (DateTime.Today.Year - _dateBirthdate.Date.Year) > 12;

        if (!thereAreEmptyFields && birthdateIsValid && descriptionUnder150Chars && validPhoneNumber)
        {
            HideErrorSection();

            if(_userIsEmpty)
                _user.email = _email;
            _user.firstName = firstName;
            _user.lastName = lastName;
            if(_birhtdateSelected)
                _user.birthDate = birthdate;
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
        _entryFirstName.Text = _user.firstName;
        _entryLastName.Text = _user.lastName;
        _entryPhoneNumber.Text = _user.phoneNumber;
        _entryPhoneCountryCode.Text = _user.phoneCountryCode;
        _editorDescription.Text = _user.description;
        // Initialize BirthDate field
        if (_userIsEmpty)
        {
            _dateBirthdate.Format = "--/--/----";
            _dateBirthdate.MaximumDate = DateTime.Today;
            _dateBirthdate.DateSelected += (o, e) =>
            {
                _dateBirthdate.Format = "MM/dd/yyyy";
                _dateBirthdate.SetDynamicResource(DatePicker.TextColorProperty, "SecondaryAccent");
                _birhtdateSelected = true;
            };
        }
        else
        {
            _dateBirthdate.Date = _user.birthDate.Date;
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