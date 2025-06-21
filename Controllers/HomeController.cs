using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;

namespace WebTruyenHay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }        public async Task<IActionResult> Index()
        {
            // Get latest updated comics with recent chapters
            var latestComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters.Where(ch => ch.IsActive).OrderByDescending(ch => ch.UpdatedDate).Take(3))
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.UpdatedDate)
                .Take(12)
                .ToListAsync();            // Get most viewed comics with recent chapters
            var popularComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters.Where(ch => ch.IsActive).OrderByDescending(ch => ch.UpdatedDate).Take(3))
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.ViewCount)
                .Take(6)
                .ToListAsync();

            // Debug logging
            _logger.LogInformation($"Found {popularComics.Count} popular comics");
            foreach (var comic in popularComics)
            {
                _logger.LogInformation($"Comic: {comic.Title}, ViewCount: {comic.ViewCount}, Chapters: {comic.Chapters.Count}");
            }

            // If no popular comics (all have 0 views), get latest comics instead
            if (!popularComics.Any() || popularComics.All(c => c.ViewCount == 0))
            {
                popularComics = await _context.Comics
                    .Include(c => c.ComicGenres)
                    .ThenInclude(cg => cg.Genre)
                    .Include(c => c.Chapters.Where(ch => ch.IsActive).OrderByDescending(ch => ch.UpdatedDate).Take(3))
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.UpdatedDate)
                    .Take(6)
                    .ToListAsync();
                
                _logger.LogInformation($"Using latest comics as popular fallback: {popularComics.Count} comics");
            }

            // Get recently added comics with recent chapters
            var newComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters.Where(ch => ch.IsActive).OrderByDescending(ch => ch.UpdatedDate).Take(3))
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedDate)
                .Take(6)
                .ToListAsync();

            ViewBag.PopularComics = popularComics;
            ViewBag.NewComics = newComics;

            return View(latestComics);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
