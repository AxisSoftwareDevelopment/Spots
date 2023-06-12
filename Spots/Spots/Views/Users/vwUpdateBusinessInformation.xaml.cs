using Spots.Models.DatabaseManagement;
using Spots.Models.ResourceManagement;
using Spots.Models.SessionManagement;
using Spots.Views.MainMenu;
using System.Globalization;

namespace Spots.Views.Users;

public partial class vwUpdateBusinessInformation : ContentPage
{
    private BusinessUser _user;
    private bool _userIsEmpty;
    private string _password, _email, _phoneNumber, _phoneCountryCode;
    private bool _birhtdateSelected, _profilePictureChanged = false;
    private ImageFile _profilePictureFile;

    public vwUpdateBusinessInformation(BusinessUser user, string email = null, string password = null, string phoneNumber = null, string phoneCountryCode = null)
    {
        _user = user;
        _userIsEmpty = password != null && email != null && !user.userDataRetrieved;
        _password = password;
        _email = email;
        _phoneNumber = phoneNumber;
        _phoneCountryCode = phoneCountryCode;

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
            _user.brandName = brandName;
            _user.businessName = BusinessName;
            //_user.profilePictureAddress
            _user.phoneNumber = phoneNumber;
            _user.phoneCountryCode = phoneCountryCode;
            _user.description = description;
            if (_profilePictureChanged)
            {
                _user.profilePictureAddress = await DatabaseManager.SaveProfilePicture(isBusiness: false, _user.userID, _profilePictureFile);
                _user.profilePictureSource = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(_profilePictureFile.Bytes));
            }

            if (await DatabaseManager.SaveBusinessDataAsync(_user))
            {
                _user.userDataRetrieved = true;
                await Application.Current.MainPage.DisplayAlert("Success", "Your information has been updated. Way to go!", "OK");
                // If the user was empty, it meas we came from the log in.
                if (_userIsEmpty)
                {
                    // We then have to log in and go to main page.
                    await DatabaseManager.LogInBusinessAsync(_email, _password, getUser: false);
                    Application.Current.MainPage = new vwMainShell(_user);
                }
                else if(DataChanged())
                {
                    // If the user was just updating information, then we just pop the page from navigation
                    CurrentSession.currentBusiness.UpdateUserData(_user);

                    await Navigation.PopAsync();
                }
                else
                {
                    // If the user was updating information, but didnt change any data, we do nothing
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
            _ProfileImage.Source = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(image.Bytes));
            _profilePictureChanged = true;
        }

    }

    #region Utilities
    private void InitializeControllers()
    {
        // Load _user data
        _entryBrandName.Text = _user.brandName;
        _entryBusinessName.Text = _user.businessName;
        _editorDescription.Text = _user.description;
        // Initialize BirthDate field
        if (_userIsEmpty)
        {
            _entryPhoneNumber.IsVisible = false;
            _entryPhoneCountryCode.IsVisible = false;
            _entryPhoneNumber.Text = _phoneNumber;
            _entryPhoneCountryCode.Text = _phoneCountryCode;
        }
        else if (_user.phoneNumber.Length > 0)
        {
            _entryPhoneNumber.Text = _user.phoneNumber;
            _entryPhoneCountryCode.Text = _user.phoneCountryCode;
        }
    }

    private bool DataChanged()
    {
        if (_profilePictureChanged)
            return true;
        if (_user.brandName != ToTitleCase(_entryBrandName.Text.Trim()))
            return true;
        if (_user.businessName != ToTitleCase(_entryBusinessName.Text.Trim()))
            return true;
        if (_user.description != _editorDescription.Text.Trim())
            return true;
        if (_user.phoneNumber != _entryPhoneNumber.Text)
            return true;
        if (_user.phoneCountryCode != _entryPhoneCountryCode.Text)
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