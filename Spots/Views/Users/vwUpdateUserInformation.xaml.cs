using Spots.Models.DatabaseManagement;
using Spots.Models.ResourceManagement;
using Spots.Models.SessionManagement;
using Spots.Views.MainMenu;
using System.Globalization;

namespace Spots.Views.Users;

public partial class vwUpdateUserInformation : ContentPage
{
	private User _user;
	private bool _userIsEmpty;
    private string _password, _email;
    private bool _birhtdateSelected, _profilePictureChanged = false;
    private ImageFile _profilePictureFile;

    public vwUpdateUserInformation(User user, string email = null, string password = null)
	{
        _user = user;
        _userIsEmpty = password != null && email != null && !user.bUserDataRetrieved;
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
                _user.sEmail = _email;
            _user.sFirstName = firstName;
            _user.sLastName = lastName;
            if(_birhtdateSelected)
                _user.dtBirthDate = birthdate;
            //_user.profilePictureAddress
            _user.sPhoneNumber = phoneNumber;
            _user.sPhoneCountryCode = phoneCountryCode;
            _user.sDescription = description;
            if (_profilePictureChanged)
            {
                _user.sProfilePictureAddress = await DatabaseManager.SaveProfilePicture(isBusiness: false, _user.sUserID, _profilePictureFile);
                _user.imProfilePictureSource = ImageSource.FromStream( () => ImageManagement.ByteArrayToStream(_profilePictureFile.Bytes) );
            }

            if (await DatabaseManager.SaveUserDataAsync(_user))
            {
                
                _user.bUserDataRetrieved = true;
                await Application.Current.MainPage.DisplayAlert("Success", "Your information has been updated. Way to go!", "OK");
                // If the business was empty, it meas we came from the log in.
                if (_userIsEmpty)
                {
                    // We then have to log in and go to main page.
                    await DatabaseManager.LogInBusinessAsync(_email, _password, getUser: false);
                    Application.Current.MainPage = new vwMainShell(_user);
                }
                else if(DataChanged())
                {
                    // If the business was just updating information, then we just pop the page from navigation
                    CurrentSession.currentUser.UpdateUserData(_user);
                    
                    await Navigation.PopAsync();
                }
                else
                {
                    // If the business was updating information, but didnt change any data, we do nothing
                    await Application.Current.MainPage.DisplayAlert("Alert", "No information was changed", "OK");
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

    public async void LoadImageOnClickAsync(object sender, EventArgs e)
    {
        ImageFile image = await ImageManagement.PickImageFromInternalStorage();

        if (image != null) 
        {
            _profilePictureFile = image;
            _ProfileImage.Source = ImageSource.FromStream( () => ImageManagement.ByteArrayToStream(image.Bytes) );
            _profilePictureChanged = true;
        }
        
    }

    #region Utilities
    private void InitializeControllers()
    {
        // Load _user data
        _entryFirstName.Text = _user.sFirstName;
        _entryLastName.Text = _user.sLastName;
        _entryPhoneNumber.Text = _user.sPhoneNumber;
        _entryPhoneCountryCode.Text = _user.sPhoneCountryCode;
        _editorDescription.Text = _user.sDescription;
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
            _dateBirthdate.Date = _user.dtBirthDate.Date;
        }
    }

    private bool DataChanged()
    {
        if(_profilePictureChanged)
            return true;
        if (_user.sFirstName != ToTitleCase(_entryFirstName.Text.Trim()))
            return true;
        if (_user.sLastName != ToTitleCase(_entryLastName.Text.Trim()))
            return true;
        if (_user.sDescription != _editorDescription.Text.Trim())
            return true;
        if (_user.sPhoneNumber != _entryPhoneNumber.Text)
            return true;
        if(_user.sPhoneCountryCode != _entryPhoneCountryCode.Text)
            return true;
        if (_birhtdateSelected)
            return true;

        return false;
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