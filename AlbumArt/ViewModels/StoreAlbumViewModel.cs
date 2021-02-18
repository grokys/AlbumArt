using System.Windows.Input;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class StoreAlbumViewModel : AlbumViewModel
    {
        public StoreAlbumViewModel(MusicStoreViewModel store, string artist, string title, string coverUrl) : base(artist, title, coverUrl)
        {
            BuyCommand = ReactiveCommand.Create(() =>
            {
                store.Buy(this);
            });
        }
        
        public ICommand BuyCommand { get; }
    }
}