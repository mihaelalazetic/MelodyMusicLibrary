namespace MelodyMusicLibrary.ViewModels
{
    public class GenreViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile ImageFile { get; set; } // Add this property
    }

}
