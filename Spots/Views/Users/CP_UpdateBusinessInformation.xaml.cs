using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Globalization;

namespace Spots;

public partial class CP_UpdateBusinessInformation : ContentPage
{
	private BusinessUser _user;
    private bool _userIsEmpty;
    private string _password, _email, _phoneNumber, _phoneCountryCode;
    private bool _profilePictureChanged = false;
    private bool _locationChanged = false;
    private ImageFile? _profilePictureFile = null;

    public CP_UpdateBusinessInformation(BusinessUser user, string email = "", string password = "", string phoneNumber = "", string phoneCountryCode = "")
    {
        _user = user;
        _userIsEmpty = !password.Equals("") && !email.Equals("") && !user.userDataRetrieved;
        _password = password;
        _email = email;
        _phoneNumber = phoneNumber;
        _phoneCountryCode = phoneCountryCode;

        DisplayInfo displayInfo = DeviceDisplay.MainDisplayInfo;
        double profilePictureDimensions = displayInfo.Height * 0.065;

        InitializeComponent();

        if(LocationManager.CurrentLocation != null)
        {
            _cvMiniMap.Pins.Clear();
            _cvMiniMap.MoveToRegion(new MapSpan(LocationManager.CurrentLocation, 0.01, 0.01));
            _cvMiniMap.Pins.Add(new Pin() { Label = "", Location = LocationManager.CurrentLocation });
        }

        _FrameProfilePicture.HeightRequest = profilePictureDimensions;
        _FrameProfilePicture.WidthRequest = profilePictureDimensions;
        _cvMiniMap.HeightRequest = profilePictureDimensions * 0.75;
        _cvMiniMap.MapClicked += _cvMiniMap_MapClicked;

        if (_userIsEmpty)
            NavigationPage.SetHasBackButton(this, false);

        InitializeControllers();

        //_cvMiniMap.Pins[0].MarkerClicked += _cvMiniMap_MarkerClicked;
    }

    private void _cvMiniMap_MapClicked(object? sender, MapClickedEventArgs e)
    {
        Navigation.PushAsync(new CP_MapLocationSelector(()=>_cvMiniMap.VisibleRegion, SetMiniMapVisibleArea, _entryAddress.Text ?? ""));
    }

    private void SetMiniMapVisibleArea(MapSpan mapSpan, string address)
    {
        _locationChanged = true;
        _cvMiniMap.MoveToRegion(mapSpan);
        _cvMiniMap.Pins.Clear();
        _cvMiniMap.Pins.Add(new Pin() { Label = address, Location = mapSpan.Center });
        _entryAddress.Text = address;
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
        if (Application.Current == null)
        {
            return;
        }

        BusinessUser newData = new BusinessUser()
        {
            BrandName = ToTitleCase(_entryBrandName.Text.Trim()),
            BusinessName = ToTitleCase(_entryBusinessName.Text.Trim()),
            Location = new FirebaseLocation(_entryAddress.Text.Trim(), 0, 0),
            Description = _editorDescription.Text.Trim(),
            PhoneNumber = _entryPhoneNumber.Text,
            PhoneCountryCode = _entryPhoneCountryCode.Text
        };

        if (ValidateFields(newData))
        {
            HideErrorSection();

            if (_userIsEmpty)
            {
                _user.Email = _email;
                _user.PhoneNumber = _phoneNumber;
                _user.PhoneCountryCode = _phoneCountryCode;
            }
            else if(DataChanged(newData))
            {
                _user.PhoneNumber = newData.PhoneNumber;
                _user.PhoneCountryCode = newData.PhoneCountryCode;
                _user.BrandName = newData.BrandName;
                _user.BusinessName = newData.BusinessName;
                _user.Description = newData.Description;
                Location locationSelected = _cvMiniMap.Pins[0].Location;
                _user.Location = new FirebaseLocation(newData.Location.Address, locationSelected.Latitude, locationSelected.Longitude);
                if (_profilePictureChanged && _profilePictureFile != null)
                {
                    _user.ProfilePictureAddress = await DatabaseManager.SaveProfilePicture(isBusiness: true, _user.UserID, _profilePictureFile);
                    _user.ProfilePictureSource = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(_profilePictureFile.Bytes?? []));
                }

                if (await DatabaseManager.SaveBusinessDataAsync(_user))
                {
                    _user.userDataRetrieved = true;
                    await UserInterface.DisplayPopUp("Success", "Your information has been updated. Way to go!", "OK");
                    // If the business was empty, it meas we came from the log in.
                    if (_userIsEmpty)
                    {
                        // We then have to log in and go to main page.
                        await DatabaseManager.LogInBusinessAsync(_email, _password, getUser: false);
                        Application.Current.MainPage = new FP_MainShell(_user);
                    }
                    else
                    {
                        // If the business was just updating information, then we just pop the page from navigation
                        CurrentSession.currentBusiness.UpdateUserData(_user);

                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                // If the business was updating information, but didnt change any data, we do nothing
                await UserInterface.DisplayPopUp("Alert", "No information was changed", "OK");
                await Navigation.PopAsync();
            }

        }
        
    }

    public async void LoadImageOnClickAsync(object sender, EventArgs e)
    {
        ImageFile? image = await ImageManagement.PickImageFromInternalStorage();

        if (image != null)
        {
            _profilePictureFile = image;
            _ProfileImage.Source = ImageSource.FromStream(() => ImageManagement.ByteArrayToStream(image.Bytes?? []));
            _profilePictureChanged = true;
        }

    }

    #region Utilities
    private void InitializeControllers()
    {
        // Load _user data
        _entryBrandName.Text = _user.BrandName;
        _entryBusinessName.Text = _user.BusinessName;
        _editorDescription.Text = _user.Description;
        _ProfileImage.Source = _user.ProfilePictureSource;

        // Initialize BirthDate field
        if (_userIsEmpty)
        {
            _entryPhoneNumber.IsVisible = false;
            _entryPhoneCountryCode.IsVisible = false;
            _lblPhoneNumber.IsVisible = false;
            _lblPlusPhoneNumber.IsVisible = false;
            _entryPhoneNumber.Text = _phoneNumber;
            _entryPhoneCountryCode.Text = _phoneCountryCode;
        }
        else 
        {
            _entryAddress.Text = _user.Location.Address;
            _cvMiniMap.MoveToRegion(new MapSpan(_user.Geolocation, 0.01, 0.01));
            _cvMiniMap.Pins.Clear();
            _cvMiniMap.Pins.Add(new Pin()
            {
                Label = _user.Location.Address,
                Location = _user.Geolocation
            });
            if (_user.PhoneNumber.Length > 0)
            {
                _entryPhoneNumber.Text = _user.PhoneNumber;
                _entryPhoneCountryCode.Text = _user.PhoneCountryCode;
            }
        }
    }

    private bool ValidateFields(BusinessUser business)
    {
        bool thereAreEmptyFields = business.BrandName.Length == 0 ||
                            business.BusinessName.Length == 0 ||
                            business.Location.Address.Length == 0;
        bool validLocationSelected = _cvMiniMap.Pins.Count == 1;
        bool descriptionUnder150Chars = business.Description.Length <= 150;
        bool validPhoneNumber;
        if (_userIsEmpty)
            validPhoneNumber = true;
        else
            validPhoneNumber = (business.PhoneNumber.Length == 8 && business.PhoneCountryCode.Length == 2) 
                || (business.PhoneNumber.Length == 0 && business.PhoneCountryCode.Length == 0);

        if(thereAreEmptyFields || !descriptionUnder150Chars || !validPhoneNumber || !validLocationSelected)
        {
            string errorMessageID = "txt_Error_UnkownError";

            #region Error message calculation
            if (!validLocationSelected)
            {
                errorMessageID = "txt_BussinessError_NoValidLocationSelected";
            }
            else if (thereAreEmptyFields)
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

            return false;
        }

        return true;
    }

    private bool DataChanged(BusinessUser business)
    {
        if (_profilePictureChanged)
            return true;
        if (_locationChanged)
            return true;
        if (_user.BrandName != business.BrandName)
            return true;
        if (_user.BusinessName != business.BusinessName)
            return true;
        if (_user.Description != business.Description)
            return true;
        if (_user.PhoneNumber != business.PhoneNumber)
            return true;
        if (_user.PhoneCountryCode != business.PhoneCountryCode)
            return true;
        if (_user.Location.Address != business.Location.Address)
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