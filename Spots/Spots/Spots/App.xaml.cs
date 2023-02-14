using Spots.Models;
using Spots.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots
{
    public partial class App : Application
    {
        public string cl_MainBrand { get; set; }
        public App()
        {
            InitializeComponent();

            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");

            BindingContext = this;

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
