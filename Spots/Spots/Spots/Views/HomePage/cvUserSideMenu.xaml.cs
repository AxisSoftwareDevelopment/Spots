using Spots.Models;
using System;
using System.ComponentModel;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views.HomePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class cvUserSideMenu : ContentView, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public cvUserSideMenu()
        {
            InitializeComponent();

            BindingContext = CurrentSession.currentUser;
        }

        void ProfilePictureOnClicked(object sender, EventArgs e)
        {
            // Go to User Profile page
        }
    }
}