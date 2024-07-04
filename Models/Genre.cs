using System.ComponentModel.DataAnnotations.Schema;

namespace MelodyMusicLibrary.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<AlbumGenre> AlbumGenres { get; set; }
        public ICollection<SongGenre> SongGenres { get; set; }
    }
}
