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
        // Images
        public string img_Logo { get; set; }
        #endregion

        #region Model Attributes
        private IAuth auth;
        #endregion

        #region Constants
        private const uint MILISECONDS_STARTUP_ANIMATION = 600;
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
            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColorHexCode("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColorHexCode("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColorHexCode("cl_TextOnElse");
            cl_TextError = RsrcManager.GetColorHexCode("cl_TextError");
            cl_MainGray = RsrcManager.GetColorHexCode("cl_MainGray");
            img_Logo = RsrcManager.GetImagePath("img_Logo");
            #endregion

            BindingContext = this;

            auth = DependencyService.Get<IAuth>();

            #region Animations
            RunAnimationsAsync();
            #endregion
        }

        private async void AttempLogin(object sender, EventArgs e)
        {
            SwitchLockViewState();

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
                        if (usrId.Length > 0)
                        {
                            await Application.Current.MainPage.DisplayAlert("dio algo", usrId, "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Nada", "nada", "OK");
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

            SwitchLockViewState();
        }

        private void OpenRegisterView(object sender, EventArgs e)
        {
            Navigation.PushAsync(new vwRegister());
        }

        private void SwitchLockViewState()
        {
            _btnLogIn.IsEnabled = ! _btnLogIn.IsEnabled;
            _btnRegister.IsEnabled = ! _btnRegister.IsEnabled;
        }

        private async void RunAnimationsAsync()
        {
            await Task.WhenAll(
                AnimateTranslateTo(_imgLogo, offset_X: 0, offset_Y: 900),
                AnimateTranslateTo(_frameEntries, offset_X: 0, offset_Y: 700)
            );
            await Task.WhenAll(
                AnimateTranslateTo(_imgLogo, offset_X: 0, offset_Y: - 900, MILISECONDS_STARTUP_ANIMATION),
                AnimateTranslateTo(_frameEntries, offset_X: 0, offset_Y: - 700, MILISECONDS_STARTUP_ANIMATION)
            );

            SwitchLockViewState();
        }

        private async Task<bool> AnimateTranslateTo(VisualElement obj, double offset_X, double offset_Y, uint length = 0)
        {
            return await obj.TranslateTo(obj.TranslationX + offset_X, obj.TranslationY + offset_Y, length);
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