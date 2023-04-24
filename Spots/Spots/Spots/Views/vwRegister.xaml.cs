using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spots.Models.DisplayManager;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwRegister : ContentPage
    {
        string firstName;
        string lastName;
        string birthDate;

        public vwRegister(string firstName, string lastName, string birthDate)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.birthDate = birthDate;

            BindingContext = RsrcManager.resourceCollection;

            InitializeComponent();
        }

        public async void BtnRegisterOnClick(Object sender, EventArgs e)
        {
            LockInputs();

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
                        await Application.Current.MainPage.DisplayAlert("Success", "User created successfully.", "OK");
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        DisplayErrorSection("txt_RegisterError_RegisterError");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message.ToString(), "OK");
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

            UnlockInputs();
        }

        private bool ValidateEmail(string email)
        {
            Match eMailIsValid = Regex.Match(email, @"^([a-zA-Z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,5})$");

            return eMailIsValid.Success;
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

        private void LockInputs()
        {
            _btnRegister.IsEnabled = false;
            _entryEmail.IsEnabled = false;
            _entryConfirmEmail.IsEnabled = false;
            _entryPassword.IsEnabled = false;
            _entryConfirmPassword.IsEnabled = false;
        }

        private void UnlockInputs()
        {
            _btnRegister.IsEnabled = true;
            _entryEmail.IsEnabled = true;
            _entryConfirmEmail.IsEnabled = true;
            _entryPassword.IsEnabled = true;
            _entryConfirmPassword.IsEnabled = true;
        }
    }
}