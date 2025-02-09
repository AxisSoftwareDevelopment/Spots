using Spots.Models;
using System.Runtime.CompilerServices;

namespace Spots;

public partial class FP_MainShell : FlyoutPage
{
    public static INavigation? MainNavigation;
    public static FlyoutPage? MainFlyout;
    public static event Action? FlyoutPresentedChanged;
    
	public FP_MainShell(Client user)
    {
        InitializeComponent();

        Detail = new NavigationPage(new TP_FeedViews());
        Flyout = new CP_SideUserMenu();

        MainNavigation = Detail.Navigation;
        MainFlyout = this;
    }

    public static void SetIsPresented(bool value)
    {
        if (MainFlyout != null)
        {
            MainFlyout.IsPresented = value;
        }
        if(value)
        {
            FlyoutPresentedChanged?.Invoke();
        }
    }

    public override bool ShouldShowToolbarButton()
    {
        return false;
    }
}