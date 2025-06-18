using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models.ViewModels
{
    public class ChapterViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tiêu đề chương không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề chương không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số chương không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số chương phải lớn hơn 0")]
        public int ChapterNumber { get; set; }
        
        public int ComicId { get; set; }
        
        public string ComicTitle { get; set; } = string.Empty;
        
        // For file upload
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
        
        // For displaying existing images
        public List<ChapterImage> ExistingImages { get; set; } = new List<ChapterImage>();
    }
}
