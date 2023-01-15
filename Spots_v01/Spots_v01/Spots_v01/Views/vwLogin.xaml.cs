using GalaSoft.MvvmLight.Command;
using Spots_v01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
//using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots_v01.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwLogin : ContentPage
    {
        #region Binding atttributes
        // Labels
        public string lbl_LogIn { get; set; }
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
        // Commands
        public ICommand LoginCommand { get; set; }
        #endregion

        #region Model Attributes
        private RsrcManager rsrcManager;
        #endregion

        public vwLogin()//Librerias.Preferencias _AppSettings, Librerias.Traductor _Traductor)
        {
            InitializeComponent();

            #region Resource Manager Setup
            // Load Reosurces
            rsrcManager = new RsrcManager();

            lbl_LogIn = rsrcManager.GetText("lbl_LogIn");
            lbl_eMailPlaceHolder = rsrcManager.GetText("lbl_eMailPlaceHolder");
            lbl_PwdPlaceHolder = rsrcManager.GetText("lbl_PwdPlaceHolder");
            txt_LogIn = rsrcManager.GetText("txt_LogIn");
            cl_MainBrand = rsrcManager.GetColor("cl_MainBrand");
            cl_BackGround = rsrcManager.GetColor("cl_BackGround");
            cl_TextOnBG = rsrcManager.GetColor("cl_TextOnBG");
            cl_TextOnElse = rsrcManager.GetColor("cl_TextOnElse");
            cl_TextError = rsrcManager.GetColor("cl_TextError");
            #endregion

            LoginCommand = new RelayCommand(Login);

            BindingContext = this;
        }

        private void Login()
        {
            if(entryEmail.Text.Length > 0 && entryPassword.Text.Length > 0)
            {
                // Look for user in database

                //Temp functionality ->
                if (entryEmail.Text == "mail@mail.com" && entryEmail.Text == "12345678")
                {
                    Application.Current.MainPage.DisplayAlert("Succesfull Log in", "Your credentials are correct.", "Ok");
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("User not in Databse", "The eMail or password are not correct. Please try again.", "Ok");
                }
            }
            else
            {
                // Raise "No enough data" exception
                DisplayErrorSection("txt_LogInError_EmptyEntry");
            }
        }

        private void DisplayErrorSection(string errorID)
        {
            lblSignInError.Text = rsrcManager.GetText(errorID);
            lblSignInError.IsVisible = true;
        }
    }
}