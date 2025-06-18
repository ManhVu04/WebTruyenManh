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
        }

        public async Task<IActionResult> Index()
        {
            // Get latest updated comics
            var latestComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.UpdatedDate)
                .Take(12)
                .ToListAsync();

            // Get most viewed comics
            var popularComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.ViewCount)
                .Take(6)
                .ToListAsync();

            // Get recently added comics
            var newComics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters)
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
