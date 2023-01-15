using Spots_v01.Librerias;
using Spots_v01.Models;
using Spots_v01.Recursos;
using System;
using System.Resources;
//using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots_v01
{
    public partial class App : Application
    {
        Librerias.Preferencias appSettings;
        Librerias.Traductor traductor;
        public App()
        {
            //appSettings = new Librerias.Preferencias();
            //traductor = new Librerias.Traductor();

            InitializeComponent();

            MainPage = new Views.vwLogin();//appSettings, traductor);
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
