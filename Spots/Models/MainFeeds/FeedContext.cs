using System.Collections.ObjectModel;
using System.ComponentModel;

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
        foreach (T element in elements)
        {
            ItemSource.Add(element);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
    }

    public void RefreshFeed(List<T> elements)
    {
        ItemSource.Clear();
        foreach (T element in elements)
        {
            ItemSource.Add(element);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
    }
}
