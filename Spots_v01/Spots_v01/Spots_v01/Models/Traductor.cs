using Spots_v01.Recursos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
//using Xamarin.CommunityToolkit.Helpers;

namespace Spots_v01.Librerias
{
    public class Traductor
    {
        public static MisLenguajes lenguaje;

        public Traductor()
        {

            ObservableCollection<MisLenguajes> lenguajes = new ObservableCollection<MisLenguajes>()
            {
                new MisLenguajes("Español", "es"),
                new MisLenguajes("English", "en")
            };

            // lenguaje = lenguajes.FirstOrDefault(len => len.CI == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);
            cargarIdioma(lenguaje);
        }

        public void cambiarLenguaje()
        {
            
        }

        public void cargarIdioma(MisLenguajes lenguaje)
        {
            //LocalizationResourceManager.Current.CurrentCulture = CultureInfo.GetCultureInfo(lenguaje.CI);
        }
    }
}
