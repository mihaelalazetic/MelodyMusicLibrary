using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MelodyMusicLibrary.Models
{
    public class Album
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Cover")]
        public string CoverUrl { get; set; }


        public ICollection<Song> Songs { get; set; } = new List<Song>(); 
        [Display(Name = "Artists")]
        public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>(); 
        [Display(Name = "Genres")]
        public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>(); 


        [NotMapped]
        public int Year
        {
            get
            {
                return ReleaseDate.Year;
            }
        }
    }
}
