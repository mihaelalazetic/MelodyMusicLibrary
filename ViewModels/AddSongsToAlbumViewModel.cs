using MelodyMusicLibrary.Models;

namespace MelodyMusicLibrary.ViewModels
{
    public class AddSongsToAlbumViewModel
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public List<Song> AvailableSongs { get; set; }
        public List<int> SelectedSongs { get; set; }
    }
}
