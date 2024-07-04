using System.ComponentModel.DataAnnotations;

namespace MelodyMusicLibrary.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than zero.")]
        public int Duration { get; set; } // in seconds

        public int AlbumId { get; set; }
        public Album? Album { get; set; }
        public ICollection<SongArtist> SongArtists { get; set; }
        public ICollection<SongGenre> SongGenres { get; set; }
    }
}
