using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spots;

public class FeedContext<T> : INotifyPropertyChanged
{
    private readonly object _syncRoot = new object();

    public event PropertyChangedEventHandler? PropertyChanged;
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
                LastItemFetched = element;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
    }

    public void RefreshFeed(List<T> elements)
    {
        lock(_syncRoot)
        {
            ItemSource.Clear();
            LastItemFetched = default;

            foreach (T element in elements)
            {
                ItemSource.Add(element);
                LastItemFetched = element;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
    }
}
