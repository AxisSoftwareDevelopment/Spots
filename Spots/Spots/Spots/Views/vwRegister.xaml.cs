using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwRegister : ContentPage
    {
        #region Binding Attributes
        // Labels
        public string lbl_Register { get; set; }
        public string lbl_RegisterEmailField { get; set; }
        public string lbl_eMailPlaceHolder { get; set; }
        public string lbl_RegisterPasswordField { get; set; }
        public string lbl_PwdPlaceHolder { get; set; }
        public string lbl_RegisterConfirmPasswordField { get; set; }
        public string lbl_RegisterConfirmEmailField { get; set; }
        // Colors
        public string cl_MainBrand { get; set; }
        public string cl_BackGround { get; set; }
        public string cl_TextOnBG { get; set; }
        public string cl_TextOnElse { get; set; }
        public string cl_TextError { get; set; }
        #endregion

        string firstName;
        string lastName;
        string birthDate;

        public vwRegister(string firstName, string lastName, string birthDate)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.birthDate = birthDate;

            #region Resource Manager Setup
            // Load Reosurces
            lbl_Register = RsrcManager.GetText("lbl_Register");
            lbl_RegisterEmailField = RsrcManager.GetText("lbl_RegisterEmailField");
            lbl_eMailPlaceHolder = RsrcManager.GetText("lbl_eMailPlaceHolder");
            lbl_RegisterPasswordField = RsrcManager.GetText("lbl_RegisterPasswordField");
            lbl_PwdPlaceHolder = RsrcManager.GetText("lbl_PwdPlaceHolder");
            lbl_RegisterConfirmPasswordField = RsrcManager.GetText("lbl_RegisterConfirmPasswordField");
            lbl_RegisterConfirmEmailField = RsrcManager.GetText("lbl_RegisterConfirmEmailField");
            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColorHexCode("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColorHexCode("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColorHexCode("cl_TextOnElse");
            cl_TextError = RsrcManager.GetColorHexCode("cl_TextError");
            #endregion

            BindingContext = this;

            InitializeComponent();
        }

        public async void BtnRegisterOnClick(Object sender, EventArgs e)
        {
            string email = _entryEmail.Text is null ? "" : _entryEmail.Text;
            string confirmEmail = _entryConfirmEmail is null ? "" : _entryConfirmEmail.Text;
            string password = _entryPassword.Text is null ? "" : _entryPassword.Text;
            string confirmPassword = _entryConfirmPassword.Text is null ? "" : _entryConfirmPassword.Text;

            bool thereAreEmptyFields = ( email.Length == 0 ||
                                confirmEmail.Length == 0 ||
                                confirmPassword.Length == 0 );
            bool emailIsValid = ValidateEmail(email);
            bool emailsMatch = email.Equals( confirmEmail );
            bool passwrodIsValid = password.Length > 7;
            bool passwordsMatch = password.Equals( confirmPassword );
            
            if (!thereAreEmptyFields && emailIsValid && emailsMatch && passwrodIsValid && passwordsMatch)
            {
                HideErrorSection();

                try
                {
                    if (await DatabaseManager.CreateUserAsync(firstName, lastName, email, password, birthDate))
                    {
                        await Application.Current.MainPage.DisplayAlert("User created successfully", "placeholder", "OK");
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please, try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Invalid Credentials", ex.Message.ToString(), "OK");
                }
            }
            else
            {
                string errorMessageID = "Error on input fields";

                #region Error message calculation
                if (thereAreEmptyFields)
                {
                    errorMessageID = "txt_RegisterError_EmptyFields";
                }
                else if (!emailIsValid)
                {
                    errorMessageID = "txt_RegisterError_InvalidEmail";
                }
                else if (!emailsMatch)
                {
                    errorMessageID = "txt_RegisterError_EmailsDontMatch";
                }
                else if (!passwrodIsValid)
                {
                    errorMessageID = "txt_RegisterError_InvalidPasswordLength";
                }
                else if (!passwordsMatch)
                {
                    errorMessageID = "txt_RegisterError_PasswordsDontMatch";
                }
                #endregion

                DisplayErrorSection(errorMessageID);
            }
        }

        private bool ValidateEmail(string email)
        {
            bool eMailIsValid;
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                eMailIsValid = addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }

            return eMailIsValid;
        }

        private void DisplayErrorSection(string errorID)
        {
            _lblRegisterError.Text = RsrcManager.GetText(errorID);
            _lblRegisterError.IsVisible = true;
        }

        private void HideErrorSection()
        {
            _lblRegisterError.IsVisible = false;
        }
    }
}