using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwRegister : ContentPage
    {
        #region Binding Attributes
        // Labels
        public string lbl_Register { get; set; }
        public string lbl_RegisterEmailField { get; set; }
        public string lbl_eMailPlaceHolder { get; set; }
        public string lbl_RegisterPasswordField { get; set; }
        public string lbl_PwdPlaceHolder { get; set; }
        public string lbl_RegisterConfirmPasswordField { get; set; }
        public string lbl_RegisterFirstName { get; set; }
        public string lbl_RegisterLastName { get; set; }
        public string lbl_FirstNamePlaceHolder { get; set; }
        public string lbl_LastNamePlaceHolder { get; set; }
        // Colors
        public string cl_MainBrand { get; set; }
        public string cl_BackGround { get; set; }
        public string cl_TextOnBG { get; set; }
        public string cl_TextOnElse { get; set; }
        #endregion
        public vwRegister()
        {
            #region Resource Manager Setup
            // Load Reosurces
            lbl_Register = RsrcManager.GetText("lbl_Register");
            lbl_RegisterEmailField = RsrcManager.GetText("lbl_RegisterEmailField");
            lbl_eMailPlaceHolder = RsrcManager.GetText("lbl_eMailPlaceHolder");
            lbl_RegisterPasswordField = RsrcManager.GetText("lbl_RegisterPasswordField");
            lbl_PwdPlaceHolder = RsrcManager.GetText("lbl_PwdPlaceHolder");
            lbl_RegisterConfirmPasswordField = RsrcManager.GetText("lbl_RegisterConfirmPasswordField");
            lbl_RegisterFirstName = RsrcManager.GetText("lbl_RegisterFirstName");
            lbl_RegisterLastName = RsrcManager.GetText("lbl_RegisterLastName");
            lbl_FirstNamePlaceHolder = RsrcManager.GetText("lbl_FirstNamePlaceHolder");
            lbl_LastNamePlaceHolder = RsrcManager.GetText("lbl_LastNamePlaceHolder");
            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColorHexCode("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColorHexCode("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColorHexCode("cl_TextOnElse");
            #endregion

            BindingContext = this;

            InitializeComponent();
        }
    }
}