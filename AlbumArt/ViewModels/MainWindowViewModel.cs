using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _collectionEmpty;

        public MainWindowViewModel()
        {
            Albums = new ObservableCollection<AlbumViewModel>();
            
            // The OpenDialog command is bound to a button/menu item in the UI.
            OpenDialog = ReactiveCommand.CreateFromTask(OpenDialogAsync);

            // The ShowDialog interaction requests the UI to show the dialog.
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            this.WhenAnyValue(x => x.Albums.Count)
                .Subscribe(x => CollectionEmpty = x == 0);

            RxApp.MainThreadScheduler.Schedule(LoadAlbums);
        }
        
        public ReactiveCommand<Unit, Unit> OpenDialog { get; }
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
        
        private async Task OpenDialogAsync()
        {
            var vm = new MusicStoreViewModel();
            
            var result = await ShowDialog.Handle(vm);

            if (result is { } album)
            {
                Albums.Add(album);
            }
        }
        
        private async void LoadCovers()
        {
            foreach (var album in Albums.ToList())
            {
                await album.LoadCover(null);
            }
        }

        private async void LoadAlbums()
        {
            var albums = await AlbumViewModel.LoadCached();

            foreach (var album in albums)
            {
                Albums.Add(album);
            }
            
            LoadCovers();
        }

        public bool CollectionEmpty
        {
            get => _collectionEmpty;
            set => this.RaiseAndSetIfChanged(ref _collectionEmpty, value);
        }

        public MusicStoreViewModel Store { get; } = new MusicStoreViewModel();


        public ObservableCollection<AlbumViewModel> Albums { get; }
    }
}