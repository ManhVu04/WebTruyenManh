using System.ComponentModel.DataAnnotations;

namespace WebTruyenHay.Models.ViewModels
{
    public class ComicViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; } = string.Empty;
          [StringLength(500, ErrorMessage = "Tên tác giả không được vượt quá 500 ký tự")]
        public string Author { get; set; } = string.Empty;
        
        public string? CoverImageUrl { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Đang tiến hành";
        
        public List<int> SelectedGenreIds { get; set; } = new List<int>();
        
        // For file upload
        public IFormFile? CoverImageFile { get; set; }
    }
}
