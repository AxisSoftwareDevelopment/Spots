using Spots.Models;
using Spots.Models.DisplayManager;
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
    public partial class vwRegister_UserData : ContentPage
    {
        private bool birhtdateSelected = false;

        public vwRegister_UserData()
        {
            BindingContext = this;

            InitializeComponent();

            // Initialize BirthDate field
            _dateBirthdate.Format = "--/--/----";
            _dateBirthdate.MaximumDate = DateTime.Today;
            _dateBirthdate.DateSelected += (o, e) =>
            {
                _dateBirthdate.Format = "MM/dd/yyyy";
                _dateBirthdate.TextColor = ColorConverters.FromHex(RsrcManager.resourceCollection.cl_TextOnBG);
                birhtdateSelected = true;
            };
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