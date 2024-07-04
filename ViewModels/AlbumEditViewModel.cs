using MelodyMusicLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MelodyMusicLibrary.ViewModels
{
    public class AlbumEditViewModel
    {
        public Album Album { get; set; } = new Album();
        public IEnumerable<int>? SelectedArtist { get; set; }
        public IEnumerable<SelectListItem>? ArtistList { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenresList { get; set; }

    }
}
