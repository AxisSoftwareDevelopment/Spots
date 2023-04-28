using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Spots.Models.DisplayManager
{
    public class ResourceCollection : BindableObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Properties
        // Labels
        private string _lbl_LogIn;
        private string _lbl_Register;
        private string _lbl_eMailPlaceHolder;
        private string _lbl_PwdPlaceHolder;
        private string _lbl_RegisterEmailField;
        private string _lbl_RegisterPasswordField;
        private string _lbl_RegisterConfirmPasswordField;
        private string _lbl_RegisterConfirmEmailField;
        private string _lbl_Next;
        private string _lbl_RegisterFirstName;
        private string _lbl_RegisterLastName;
        private string _lbl_FirstNamePlaceHolder;
        private string _lbl_LastNamePlaceHolder;
        private string _lbl_BirthdateField;
        private string _lbl_UserData;
        private string _lbl_Menu_1;
        private string _lbl_Menu_2;
        private string _lbl_Menu_3;
        private string _lbl_MyProfile;
        private string _lbl_Preferences;
        private string _lbl_LogOut;
        private string _lbl_AreYouSure;
        private string _lbl_Yes;
        private string _lbl_No;

        // Texts
        private string _txt_LogIn;
        private string _txt_ConfirmLogOut;

        // Colors
        private string _cl_MainBrand;
        private string _cl_BackGround;
        private string _cl_TextOnBG;
        private string _cl_TextOnElse;
        private string _cl_TextError;
        private string _cl_MainGray;

        // Images
        private string _img_Logo;
        #endregion

        #region Public Properties
        // Labels
        public string lbl_LogIn
        {
            get { return _lbl_LogIn; }
            set
            {
                _lbl_LogIn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_LogIn)));
            }
        }
        public string lbl_Register
        {
            get { return _lbl_Register; }
            set
            {
                _lbl_Register = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Register)));
            }
        }
        public string lbl_eMailPlaceHolder
        {
            get { return _lbl_eMailPlaceHolder; }
            set
            {
                _lbl_eMailPlaceHolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_eMailPlaceHolder)));
            }
        }
        public string lbl_PwdPlaceHolder
        {
            get { return _lbl_PwdPlaceHolder; }
            set
            {
                _lbl_PwdPlaceHolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_PwdPlaceHolder)));
            }
        }
        public string lbl_RegisterEmailField
        {
            get { return _lbl_RegisterEmailField; }
            set
            {
                _lbl_RegisterEmailField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterEmailField)));
            }
        }
        public string lbl_RegisterPasswordField
        {
            get { return _lbl_RegisterPasswordField; }
            set
            {
                _lbl_RegisterPasswordField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterPasswordField)));
            }
        }
        public string lbl_RegisterConfirmPasswordField
        {
            get { return _lbl_RegisterConfirmPasswordField; }
            set
            {
                _lbl_RegisterConfirmPasswordField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterConfirmPasswordField)));
            }
        }
        public string lbl_RegisterConfirmEmailField
        {
            get { return _lbl_RegisterConfirmEmailField; }
            set
            {
                _lbl_RegisterConfirmEmailField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterConfirmEmailField)));
            }
        }
        public string lbl_Next
        {
            get { return _lbl_Next; }
            set
            {
                _lbl_Next = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Next)));
            }
        }
        public string lbl_RegisterFirstName
        {
            get { return _lbl_RegisterFirstName; }
            set
            {
                _lbl_RegisterFirstName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterFirstName)));
            }
        }
        public string lbl_RegisterLastName
        {
            get { return _lbl_RegisterLastName; }
            set
            {
                _lbl_RegisterLastName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_RegisterLastName)));
            }
        }
        public string lbl_FirstNamePlaceHolder
        {
            get { return _lbl_FirstNamePlaceHolder; }
            set
            {
                _lbl_FirstNamePlaceHolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_FirstNamePlaceHolder)));
            }
        }
        public string lbl_LastNamePlaceHolder
        {
            get { return _lbl_LastNamePlaceHolder; }
            set
            {
                _lbl_LastNamePlaceHolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_LastNamePlaceHolder)));
            }
        }
        public string lbl_BirthdateField
        {
            get { return _lbl_BirthdateField; }
            set
            {
                _lbl_BirthdateField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_BirthdateField)));
            }
        }
        public string lbl_UserData
        {
            get { return _lbl_UserData; }
            set
            {
                _lbl_UserData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_UserData)));
            }
        }
        public string lbl_Menu_1
        {
            get { return _lbl_Menu_1; }
            set
            {
                _lbl_Menu_1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Menu_1)));
            }
        }
        public string lbl_Menu_2
        {
            get { return _lbl_Menu_2; }
            set
            {
                _lbl_Menu_2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Menu_2)));
            }
        }
        public string lbl_Menu_3
        {
            get { return _lbl_Menu_3; }
            set
            {
                _lbl_Menu_3 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Menu_3)));
            }
        }
        public  string lbl_MyProfile
        {
            get { return _lbl_MyProfile; }
            set
            {
                _lbl_MyProfile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_MyProfile)));
            }
        }
        public string lbl_Preferences
        {
            get { return _lbl_Preferences; }
            set
            {
                _lbl_Preferences = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Preferences)));
            }
        }
        public string lbl_LogOut
        {
            get { return _lbl_LogOut; }
            set
            {
                _lbl_LogOut = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_LogOut)));
            }
        }
        public string lbl_AreYouSure
        {
            get { return _lbl_AreYouSure; }
            set
            {
                _lbl_AreYouSure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_AreYouSure)));
            }
        }
        public string lbl_Yes
        {
            get { return _lbl_Yes; }
            set
            {
                _lbl_Yes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_Yes)));
            }
        }
        public string lbl_No
        {
            get { return _lbl_No; }
            set
            {
                _lbl_No = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(lbl_No)));
            }
        }

        // Texts
        public string txt_LogIn
        {
            get { return _txt_LogIn; }
            set
            {
                _txt_LogIn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(txt_LogIn)));
            }
        }
        public string txt_ConfirmLogOut
        {
            get { return _txt_ConfirmLogOut; }
            set
            {
                _txt_ConfirmLogOut = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(txt_ConfirmLogOut)));
            }
        }

        // Colors
        public string cl_MainBrand
        {
            get { return _cl_MainBrand; }
            set
            {
                _cl_MainBrand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_MainBrand)));
            }
        }
        public string cl_BackGround
        {
            get { return _cl_BackGround; }
            set
            {
                _cl_BackGround = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_BackGround)));
            }
        }
        public string cl_TextOnBG
        {
            get { return _cl_TextOnBG; }
            set
            {
                _cl_TextOnBG = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_TextOnBG)));
            }
        }
        public string cl_TextOnElse
        {
            get { return _cl_TextOnElse; }
            set
            {
                _cl_TextOnElse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_TextOnElse)));
            }
        }
        public string cl_TextError
        {
            get { return _cl_TextError; }
            set
            {
                _cl_TextError = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_TextError)));
            }
        }
        public string cl_MainGray
        {
            get { return _cl_MainGray; }
            set
            {
                _cl_MainGray = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cl_MainGray)));
            }
        }

        // Images
        public string img_Logo
        {
            get { return _img_Logo; }
            set
            {
                _img_Logo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(img_Logo)));
            }
        }
        #endregion

        public ResourceCollection()
        {
            LoadResources();
        }

        public void LoadResources()
        {
            // Labels
            lbl_LogIn = RsrcManager.GetText("lbl_LogIn");
            lbl_Register = RsrcManager.GetText("lbl_Register");
            lbl_eMailPlaceHolder = RsrcManager.GetText("lbl_eMailPlaceHolder");
            lbl_PwdPlaceHolder = RsrcManager.GetText("lbl_PwdPlaceHolder");
            lbl_RegisterEmailField = RsrcManager.GetText("lbl_RegisterEmailField");
            lbl_RegisterPasswordField = RsrcManager.GetText("lbl_RegisterPasswordField");
            lbl_RegisterConfirmPasswordField = RsrcManager.GetText("lbl_RegisterConfirmPasswordField");
            lbl_RegisterConfirmEmailField = RsrcManager.GetText("lbl_RegisterConfirmEmailField");
            lbl_Next = RsrcManager.GetText("lbl_Next");
            lbl_RegisterFirstName = RsrcManager.GetText("lbl_RegisterFirstName");
            lbl_RegisterLastName = RsrcManager.GetText("lbl_RegisterLastName");
            lbl_FirstNamePlaceHolder = RsrcManager.GetText("lbl_FirstNamePlaceHolder");
            lbl_LastNamePlaceHolder = RsrcManager.GetText("lbl_LastNamePlaceHolder");
            lbl_BirthdateField = RsrcManager.GetText("lbl_BirthdateField");
            lbl_UserData = RsrcManager.GetText("lbl_UserData");
            lbl_Menu_1 = RsrcManager.GetText(Preferences.Get("lbl_Menu_1", "lbl_Feed"));
            lbl_Menu_2 = RsrcManager.GetText(Preferences.Get("lbl_Menu_2", "lbl_Discovery"));
            lbl_Menu_3 = RsrcManager.GetText(Preferences.Get("lbl_Menu_3", "lbl_MyPraises"));
            lbl_MyProfile = RsrcManager.GetText("lbl_MyProfile");
            lbl_Preferences = RsrcManager.GetText("lbl_Preferences");
            lbl_LogOut = RsrcManager.GetText("lbl_LogOut");
            lbl_AreYouSure = RsrcManager.GetText("lbl_AreYouSure");
            lbl_Yes = RsrcManager.GetText("lbl_Yes");
            lbl_No = RsrcManager.GetText("lbl_No");
            // Texts
            txt_LogIn = RsrcManager.GetText("txt_LogIn");
            txt_ConfirmLogOut = RsrcManager.GetText("txt_ConfirmLogOut");
            // Colors
            cl_MainBrand = RsrcManager.GetColorHexCode("cl_MainBrand");
            cl_BackGround = RsrcManager.GetColorHexCode("cl_BackGround");
            cl_TextOnBG = RsrcManager.GetColorHexCode("cl_TextOnBG");
            cl_TextOnElse = RsrcManager.GetColorHexCode("cl_TextOnElse");
            cl_TextError = RsrcManager.GetColorHexCode("cl_TextError");
            cl_MainGray = RsrcManager.GetColorHexCode("cl_MainGray");
            // Images
            img_Logo = RsrcManager.GetImagePath("img_Logo");
        }
    }
}
