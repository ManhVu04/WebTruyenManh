using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class ChapterImage
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int PageNumber { get; set; }
        
        // Foreign key
        public int ChapterId { get; set; }
        
        // Navigation property
        public Chapter Chapter { get; set; } = null!;
    }
}
