using AndroidX.Activity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spots;

public class FeedContext<T> : BindableObject, INotifyPropertyChanged
{
    new public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<T> ItemSource { get; }

    public FeedContext()
    {
        ItemSource = [];
    }

    public void AddElements(List<T> elements)
    {
        //MainThread.BeginInvokeOnMainThread(() =>
        //{
            foreach (T element in elements)
            {
                ItemSource.Add(element);
            }
        //});
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
    }

    public void RefreshFeed(List<T> elements)
    {
        //MainThread.BeginInvokeOnMainThread(() =>
        //{
            ItemSource.Clear();
            foreach (T element in elements)
            {
                ItemSource.Add(element);
            }
        //});
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
    }
}
