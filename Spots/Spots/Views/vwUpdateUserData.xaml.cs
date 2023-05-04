using Plugin.Firebase.Auth;
using System.Drawing;

namespace Spots.Views;

public partial class vwUpdateUserData : ContentPage
{
	bool isNewUser;
	IFirebaseUser user;
    private bool birhtdateSelected = false;

    public vwUpdateUserData()
	{
		isNewUser = true;

		InitializeComponent();
        Resources = Application.Current.Resources;

        InitializeControllers();
    }

	public vwUpdateUserData(IFirebaseUser user)
	{
		isNewUser = false;
		this.user = user;

        InitializeComponent();
        Resources = Application.Current.Resources;

        InitializeControllers();
    }

    public async void BtnRegisterOnClick(Object sender, EventArgs e)
    {
        string firstName = _entryFirstName.Text is null ? "" : _entryFirstName.Text;
        string lastName = _entryLastName.Text is null ? "" : _entryLastName.Text;
        string birthdate = _dateBirthdate.Date.ToString();

        bool thereAreEmptyFields = (firstName.Length == 0 ||
                            lastName.Length == 0 ||
                            !birhtdateSelected);
        bool birthdateIsValid = (DateTime.Today.Year - _dateBirthdate.Date.Year) > 12;

        if (!thereAreEmptyFields && birthdateIsValid)
        {
            HideErrorSection();

            await Navigation.PushAsync(new vwRegister(firstName, lastName, birthdate));
        }
        else
        {
            string errorMessageID = "Error on input fields";

            #region Error message calculation
            if (thereAreEmptyFields)
            {
                errorMessageID = "txt_RegisterError_EmptyFields";
            }
            else if (!birthdateIsValid)
            {
                errorMessageID = "txt_RegisterError_InvalidBirthdate";
            }
            #endregion

            DisplayErrorSection(errorMessageID);
        }
    }

    #region Utilities
    private void InitializeControllers()
    {
        // Initialize BirthDate field
        _dateBirthdate.Format = "--/--/----";
        _dateBirthdate.MaximumDate = DateTime.Today;
        _dateBirthdate.DateSelected += (o, e) =>
        {
            _dateBirthdate.Format = "MM/dd/yyyy";
            _dateBirthdate.SetDynamicResource(DatePicker.TextColorProperty, "cl_TextOnBG");
            birhtdateSelected = true;
        };

        // Set Button Text Propperty
        if (isNewUser)
            _btnSave.SetDynamicResource(Button.TextProperty, "lbl_Next");
        else
        {
            _btnSave.SetDynamicResource(Button.TextProperty, "lbl_Save");
            LoadCurrentUserData();
        }
    }

    private void LoadCurrentUserData()
    {
        // Get User Data and Fill the fields
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
    #endregion
}