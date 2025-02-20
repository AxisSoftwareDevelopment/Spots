namespace eatMeet.ResourceManager;

public static class ThemeManager
{
    public static void ChangeTheme(string theme)
    {
        if (!theme.Equals(Preferences.Get("Theme", "null")))
        {
            Preferences.Set("Theme", theme);

            UpdateColors(theme);
        }
    }

    private static void UpdateColors(string theme) 
    {
        if(Application.Current != null)
        {
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if(dictionary.TryGetValue($"{theme}Primary", out var primary))
                    dictionary["Primary"] = primary;

                if (dictionary.TryGetValue($"{theme}Secondary", out var Secondary))
                    dictionary["Secondary"] = Secondary;

                if (dictionary.TryGetValue($"{theme}Tertiary", out var Tertiary))
                    dictionary["Tertiary"] = Tertiary;
                
                if (dictionary.TryGetValue($"{theme}PrimaryAccent", out var PrimaryAccent))
                    dictionary["PrimaryAccent"] = PrimaryAccent;

                if (dictionary.TryGetValue($"{theme}SecondaryAccent", out var SecondaryAccent))
                    dictionary["SecondaryAccent"] = SecondaryAccent;
            }
        }
    }
}
