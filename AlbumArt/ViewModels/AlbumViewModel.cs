using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace AlbumArt.ViewModels
{
    public class AlbumViewModel : ViewModelBase
    {
        private readonly string _coverUrl;
        private Bitmap? _cover;

        public static AlbumViewModel FromAlbumData(AlbumData data)
        {
            return new AlbumViewModel(data.Artist!, data.Title!, String.Empty);
        }
        
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

        private string CachePath => $"./Cache/{Artist} - {Title}";

        public async Task LoadCover(HttpClient? client)
        {
            Stream? imageStream = null;

            if (File.Exists(CachePath + ".bmp"))
            {
                imageStream = File.OpenRead(CachePath + ".bmp");
            }
            else if(client != null)
            {
                var data = await client.GetByteArrayAsync(_coverUrl);

                imageStream = new MemoryStream(data);
            }

            if (imageStream != null)
            {
                await using (imageStream)
                {
                    Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
                }
            }
        }

        public static async Task<IEnumerable<AlbumViewModel>> LoadCached()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            var results = new List<AlbumViewModel>();

            foreach (var file in Directory.EnumerateFiles("./Cache"))
            {
                if(string.IsNullOrWhiteSpace(new DirectoryInfo(file).Extension))
                {
                    using (var fs = File.OpenRead(file))
                    {
                        results.Add(AlbumViewModel.FromAlbumData((await JsonSerializer.DeserializeAsync<AlbumData>(fs))!));
                    }
                }
            }

            return results;
        }

        public async Task SaveToDiskAsync()
        {
            if (!Directory.Exists("./Cache"))
            {
                Directory.CreateDirectory("./Cache");
            }

            using (var fs = File.OpenWrite(CachePath))
            {
                await AlbumData.SaveToStreamAsync(new AlbumData
                {
                    Artist = Artist,
                    Title = Title
                }, fs);
            }

            if (Cover != null)
            {
                await Task.Run(() =>
                {
                    using (var fs = File.OpenWrite(CachePath + ".bmp"))
                    {
                        Cover.Save(fs);
                    }
                });
            }
        }
    }
}