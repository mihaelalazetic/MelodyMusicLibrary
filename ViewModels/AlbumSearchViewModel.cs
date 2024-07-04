using MelodyMusicLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MelodyMusicLibrary.ViewModels
{
    public class AlbumSearchViewModel
    {
        public IList<Album> Albums { get; set; }
        public SelectList Genres { get; set; }
        public string Genre { get; set; }

        public SelectList Artists { get; set; }
        public string Artist { get; set; }

        public string Title { get; set; }
    }
}
