using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebTruyenHay.Services;

namespace WebTruyenHay.Controllers
{
    [Authorize]
    public class ReadingHistoryController : Controller
    {
        private readonly IReadingHistoryService _readingHistoryService;
        private readonly UserManager<IdentityUser> _userManager;

        public ReadingHistoryController(IReadingHistoryService readingHistoryService, UserManager<IdentityUser> userManager)
        {
            _readingHistoryService = readingHistoryService;
            _userManager = userManager;
        }

        // GET: ReadingHistory
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            const int pageSize = 10;
            var histories = await _readingHistoryService.GetUserReadingHistoriesAsync(userId, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.HasNextPage = histories.Count == pageSize; // Simple pagination check

            return View(histories);
        }

        // POST: ReadingHistory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int comicId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện hành động này." });
            }

            try
            {
                await _readingHistoryService.DeleteReadingHistoryAsync(userId, comicId);
                return Json(new { success = true, message = "Đã xóa khỏi lịch sử đọc." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
