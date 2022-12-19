using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots_v01
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Views.vwLogin();
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
