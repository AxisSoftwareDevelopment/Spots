﻿using Spots.Models;
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

            MainPage = new vwLogin();
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