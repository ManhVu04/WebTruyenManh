using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public int ChapterNumber { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        
        public int ViewCount { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        // Foreign key
        public int ComicId { get; set; }
        
        // Navigation properties
        public Comic Comic { get; set; } = null!;
        
        public ICollection<ChapterImage> ChapterImages { get; set; } = new List<ChapterImage>();
    }
}
