namespace WebTruyenHay.Models
{
    public class ComicGenre
    {
        public int ComicId { get; set; }
        public int GenreId { get; set; }
        
        // Navigation properties
        public Comic Comic { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}
