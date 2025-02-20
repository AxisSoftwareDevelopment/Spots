#if ANDROID
using AndroidX.AppCompat.App;
#endif

namespace eatMeet;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		UserAppTheme = AppTheme.Light;
#if ANDROID
        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
#endif
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new CP_WaitForValidation());
    }
}
