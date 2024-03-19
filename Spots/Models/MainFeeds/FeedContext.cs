using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spots;

public class FeedContext<T> : BindableObject, INotifyPropertyChanged
{
    private readonly object _syncRoot = new object();

    new public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<T> ItemSource { get; }
    public T? LastItemFetched { get; private set; }

    public FeedContext()
    {
        ItemSource = [];
    }

    public void AddElements(List<T> elements)
    {
        lock(_syncRoot)
        {
            foreach (T element in elements)
            {
                ItemSource.Add(element);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
    }

    public void RefreshFeed(List<T> elements)
    {
        lock(_syncRoot)
        {
            ItemSource.Clear();
            foreach (T element in elements)
            {
                ItemSource.Add(element);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
    }
}
