using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int ComicId { get; set; }
        
        public DateTime FollowedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Comic? Comic { get; set; }
    }
}
