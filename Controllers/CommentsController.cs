using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int chapterId, string content)
        {
            Console.WriteLine($"Comments/Add called with chapterId: {chapterId}, content: {content}");
            Console.WriteLine($"User authenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User name: {User.Identity?.Name}");
            
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Content is empty or whitespace");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Nội dung bình luận không được để trống." });
                }
                return BadRequest();
            }

            try
            {
                var userName = User.Identity?.Name ?? "Ẩn danh";
                var userId = User.Identity?.IsAuthenticated == true ? 
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value : 
                    null;
                
                Console.WriteLine($"Creating comment with userId: {userId}, userName: {userName}");
                
                var comment = new Comment
                {
                    Content = content,
                    CreatedAt = DateTime.Now,
                    UserId = userId,
                    UserName = userName,
                    ChapterId = chapterId
                };
                
                _context.Comments.Add(comment);
                var saveResult = await _context.SaveChangesAsync();
                
                Console.WriteLine($"SaveChanges result: {saveResult} rows affected");

                // Check if this is an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    Console.WriteLine("Returning JSON success response");
                    return Json(new { success = true, message = "Bình luận đã được thêm thành công!" });
                }                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Comments/Add: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Log error if needed
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi thêm bình luận: " + ex.Message });
                }
                return BadRequest();
            }
        }        [HttpGet]
        public async Task<IActionResult> List(int chapterId)
        {
            Console.WriteLine($"Comments/List called with chapterId: {chapterId}");
            
            try
            {
                var comments = await _context.Comments
                    .Where(c => c.ChapterId == chapterId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
                
                Console.WriteLine($"Found {comments.Count} comments for chapter {chapterId}");
                
                foreach (var comment in comments)
                {
                    Console.WriteLine($"Comment: {comment.Id}, User: {comment.UserName}, Content: {comment.Content}");
                }
                
                return PartialView("_CommentsList", comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Comments/List: {ex.Message}");
                return PartialView("_CommentsList", new List<Comment>());
            }
        }
    }
}
