using Android.Telephony;
using Spots.Models;
using Spots.Models.DisplayManager;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views.HomePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class cvUserSideMenu : ContentView
    {
        public cvUserSideMenu()
        {
            InitializeComponent();

            BindingContext = CurrentSession.currentUser;
            _ButtonStack.BindingContext = RsrcManager.resourceCollection;
        }

        void ProfilePictureOrMyProfileOnClicked(object sender, EventArgs e)
        {
            // Go to User Profile page
        }

        void PreferencesOnClicked(object sender, EventArgs e)
        {

        }

        async void LogOutOnClickedAsync(object sender, EventArgs e)
        {
            if (await Application.Current.MainPage.DisplayAlert(RsrcManager.resourceCollection.lbl_AreYouSure, RsrcManager.resourceCollection.txt_ConfirmLogOut, 
                RsrcManager.resourceCollection.lbl_LogOut, RsrcManager.resourceCollection.lbl_No))
            {
                DatabaseManager.LogOut();
                Application.Current.MainPage = new NavigationPage(new vwLogin());
            }
        }
    }
}