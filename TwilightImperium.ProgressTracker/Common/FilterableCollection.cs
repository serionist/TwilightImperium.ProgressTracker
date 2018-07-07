using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TwilightImperium.ProgressTracker.Common
{
    public class FilterableCollection<T>:ViewModel
    {
        private string _searchString;
        private Dispatcher _dispatcher;
        public FilterableCollection(ObservableCollection<T> allItems, Func<T,string, bool> filterFunc, IComparer<T> comparer)
        {
            FilteredItems = new ReadOnlyObservableCollection<T>(_filteredItems = new ObservableCollection<T>());
            AllItems = allItems;
            AllItems.CollectionChanged += (sender, args) => refreshItems(CancellationToken.None);
            FilterFunc = filterFunc;
            Comparer = comparer;
            _dispatcher = Dispatcher.CurrentDispatcher;
            refreshItems(CancellationToken.None);
           
        }

        public ObservableCollection<T> AllItems { get; }
        public ReadOnlyObservableCollection<T> FilteredItems { get; }
        private readonly ObservableCollection<T> _filteredItems = null;
        public Func<T,string,bool> FilterFunc { get; }
        public IComparer<T> Comparer { get; }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                ThisPropChanged();
                searchStringChanged();
            }
        }

        private CancellationTokenSource _searchCancel = null;
        private void searchStringChanged()
        {
            _searchCancel?.Cancel();
            var cancel = new CancellationTokenSource();
            _searchCancel = cancel;
            Task.Run(async() =>
            {
                await Task.Delay(200, cancel.Token);
                refreshItems(cancel.Token);
            }, cancel.Token);
        }

        private void refreshItems(CancellationToken cancelToken)
        {
            List<T> filtered;
            if (string.IsNullOrEmpty(SearchString))
                filtered = AllItems.ToList();
            else
                filtered = AllItems.Where(e => FilterFunc(e, SearchString)).OrderBy(e=>Comparer).ToList();
            cancelToken.ThrowIfCancellationRequested();
            var existingFilteredItems = FilteredItems.ToList();
            cancelToken.ThrowIfCancellationRequested();
            var toDel = existingFilteredItems.Where(e => !filtered.Exists(f => f.Equals(e)));

            _dispatcher.Invoke(() =>
            {
                foreach (var toDelete in toDel)
                {
                    if (cancelToken.IsCancellationRequested)
                        return;
                    _filteredItems.Remove(toDelete);
                }
                for (int i = 0; i < filtered.Count; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                        return;
                    if (_filteredItems.Contains(filtered[i]))
                        _filteredItems.Move(_filteredItems.IndexOf(filtered[i]), i);
                    else _filteredItems.Insert(i, filtered[i]);

                }
            });

        }


    }
}
