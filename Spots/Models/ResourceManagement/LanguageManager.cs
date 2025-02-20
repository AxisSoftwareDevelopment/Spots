namespace eatMeet.ResourceManager;

public static class LanguageManager
{
    public static void ChangeLanguage(string language)
    {
        if (!language.Equals(Preferences.Get("Language", "null")))
        {
            Preferences.Set("Language", language);

            UpdateCurrentLanguage(language);
        }
    }

    private static void UpdateCurrentLanguage(string language)
    {
        ResourceDictionary currentLanguage = new ResourceDictionary { Source = new Uri(@"Resources/Styles/CurrentLanguage.xaml", UriKind.Relative) };
        ResourceDictionary supportedLanguages = new ResourceDictionary { Source = new Uri(@"Resources/Styles/Languages.xaml", UriKind.Relative) };

        foreach (string key in currentLanguage.Keys)
        {
            if (supportedLanguages.TryGetValue($"{language}{key}", out var value))
                currentLanguage[key] = value;
        }
    }
}
