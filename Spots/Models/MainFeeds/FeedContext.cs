using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spots.Models;

public class FeedContext<T> : INotifyPropertyChanged
{
    private readonly object _syncRoot = new object();

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<T> ItemSource { get; }
    public T? LastItemFetched { get; private set; }
    public Func<T, bool>? RuleToShowItemOnFeed { get; set; } = default;

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
                if(RuleToShowItemOnFeed == null
                    || (RuleToShowItemOnFeed != null && RuleToShowItemOnFeed(element)))
                {
                    ItemSource.Add(element);
                }
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
                if (RuleToShowItemOnFeed == null
                    || (RuleToShowItemOnFeed != null && RuleToShowItemOnFeed(element)))
                {
                    ItemSource.Add(element);
                }
                LastItemFetched = element;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemSource)));
        }
    }
}
