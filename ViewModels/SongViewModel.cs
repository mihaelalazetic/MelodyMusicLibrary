using MelodyMusicLibrary.Models;

namespace MelodyMusicLibrary.ViewModels
{
    public class SongViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string Minutes { get; set; } // Calculated property for duration in minutes
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; } // Displaying album title

        public string ArtistNames { get; set; } // Concatenated list of artist names

        public string AlbumCoverUrl { get; set; } // Cover image URL of the album

        public SongViewModel(Song song)
        {
            Id = song.Id;
            Title = song.Title;
            Duration = song.Duration;
            Minutes = $"{(song.Duration / 60):D2}:{(song.Duration % 60):D2}";

            if (song.Album != null)
            {
                AlbumTitle = song.Album.Title;
                AlbumCoverUrl = song.Album.CoverUrl;
                AlbumId = song.Album.Id;
            }

            // Concatenate artist names into a single string
            if (song.SongArtists != null && song.SongArtists.Count > 0)
            {
                ArtistNames = string.Join("</br> ", song.SongArtists.Select(sa => sa.Artist.Name));
            }
        }
    }
}
