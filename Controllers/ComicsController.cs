using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebTruyenHay.Data;
using WebTruyenHay.Models;
using WebTruyenHay.Models.ViewModels;
using WebTruyenHay.Services;

namespace WebTruyenHay.Controllers
{    public class ComicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IReadingHistoryService _readingHistoryService;

        public ComicsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IReadingHistoryService readingHistoryService)
        {
            _context = context;
            _userManager = userManager;
            _readingHistoryService = readingHistoryService;
        }

        // GET: Comics
        public async Task<IActionResult> Index(string? search, int? genreId, int page = 1)
        {
            var comics = _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters)
                .Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                comics = comics.Where(c => c.Title.Contains(search) || c.Author.Contains(search));
                ViewBag.Search = search;
            }

            if (genreId.HasValue)
            {
                comics = comics.Where(c => c.ComicGenres.Any(cg => cg.GenreId == genreId.Value));
                ViewBag.SelectedGenreId = genreId.Value;
            }

            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = genres;

            var pageSize = 12;
            var totalComics = await comics.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalComics / pageSize);

            var comicsToShow = await comics
                .OrderByDescending(c => c.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(comicsToShow);
        }        // GET: Comics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters.Where(ch => ch.IsActive))
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (comic == null)
            {
                return NotFound();
            }

            // Increase view count
            comic.ViewCount++;
            await _context.SaveChangesAsync();            // Kiểm tra trạng thái theo dõi của user hiện tại
            bool isFollowing = false;
            ReadingHistory? readingHistory = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != null)
                {
                    isFollowing = await _context.Follows
                        .AnyAsync(f => f.UserId == userId && f.ComicId == id);
                    
                    // Get reading history for this comic
                    readingHistory = await _readingHistoryService.GetReadingHistoryAsync(userId, id.Value);
                }
            }

            var viewModel = new ComicDetailsViewModel
            {
                Comic = comic,
                IsFollowing = isFollowing,
                ReadingHistory = readingHistory
            };return View(viewModel);
        }

        // GET: Comics/Read/5?chapter=1 hoặc Comics/Read?comicId=5&chapterNumber=1
        public async Task<IActionResult> Read(int? id, int chapter = 1, int? comicId = null, int? chapterNumber = null)
        {
            // Hỗ trợ cả 2 định dạng tham số
            var targetComicId = id ?? comicId;
            var targetChapter = chapterNumber ?? chapter;
            
            if (targetComicId == null)
            {
                return NotFound();
            }            var comic = await _context.Comics
                .Include(c => c.Chapters.Where(ch => ch.IsActive))
                .ThenInclude(ch => ch.ChapterImages)
                .FirstOrDefaultAsync(c => c.Id == targetComicId && c.IsActive);

            if (comic == null)
            {
                return NotFound();
            }            var currentChapter = comic.Chapters
                .FirstOrDefault(ch => ch.ChapterNumber == targetChapter);

            if (currentChapter == null)
            {
                return NotFound();
            }

            // Increase chapter view count
            currentChapter.ViewCount++;
            await _context.SaveChangesAsync();

            // Save reading history for logged-in users
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    await _readingHistoryService.UpdateReadingHistoryAsync(userId, targetComicId.Value, currentChapter.Id, targetChapter);
                }
            }

            ViewBag.Comic = comic;
            ViewBag.AllChapters = comic.Chapters.OrderBy(ch => ch.ChapterNumber).ToList();
            ViewBag.CurrentChapterNumber = targetChapter;
            ViewBag.Comments = await _context.Comments
                .Where(c => c.ChapterId == currentChapter.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(currentChapter);
        }
    }
}
