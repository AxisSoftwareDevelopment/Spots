using Spots_v01.Recursos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots_v01.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwLogin : ContentPage
    {
        Librerias.Traductor traductor;
        public vwLogin()
        {
            InitializeComponent();
            
            traductor = new Librerias.Traductor();
        }
    }
}