using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Text;
using iTunesSearch.Library.Models;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _collectionEmpty;

        public MainWindowViewModel()
        {
            Albums = new ObservableCollection<AlbumViewModel>();

            this.WhenAnyValue(x => x.Albums.Count)
                .Subscribe(x => CollectionEmpty = x == 0);

            RxApp.MainThreadScheduler.Schedule(LoadAlbums);
        }

        private async void LoadAlbums()
        {
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