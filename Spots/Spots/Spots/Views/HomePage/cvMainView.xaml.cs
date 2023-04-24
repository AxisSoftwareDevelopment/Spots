using Spots.Models;
using Spots.Models.DisplayManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views.HomePage
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class cvMainView : ContentView
	{
        public cvMainView ()
		{
			InitializeComponent();

			BindingContext = RsrcManager.resourceCollection;

			_TabView1.Content = new cvFeed();
			_TabView2.Content = new cvDiscover();
			_TabView3.Content = new cvMyPraises();
		}
	}
}