using Spots.Models;
using Spots.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots
{
    public partial class App : Application
    {
        private protected string FIREBASE_APIKEY = "AIzaSyBsmEji5qtAuiCk_llFUgZ8OBY1WLvtifA";
        public App()
        {
            InitializeComponent();

            bool appStatus = FireBaseManager.StartConnection(FIREBASE_APIKEY);

            MainPage = new vwLogin();

            if (appStatus != true)
            {
                Application.Current.MainPage.DisplayAlert("No Internet Connection", "No Internet Connection.", "Ok");
            }
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
