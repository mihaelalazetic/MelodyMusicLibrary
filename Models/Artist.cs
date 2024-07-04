namespace MelodyMusicLibrary.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<AlbumArtist> AlbumArtists { get; set; }
        public ICollection<SongArtist> SongArtists { get; set; }
    }
}
