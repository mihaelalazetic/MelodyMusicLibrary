using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MelodyMusicLibrary.ViewModels
{
    public class SongCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Display(Name = "Album")]
        public int AlbumId { get; set; }

        public List<int> SelectedArtists { get; set; }
        public List<int> SelectedGenres { get; set; }

        public SelectList ArtistList { get; set; }
        public SelectList AlbumList { get; set; }
        public SelectList GenreList { get; set; }
    }
}
