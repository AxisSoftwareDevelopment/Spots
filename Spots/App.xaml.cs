﻿using Spots.Models.DatabaseManagement;
using Spots.Views;
using Spots.Views.MainMenu;
using Spots.Views.Maps;

#if ANDROID
using AndroidX.AppCompat.App;
#endif

namespace Spots;

public partial class App : Application
{
	public App()
	{
        
        InitializeComponent();

        UserAppTheme = AppTheme.Light;
#if ANDROID
        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
#endif
        MainPage = new vwWaitForValidation();
        //MainPage = new vwMapLocationSelector();
    }
}