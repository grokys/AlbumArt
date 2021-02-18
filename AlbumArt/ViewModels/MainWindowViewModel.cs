using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
            // The OpenDialog command is bound to a button/menu item in the UI.
            OpenDialog = ReactiveCommand.CreateFromTask(OpenDialogAsync);

            // The ShowDialog interaction requests the UI to show the dialog.
            ShowDialog = new Interaction<MusicStoreViewModel, string?>();

            this.WhenAnyValue(x => x.Albums.Count)
                .Subscribe(x => CollectionEmpty = x == 0);

            RxApp.MainThreadScheduler.Schedule(LoadAlbums);
        }
        
        public ReactiveCommand<Unit, Unit> OpenDialog { get; }
        public Interaction<MusicStoreViewModel, string?> ShowDialog { get; }
        
        private async Task OpenDialogAsync()
        {
            var vm = new MusicStoreViewModel { Text = "Hello Dialog!" };
            var result = await ShowDialog.Handle(vm);

            if (result is object)
            {
                // Here's the result.
            }
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