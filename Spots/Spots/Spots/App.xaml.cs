using Spots.Models;
using Spots.Views;
using Spots.Models.DisplayManager;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            BindingContext = RsrcManager.resourceCollection;
            DatabaseManager.Load();

            MainPage = new NavigationPage(new vwLogin());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
