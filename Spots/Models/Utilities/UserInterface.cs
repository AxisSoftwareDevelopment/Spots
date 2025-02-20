using System.Threading.Tasks;

namespace eatMeet.Utilities;

public static class UserInterface
{
    public static async Task DisplayPopUp_Regular(string title, string message, string button)
    {
        await Application.Current?.Windows[0].Page?.DisplayAlert(title, message, button);
    }

    public static async Task<bool> DisplayPopPup_Choice(string title, string message, string button_True, string button_False)
    {
        if (Application.Current != null)
        {
            return await Application.Current.Windows[0].Page?.DisplayAlert(title, message, button_True, button_False);
        }

        return false;
    }
}
