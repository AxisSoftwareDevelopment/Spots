using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Spots_v01.Librerias
{
    public class Preferencias
    {
        public int theme { get; set; }
        public string lenguaje { get; set; }

        public Preferencias() 
        {
            // Load Defaults
            if(!Preferences.ContainsKey("loaded"))
            {
                LoadDefaults();
            }
        }

        public void LoadDefaults() 
        {
            // Set Defaults
            theme = 0;
            lenguaje = "en";

            // Apply Settings
            Apply();

            // Set 'loaded' Setting
            Preferences.Set("loaded", true);
        }

        public void Apply()
        {
            Preferences.Set("iTheme", theme);
            Preferences.Set("strLenguaje", lenguaje);
        }
    }
}
