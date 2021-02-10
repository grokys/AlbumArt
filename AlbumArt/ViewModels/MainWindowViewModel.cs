﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        
        public MainWindowViewModel()
        {
            _search = new iTunesSearchManager();
            SearchResults = new ObservableCollection<AlbumViewModel>();
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch);
        }

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ObservableCollection<AlbumViewModel> SearchResults { get; }

        async void DoSearch(string? s)
        {
            SearchResults.Clear();

            if (string.IsNullOrEmpty(s)) return;
            
            var result = await _search.GetAlbumsAsync(_searchText);

            foreach (var album in result.Albums)
            {
                var vm = new AlbumViewModel(album.ArtistName, album.CollectionName, album.ArtworkUrl100);
                SearchResults.Add(vm);                    
            }

            foreach (var vm in SearchResults)
            {
                await vm.LoadCover();
            }
        }
    }
}