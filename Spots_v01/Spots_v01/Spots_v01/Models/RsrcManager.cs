using Spots_v01.Recursos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Essentials;

namespace Spots_v01.Models
{
    class RsrcManager
    {
        private Dictionary<string, ResourceManager> Dict;
        private string Language;
        private int Theme;

        public RsrcManager()
        {
            Dict = new Dictionary<string, ResourceManager>()
            {
                // Theme Resx
                { "0", Colors.ResourceManager },
                { "1", Colors_Dark.ResourceManager },
                // Lang Resx
                { "en", AppResources.ResourceManager },
                { "es", AppResources_es.ResourceManager },

            };
            UpdateLangaje();
            UpdateTheme();
        }

        public string GetText(string _id)
        {
            return Dict[Language].GetString(_id);
        }

        public string GetColor(string _id)
        {
            return Dict[Theme.ToString()].GetString(_id);
        }

        public void UpdateLangaje()
        {
            Language = Preferences.Get("strLenguaje", "en");
        }

        private void UpdateTheme()
        {
            Theme = Preferences.Get("iTheme", 0);
        }
    }
}
