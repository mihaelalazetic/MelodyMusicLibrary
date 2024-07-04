using Microsoft.AspNetCore.Mvc.Rendering;

namespace MelodyMusicLibrary.ViewModels
{
    public class AlbumCreateViewModel
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverUrl { get; set; }

        // Properties for artists and genres
        public IEnumerable<int>? SelectedArtist { get; set; }
        public IEnumerable<SelectListItem>? ArtistList { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenresList { get; set; }

    }
}
