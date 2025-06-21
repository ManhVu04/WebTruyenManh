using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebTruyenHay.Models
{
    public class ReadingHistory
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int ComicId { get; set; }
        
        [Required]
        public int ChapterId { get; set; }
        
        public int ChapterNumber { get; set; }
        
        public DateTime LastReadDate { get; set; } = DateTime.Now;
        
        // Navigation properties
        public IdentityUser User { get; set; } = null!;
        public Comic Comic { get; set; } = null!;
        public Chapter Chapter { get; set; } = null!;
    }
}
