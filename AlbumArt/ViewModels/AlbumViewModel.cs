using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class AlbumViewModel : ViewModelBase
    {
        private readonly string _coverUrl;
        private Bitmap? _cover;
        
        public AlbumViewModel(string artist, string title, string coverUrl)
        {
            Artist = artist;
            Title = title;
            _coverUrl = coverUrl;
        }
        
        public string Artist { get; }
        public string Title { get; }

        public Bitmap? Cover
        {
            get => _cover;
            private set => this.RaiseAndSetIfChanged(ref _cover, value);
        }

        public async Task LoadCover()
        {
            using var client = new HttpClient();
            var data = await client.GetByteArrayAsync(_coverUrl);

            await using var s = new MemoryStream(data);
            Cover = Bitmap.DecodeToWidth(s, 300);
        }
    }
}