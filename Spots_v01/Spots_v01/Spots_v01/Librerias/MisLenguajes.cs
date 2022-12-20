using System;
using System.Collections.Generic;
using System.Text;

namespace Spots_v01.Librerias
{
    public class MisLenguajes
    {
        public string Idioma { get; set; }
        public string CI { get; set; }

        public MisLenguajes(string idioma, string cI)
        {
            Idioma = idioma;
            CI = cI;
        }
    }
}
