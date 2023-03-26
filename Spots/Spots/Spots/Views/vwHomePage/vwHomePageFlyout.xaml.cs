using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Spots.Views.vwHomePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vwHomePageFlyout : ContentPage
    {
        public ListView ListView;

        public vwHomePageFlyout()
        {
            InitializeComponent();

            BindingContext = new vwHomePageFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        private class vwHomePageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<vwHomePageFlyoutMenuItem> MenuItems { get; set; }

            public vwHomePageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<vwHomePageFlyoutMenuItem>(new[]
                {
                    new vwHomePageFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new vwHomePageFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new vwHomePageFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new vwHomePageFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new vwHomePageFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}