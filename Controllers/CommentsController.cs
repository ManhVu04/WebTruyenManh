using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int chapterId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest();
            }
            var userName = User.Identity?.Name ?? "Ẩn danh";
            var userId = User.Identity?.IsAuthenticated == true ? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value : null;
            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                UserId = userId,
                UserName = userName,
                ChapterId = chapterId
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List(int chapterId)
        {
            var comments = await _context.Comments
                .Where(c => c.ChapterId == chapterId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return PartialView("_CommentsList", comments);
        }
    }
}
