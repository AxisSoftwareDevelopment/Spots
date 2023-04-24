using Spots.Models.DisplayManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;
using Spots.Models;

namespace Spots.Views.HomePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwHomePage : ContentPage
    {
        public vwHomePage()
        {
            BindingContext = RsrcManager.resourceCollection;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            _FrameProfilePicture.BindingContext = CurrentSession.currentUser;

            _MainView.Content = new cvMainView();
            _LeftView.Content = new cvUserSideMenu();
        }

        public void OpenLeftMenu(Object sender, EventArgs e)
        {
            _SideMenuView.State = SideMenuState.LeftMenuShown;
        }

        public void OpenRightMenu(Object sender, EventArgs e)
        {
            _SideMenuView.State = SideMenuState.RightMenuShown;
        }
    }
}