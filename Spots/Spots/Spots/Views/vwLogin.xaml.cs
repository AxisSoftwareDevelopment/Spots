using Firebase.Auth;
using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

            btnLogIn.Clicked += (object sender, EventArgs e) => AttempLogin(sender, e);

            BindingContext = this;
        }

        private void AttempLogin(object sender, EventArgs e)
        {
            if(entryEmail.Text.Length > 0 && entryPassword.Text.Length > 0)
            {
                HideErrorSection();
                // Look for user in database

                //Temp functionality ->
                try
                {
                    Task<UserCredential> signIn = FireBaseManager.SignInAsync(entryEmail.Text, entryPassword.Text);
                    signIn.Wait();

                    if (signIn != null)
                    {
                        Application.Current.MainPage.DisplayAlert("", signIn.Result.User.Credential.ToString(), "OK");
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("UserCrd is null", "Es nullo", "OK");
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.MainPage.DisplayAlert("Tiro excepcion", ex.Message.ToString(), "OK");
                }
            }
            else
            {
                // Raise "No enough data" exception
                DisplayErrorSection("txt_LogInError_EmptyEntry");
            }
        }

        private void OpenRegisterView()
        {
            Application.Current.MainPage.DisplayAlert("Register view", "Register view.", "Ok");
        }

        private void DisplayErrorSection(string errorID)
        {
            lblSignInError.Text = RsrcManager.GetText(errorID);
            lblSignInError.IsVisible = true;
        }

        private void HideErrorSection()
        {
            lblSignInError.IsVisible = false;
        }
    }
}