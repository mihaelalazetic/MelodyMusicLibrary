using System.ComponentModel.DataAnnotations;

namespace MelodyMusicLibrary.ViewModels
{
    public class GenreViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public IFormFile ImageFile { get; set; } 
    }

}
