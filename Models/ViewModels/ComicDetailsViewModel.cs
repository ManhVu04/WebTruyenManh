namespace WebTruyenHay.Models.ViewModels
{
    public class ComicDetailsViewModel
    {
        public Comic Comic { get; set; } = null!;
        public bool IsFollowing { get; set; }
        public ReadingHistory? ReadingHistory { get; set; }
    }
}
