using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using iTunesSearch.Library;
using iTunesSearch.Library.Models;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly iTunesSearchManager _search;
        private string?_searchText;
        private bool _collectionEmpty;
        
        public MainWindowViewModel()
        {
            _search = new iTunesSearchManager();
            Albums = new ObservableCollection<AlbumViewModel>();

            this.WhenAnyValue(x => x.Albums.Count)
                .Subscribe(x => CollectionEmpty = x == 0);

            RxApp.MainThreadScheduler.Schedule(LoadAlbums);
            
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch);
        }

        private async void LoadAlbums()
        {
        }

        public bool CollectionEmpty
        {
            get => _collectionEmpty;
            set => this.RaiseAndSetIfChanged(ref _collectionEmpty, value);
        }

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        async void DoSearch(string? s)
        {
            Albums.Clear();

            if (string.IsNullOrEmpty(s)) return;
            
            var result = await _search.GetAlbumsAsync(_searchText);

            foreach (var album in result.Albums)
            {
                var vm = new AlbumViewModel(album.ArtistName, album.CollectionName, album.ArtworkUrl100);
                _ = vm.LoadCover();
                Albums.Add(vm);                    
            }
        }
    }
}