using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class Comic
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string CoverImageUrl { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Đang tiến hành"; // Đang tiến hành, Hoàn thành, Tạm ngưng
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        
        public int ViewCount { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
        
        public ICollection<ComicGenre> ComicGenres { get; set; } = new List<ComicGenre>();
    }
}
