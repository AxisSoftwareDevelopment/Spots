using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
//using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwLogin : ContentPage
    {
        #region Binding atttributes
        // Labels
        public string lbl_LogIn { get; set; }
        public string lbl_Register { get; set; }
        public string lbl_eMailPlaceHolder { get; set; }
        public string lbl_PwdPlaceHolder { get; set; }
        // Texts
        public string txt_LogIn { get; set; }
        // Colors
        public string cl_MainBrand { get; set; }
        public string cl_BackGround { get; set; }
        public string cl_TextOnBG { get; set; }
        public string cl_TextOnElse { get; set; }
        public string cl_TextError { get; set; }
        public string cl_MainGray { get; set; }
        #endregion

        #region Model Attributes
        private IAuth auth;
        #endregion

        public vwLogin()//Librerias.Preferencias _AppSettings, Librerias.Traductor _Traductor)
        {
            InitializeComponent();

            #region Resource Manager Setup
            // Load Reosurces
            lbl_LogIn = RsrcManager.GetText("lbl_LogIn");
            lbl_eMailPlaceHolder = RsrcManager.GetText("lbl_eMailPlaceHolder");
            lbl_PwdPlaceHolder = RsrcManager.GetText("lbl_PwdPlaceHolder");
            lbl_Register = RsrcManager.GetText("lbl_Register");
            txt_LogIn = RsrcManager.GetText("txt_LogIn");
            cl_MainBrand = RsrcManager.GetColor("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColor("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColor("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColor("cl_TextOnElse");
            cl_TextError = RsrcManager.GetColor("cl_TextError");
            cl_MainGray = RsrcManager.GetColor("cl_MainGray");
            #endregion

            BindingContext = this;

            auth = DependencyService.Get<IAuth>();
        }

        private async void AttempLogin(object sender, EventArgs e)
        {
            string email = _entryEmail.Text;
            string password = _entryPassword.Text;
            if (EmailIsValid(email) && PasswordIsValid(password))
            {
                HideErrorSection();
                // Look for user in database
                try
                {
                    string usrId = await auth.LogInWithEmailAndPasswordAsync(email, password);
                    
                    if (usrId != null)
                    {
                        if (usrId.Length == 0)
                        {
                            await Application.Current.MainPage.DisplayAlert("Nada", "nada", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("dio algo", usrId, "OK");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Tiro excepcion", ex.Message.ToString(), "OK");
                }
            }
            else
            {
                // Raise "No enough data" exception
                DisplayErrorSection("txt_LogInError_EmptyEntry");
            }
        }

        private void OpenRegisterView(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("Register view", "Register view.", "Ok");
        }

        private void DisplayErrorSection(string errorID)
        {
            _lblSignInError.Text = RsrcManager.GetText(errorID);
            _lblSignInError.IsVisible = true;
        }

        private void HideErrorSection()
        {
            _lblSignInError.IsVisible = false;
        }

        private bool EmailIsValid(string email)
        {
            bool isNotEmpty = email.Length > 0;
            return isNotEmpty;
        }

        private bool PasswordIsValid(string password)
        {
            bool isNotEmpty = password.Length > 0;
            return isNotEmpty;
        }
    }
}