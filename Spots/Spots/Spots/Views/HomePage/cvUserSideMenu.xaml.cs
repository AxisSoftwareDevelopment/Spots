using Spots.Models;
using Spots.Models.DisplayManager;
using System;
using System.ComponentModel;


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

        void ProfilePictureOnClicked(object sender, EventArgs e)
        {
            // Go to User Profile page
        }
    }
}