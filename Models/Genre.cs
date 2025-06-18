using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class Genre
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;
        
        // Navigation property
        public ICollection<ComicGenre> ComicGenres { get; set; } = new List<ComicGenre>();
    }
}
