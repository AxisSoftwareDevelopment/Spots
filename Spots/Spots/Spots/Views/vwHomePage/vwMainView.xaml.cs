using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views.vwHomePage
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class vwMainView : ContentView
	{
        #region Binding Attributes
        // Labels
        public string lbl_Menu_1 { get; set; } = RsrcManager.GetText(Preferences.Get("lbl_Menu_1", "lbl_Feed"));
        public string lbl_Menu_2 { get; set; } = RsrcManager.GetText(Preferences.Get("lbl_Menu_2", "lbl_Discovery"));
        public string lbl_Menu_3 { get; set; } = RsrcManager.GetText(Preferences.Get("lbl_Menu_3", "lbl_MyPraises"));
        // Colors
        public string cl_MainBrand { get; set; } = RsrcManager.GetColorHexCode("cl_MainBrand");
        public string cl_BackGround { get; set; } = RsrcManager.GetColorHexCode("cl_BackGround");
        public string cl_TextOnBG { get; set; } = RsrcManager.GetColorHexCode("cl_TextOnBG");
        #endregion
        public vwMainView ()
		{
			BindingContext = this;
			InitializeComponent ();
		}
	}
}