using Spots_v01.Recursos;
using System;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots_v01
{
    public partial class App : Application
    {
        public App()
        {
            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

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
