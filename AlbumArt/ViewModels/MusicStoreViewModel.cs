using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using iTunesSearch.Library;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class MusicStoreViewModel : ViewModelBase
    {
        private readonly iTunesSearchManager _search;
        private string? _searchText;


        public MusicStoreViewModel()
        {
            _search = new iTunesSearchManager();

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch);
        }

        public ObservableCollection<AlbumViewModel> SearchResults { get; }

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        private async void DoSearch(string? s)
        {
            SearchResults.Clear();

            if (string.IsNullOrEmpty(s)) return;

            var result = await _search.GetAlbumsAsync(_searchText);

            foreach (var album in result.Albums)
            {
                var vm = new AlbumViewModel(album.ArtistName, album.CollectionName, album.ArtworkUrl100);
                _ = vm.LoadCover();
                SearchResults.Add(vm);
            }
        }
    }
}