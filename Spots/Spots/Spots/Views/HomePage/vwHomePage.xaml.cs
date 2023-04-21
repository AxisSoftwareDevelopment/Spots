using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;

namespace Spots.Views.HomePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwHomePage : ContentPage
    {
        #region Binding Attributes
        // Labels

        // Colors
        public string cl_MainBrand { get; set; } = RsrcManager.GetColorHexCode("cl_MainBrand");
        public string cl_BackGround { get; set; } = RsrcManager.GetColorHexCode("cl_BackGround");
        public string cl_TextOnBG { get; set; } = RsrcManager.GetColorHexCode("cl_TextOnBG");
        // Images
        public string img_Logo { get; set; } = RsrcManager.GetImagePath("img_Logo");
        #endregion
        public vwHomePage()
        {
            BindingContext = this;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
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