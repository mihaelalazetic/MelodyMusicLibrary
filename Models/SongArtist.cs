namespace MelodyMusicLibrary.Models
{
    public class SongArtist
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public Song? Song { get; set; }

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }
    }
}
