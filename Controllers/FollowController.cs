using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FollowController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int comicId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để theo dõi truyện." });
            }

            try
            {
                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ComicId == comicId);

                if (existingFollow != null)
                {
                    // Bỏ theo dõi
                    _context.Follows.Remove(existingFollow);
                    await _context.SaveChangesAsync();
                      return Json(new { 
                        success = true, 
                        isFollowing = false, 
                        message = "Đã bỏ theo dõi truyện." 
                    });
                }
                else
                {
                    // Theo dõi mới
                    var follow = new Follow
                    {
                        UserId = userId,
                        ComicId = comicId,
                        FollowedAt = DateTime.Now
                    };
                    
                    _context.Follows.Add(follow);
                    await _context.SaveChangesAsync();
                      return Json(new { 
                        success = true, 
                        isFollowing = true, 
                        message = "Đã theo dõi truyện thành công." 
                    });}
            }
            catch (Exception)
            {
                return Json(new { 
                    success = false, 
                    message = "Có lỗi xảy ra. Vui lòng thử lại." 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckStatus(int comicId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { isFollowing = false });
            }

            var isFollowing = await _context.Follows
                .AnyAsync(f => f.UserId == userId && f.ComicId == comicId);

            return Json(new { isFollowing });
        }

        [HttpGet]
        public async Task<IActionResult> MyFollows()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }            var follows = await _context.Follows
                .Include(f => f.Comic)
                    .ThenInclude(c => c!.Chapters)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.FollowedAt)
                .ToListAsync();

            ViewBag.Title = "Truyện đang theo dõi";
            return View(follows);
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowCount(int comicId)
        {
            var count = await _context.Follows
                .CountAsync(f => f.ComicId == comicId);

            return Json(new { count });
        }

        [HttpPost]
        public async Task<IActionResult> FollowMultiple(int[] comicIds)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            try
            {
                var existingFollows = await _context.Follows
                    .Where(f => f.UserId == userId && comicIds.Contains(f.ComicId))
                    .Select(f => f.ComicId)
                    .ToListAsync();

                var newFollows = comicIds
                    .Where(id => !existingFollows.Contains(id))
                    .Select(id => new Follow
                    {
                        UserId = userId,
                        ComicId = id,
                        FollowedAt = DateTime.Now
                    })
                    .ToList();

                if (newFollows.Any())
                {
                    _context.Follows.AddRange(newFollows);
                    await _context.SaveChangesAsync();
                }                return Json(new { 
                    success = true, 
                    added = newFollows.Count, 
                    message = $"Đã theo dõi {newFollows.Count} truyện mới." 
                });
            }
            catch (Exception)
            {
                return Json(new { 
                    success = false, 
                    message = "Có lỗi xảy ra. Vui lòng thử lại." 
                });
            }
        }
    }
}
