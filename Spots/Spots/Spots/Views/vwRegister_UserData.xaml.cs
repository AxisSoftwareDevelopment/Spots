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
    public partial class vwRegister_UserData : ContentPage
    {
        #region Binding Attributes
        // Labels
        public string lbl_Next { get; set; }
        public string lbl_RegisterFirstName { get; set; }
        public string lbl_RegisterLastName { get; set; }
        public string lbl_FirstNamePlaceHolder { get; set; }
        public string lbl_LastNamePlaceHolder { get; set; }
        public string lbl_BirthdateField { get; set; }
        public string lbl_UserData { get; set; }
        // Colors
        public string cl_MainBrand { get; set; }
        public string cl_BackGround { get; set; }
        public string cl_TextOnBG { get; set; }
        public string cl_TextOnElse { get; set; }
        public string cl_TextError { get; set; }
        #endregion

        private bool birhtdateSelected = false;

        public vwRegister_UserData()
        {
            #region Resource Manager Setup
            // Load Reosurces
            lbl_Next = RsrcManager.GetText("lbl_Next");
            lbl_RegisterFirstName = RsrcManager.GetText("lbl_RegisterFirstName");
            lbl_RegisterLastName = RsrcManager.GetText("lbl_RegisterLastName");
            lbl_FirstNamePlaceHolder = RsrcManager.GetText("lbl_FirstNamePlaceHolder");
            lbl_LastNamePlaceHolder = RsrcManager.GetText("lbl_LastNamePlaceHolder");
            lbl_BirthdateField = RsrcManager.GetText("lbl_BirthdateField");
            lbl_UserData = RsrcManager.GetText("lbl_UserData");
            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColorHexCode("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColorHexCode("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColorHexCode("cl_TextOnElse");
            cl_TextError = RsrcManager.GetColorHexCode("cl_TextError");
            #endregion

            BindingContext = this;

            InitializeComponent();

            _dateBirthdate.MaximumDate = DateTime.Today;
            _dateBirthdate.DateSelected += (o, e) =>
            {
                _dateBirthdate.Format = "MM/dd/yyyy";
                _dateBirthdate.TextColor = ColorConverters.FromHex(cl_TextOnBG);
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