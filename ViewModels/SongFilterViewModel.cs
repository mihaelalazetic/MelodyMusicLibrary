using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MelodyMusicLibrary.ViewModels
{
    public class SongFilterViewModel
    {
        public string Title { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }

        // Optionally, you can include SelectList properties for dropdowns if needed
        public SelectList Albums { get; set; }
        public SelectList Artists { get; set; }
    }
}
