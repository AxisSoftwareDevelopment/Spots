using Spots.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Essentials;

namespace Spots.Models.DisplayManager
{
    public static class RsrcManager
    {
        private static ResourceCollection _resourceCollection;
        private static string[] validLanguajes = new string[] { "en", "es" };
        private static int[] validThemes = new int[] { 0, 1 };
        private static ResourceManager imageManager = AppImages.ResourceManager;
        private static Dictionary<string, ResourceManager> themeResources = new Dictionary<string, ResourceManager>()
        {
            { "0", Colors.ResourceManager },
            { "1", Colors_dark.ResourceManager }
        };
        private static Dictionary<string, ResourceManager> textResources = new Dictionary<string, ResourceManager>()
        {
            { "en", AppResources.ResourceManager },
            { "es", AppResources_es.ResourceManager }
        };

        private static string Language
        {
            get
            {
                return Preferences.Get("strLenguaje", "en");
            }
            set
            {
                if (validLanguajes.Contains(value))
                    Preferences.Set("strLenguaje", value);
            }
        }
        private static int Theme
        {
            get
            {
                return Preferences.Get("iTheme", 0);
            }
            set
            {
                if (validThemes.Contains(value))
                    Preferences.Set("iTheme", value);
            }
        }

        public static ResourceCollection resourceCollection
        {
            get
            {
                if (_resourceCollection == null)
                {
                    _resourceCollection = new ResourceCollection();
                }
                return _resourceCollection;
            }
            set
            {
                _resourceCollection = value;
            }
        }

        public static string GetText(string _id)
        {
            return textResources[Language].GetString(_id);
        }

        public static string GetColorHexCode(string _id)
        {
            return themeResources[Theme.ToString()].GetString(_id);
        }

        public static string GetImagePath(string _id)
        {
            return imageManager.GetString(_id);
        }
    }
}
