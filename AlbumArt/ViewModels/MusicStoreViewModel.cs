using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using iTunesSearch.Library;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class MusicStoreViewModel : ViewModelBase
    {
        private readonly iTunesSearchManager _search;
        private string? _searchText;
        private bool _isBusy;
        private HttpClient _client;
        private CancellationTokenSource _cancellationTokenSource;

        public MusicStoreViewModel()
        {
            _client = new HttpClient();
            _search = new iTunesSearchManager();

            this.WhenAnyValue(x => x.SearchText)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);
            
            Ok = ReactiveCommand.Create(() => { });
        }
        
        public string? Text { get; set; }
        
        public ReactiveCommand<Unit, Unit> Ok { get; }

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        private async void DoSearch(string s)
        {
            IsBusy = true;
            SearchResults.Clear();
            
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();

            var result = await _search.GetAlbumsAsync(_searchText);

            foreach (var album in result.Albums)
            {
                var vm = new AlbumViewModel(album.ArtistName, album.CollectionName, album.ArtworkUrl100.Replace("100x100bb", "600x600bb"));
                
                SearchResults.Add(vm);
            }
            
            LoadCovers(_cancellationTokenSource.Token);

            IsBusy = false;
        }

        private async void LoadCovers(CancellationToken cancellationToken)
        {
            foreach (var album in SearchResults.ToList())
            {
                await album.LoadCover(_client);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}