using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = null!;
    }
}
