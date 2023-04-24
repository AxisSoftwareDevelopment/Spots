using Spots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
//using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Spots.Models.DisplayManager;

namespace Spots.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwLogin : ContentPage
    {
        private const uint MILISECONDS_STARTUP_ANIMATION = 600;

        public vwLogin()
        {
            InitializeComponent();

            BindingContext = RsrcManager.resourceCollection;

            #region Animations
            RunAnimationsAsync();
            #endregion
        }

        private async void BtnLogInOnClick(object sender, EventArgs e)
        {
            SwitchLockViewState();

            string email = _entryEmail.Text;
            string password = _entryPassword.Text;
            string errorMsg;

            if (CredentialsAreValid(email, password, out errorMsg))
            {
                HideErrorSection();
                // Look for user in database
                try
                {
                    User user = await DatabaseManager.LogInAsync(email, password);

                    if (user.userID != null && user.userID.Length > 0)
                    {
                        CurrentSession.StartSession(user);
                        Application.Current.MainPage = new NavigationPage(new HomePage.vwHomePage());
                    }
                    else
                    {
                        DisplayErrorSection("txt_LogInError_WrongCredentials");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message.ToString(), "OK");
                }
            }
            else
            {
                DisplayErrorSection(errorMsg);
            }

            SwitchLockViewState();
        }

        private void BtnRegisterOnClick(object sender, EventArgs e)
        {
            Navigation.PushAsync(new vwRegister_UserData());
        }

        private void SwitchLockViewState()
        {
            _btnLogIn.IsEnabled = ! _btnLogIn.IsEnabled;
            _btnRegister.IsEnabled = ! _btnRegister.IsEnabled;
        }

        private async void RunAnimationsAsync()
        {
            // Hide elements
            await Task.WhenAll(
                AnimateTranslateTo(_imgLogo, offset_X: 0, offset_Y: 900),
                AnimateTranslateTo(_frameEntries, offset_X: 0, offset_Y: 700)
            );

            // Bring elements up
            await Task.WhenAll(
                AnimateTranslateTo(_imgLogo, offset_X: 0, offset_Y: - 900, MILISECONDS_STARTUP_ANIMATION),
                AnimateTranslateTo(_frameEntries, offset_X: 0, offset_Y: - 700, MILISECONDS_STARTUP_ANIMATION)
            );

            SwitchLockViewState();
        }

        private async Task<bool> AnimateTranslateTo(VisualElement obj, double offset_X, double offset_Y, uint length = 0)
        {
            return await obj.TranslateTo(obj.TranslationX + offset_X, obj.TranslationY + offset_Y, length);
        }

        private void DisplayErrorSection(string errorID)
        {
            _lblSignInError.Text = RsrcManager.GetText(errorID);
            _lblSignInError.IsVisible = true;
        }

        private void HideErrorSection()
        {
            _lblSignInError.IsVisible = false;
        }

        private bool CredentialsAreValid(string email, string password, out string errorMessage)
        {
            bool credentialsAreValid = true;
            errorMessage = "";
            if (email.Length == 0 || password.Length == 0)
            {
                credentialsAreValid = false;
                errorMessage = "txt_LogInError_EmptyEntry";
            }
            return credentialsAreValid;
        }
    }
}