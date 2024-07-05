using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MelodyMusicLibrary.ViewModels
{
    public class SongEditViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public int AlbumId { get; set; }

        public List<SelectListItem> ArtistList { get; set; } = new List<SelectListItem>();// List of artists for dropdown

        [Display(Name = "Select Artist")]
        public int SelectedArtistId { get; set; } // Selected artist ID

    }
}
