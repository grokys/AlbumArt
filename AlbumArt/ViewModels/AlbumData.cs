using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbumArt.ViewModels
{
    public class AlbumData
    {
        public string? Artist { get; set; }
        
        public string? Title { get; set; }
        
        public static async Task SaveToStreamAsync(AlbumData data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }

        public static async Task<AlbumData> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<AlbumData>(stream))!;
        }
    }
}