using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots;

public static class UserInterface
{
    public static async Task DisplayPopUp(string title, string message, string button)
    {
        if (Application.Current != null && Application.Current.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, button);
        }
    }
}
