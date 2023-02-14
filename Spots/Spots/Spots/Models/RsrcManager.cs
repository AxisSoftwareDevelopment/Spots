using Spots.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using Xamarin.Essentials;

namespace Spots.Models
{
    static class RsrcManager
    {
        private static ResourceManager imageManager;
        private static Dictionary<string, ResourceManager> Dict;
        private static string Language { get; set; }
        private static int Theme { get; set; }

        /// <summary>
        /// Resource manager class. It allows to get string values from .resx files.
        /// </summary>
        static RsrcManager()
        {
            Dict = new Dictionary<string, ResourceManager>()
            {
                // Theme Resx
                { "0", Colors.ResourceManager },
                { "1", Colors_dark.ResourceManager },
                // Lang Resx
                { "en", AppResources.ResourceManager },
                { "es", AppResources_es.ResourceManager }
            };
            imageManager = AppImages.ResourceManager;

            UpdateLangaje();
            UpdateTheme();
        }

        public static string GetText(string _id)
        {
            return Dict[Language].GetString(_id);
        }

        public static string GetColorHexCode(string _id)
        {
            return Dict[Theme.ToString()].GetString(_id);
        }

        public static string GetImagePath(string _id)
        {
            return imageManager.GetString(_id);
        }

        public static void UpdateLangaje()
        {
            Language = Preferences.Get("strLenguaje", "en");
        }

        private static void UpdateTheme()
        {
            Theme = Preferences.Get("iTheme", 0);
        }
    }
}
