namespace Spots;

public partial class CP_AppPreferences : ContentPage
{
	bool _LanguageChanged, _ThemeChanged = false;
    private string[] _LanguageCodes = new string[] { "en", "es" };
    private string[] _ThemeCodes = new string[] { "light", "dark" };
    private Dictionary<string, string> _LanguageByCode = new Dictionary<string, string>();
    private Dictionary<string, string> _ThemeByCode = new Dictionary<string, string>();
    public CP_AppPreferences()
	{
		InitializeComponent();

        Resources = Application.Current?.Resources;
        LoadPickerElements();

        _PickerLanguage.SelectedIndexChanged += _PickerLanguage_SelectedIndexChanged;
        _PickerTheme.SelectedIndexChanged += _PickerTheme_SelectedIndexChanged;
	}

    private void _PickerTheme_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _ThemeChanged = true;
    }

    private void _PickerLanguage_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _LanguageChanged = true;
    }

    private void LoadPickerElements()
    {
        // Languages
		string[] languages = ResourceManagement.GetStringResources(Resources, new string[] { "lbl_LangEnglish", "lbl_LangSpanish" });
        for (int i = 0; i < languages.Length; i++)
		{
            _PickerLanguage.Items.Add(languages[i]);
            _LanguageByCode[_LanguageCodes[i]] = languages[i];
        }
        _PickerLanguage.SelectedIndex = _PickerLanguage.Items.IndexOf(_LanguageByCode[Preferences.Get("Language", "en")]);

        // Themes
        string[] themes = ResourceManagement.GetStringResources(Resources, new string[] { "lbl_ThemeLight", "lbl_ThemeDark" });
        for (int i = 0; i < themes.Length; i++)
        {
            _PickerTheme.Items.Add(themes[i]);
            _ThemeByCode[_ThemeCodes[i]] = themes[i];
        }
        _PickerTheme.SelectedIndex = _PickerTheme.Items.IndexOf(_ThemeByCode[Preferences.Get("Theme", "light")]);
    }

    public void ApplyOnClick(object sender, EventArgs e)
    {
        if(_ThemeChanged)
        {
            Preferences.Set("Theme", _PickerTheme.Items[_PickerTheme.SelectedIndex]);
            _ThemeChanged = false;
        }
        if (_LanguageChanged)
        {
            Preferences.Set("Language", _PickerLanguage.Items[_PickerLanguage.SelectedIndex]);
            _LanguageChanged = false;
        }
        //TODO: Apply changes on resources
        _LanguageChanged = false;
        Navigation.PopAsync();
    }

    public void CancelOnClick(object sender, EventArgs e)
    {
        if(_ThemeChanged || _LanguageChanged)
        {
            //TODO: Ask if user wants to discard changes.
        }
        Navigation.PopAsync();
    }
}