using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;
using WebTruyenHay.Models.ViewModels;

namespace WebTruyenHay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Comics
        public async Task<IActionResult> Comics()
        {
            var comics = await _context.Comics
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .Include(c => c.Chapters)
                .OrderByDescending(c => c.UpdatedDate)
                .ToListAsync();
            
            return View(comics);
        }

        // GET: Admin/CreateComic
        public async Task<IActionResult> CreateComic()
        {
            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = genres;
            return View(new ComicViewModel());
        }

        // POST: Admin/CreateComic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComic(ComicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comic = new Comic
                {
                    Title = model.Title,
                    Description = model.Description,
                    Author = model.Author,
                    Status = model.Status,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                // Handle cover image upload
                if (model.CoverImageFile != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "comics");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImageFile.CopyToAsync(fileStream);
                    }
                    
                    comic.CoverImageUrl = "/images/comics/" + uniqueFileName;
                }

                _context.Comics.Add(comic);
                await _context.SaveChangesAsync();

                // Add genres
                foreach (var genreId in model.SelectedGenreIds)
                {
                    _context.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = genreId });
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm truyện thành công!";
                return RedirectToAction(nameof(Comics));
            }

            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = genres;
            return View(model);
        }

        // GET: Admin/EditComic/5
        public async Task<IActionResult> EditComic(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics
                .Include(c => c.ComicGenres)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comic == null)
            {
                return NotFound();
            }            var model = new ComicViewModel
            {
                Id = comic.Id,
                Title = comic.Title,
                Description = comic.Description,
                Author = comic.Author,
                CoverImageUrl = comic.CoverImageUrl ?? string.Empty,
                Status = comic.Status,
                SelectedGenreIds = comic.ComicGenres.Select(cg => cg.GenreId).ToList()
            };

            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = genres;
            return View(model);
        }        // POST: Admin/EditComic/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComic(int id, ComicViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var comic = await _context.Comics
                        .Include(c => c.ComicGenres)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (comic == null)
                    {
                        return NotFound();
                    }

                    comic.Title = model.Title;
                    comic.Description = model.Description;
                    comic.Author = model.Author;
                    comic.Status = model.Status;
                    comic.UpdatedDate = DateTime.Now;

                    // Handle cover image upload
                    if (model.CoverImageFile != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(comic.CoverImageUrl))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, comic.CoverImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "comics");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.CoverImageFile.CopyToAsync(fileStream);
                        }
                        
                        comic.CoverImageUrl = "/images/comics/" + uniqueFileName;
                    }

                    // Update genres
                    _context.ComicGenres.RemoveRange(comic.ComicGenres);
                    if (model.SelectedGenreIds != null && model.SelectedGenreIds.Any())
                    {
                        foreach (var genreId in model.SelectedGenreIds)
                        {
                            _context.ComicGenres.Add(new ComicGenre { ComicId = comic.Id, GenreId = genreId });
                        }
                    }

                    await _context.SaveChangesAsync();                    TempData["Success"] = "Cập nhật truyện thành công!";
                    return RedirectToAction(nameof(Comics));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật truyện: " + ex.Message;
                }
            }
            else
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                    
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường thông tin.";
            }

            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = genres;
            return View(model);
        }        // GET: Admin/Chapters/5
        public async Task<IActionResult> Chapters(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics
                .Include(c => c.Chapters)
                .ThenInclude(ch => ch.ChapterImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comic == null)
            {
                return NotFound();
            }

            ViewBag.Comic = comic;
            return View(comic.Chapters.OrderBy(ch => ch.ChapterNumber).ToList());
        }

        // GET: Admin/CreateChapter/5
        public async Task<IActionResult> CreateChapter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics.FindAsync(id);
            if (comic == null)
            {
                return NotFound();
            }

            var lastChapter = await _context.Chapters
                .Where(ch => ch.ComicId == id)
                .OrderByDescending(ch => ch.ChapterNumber)
                .FirstOrDefaultAsync();

            var model = new ChapterViewModel
            {
                ComicId = comic.Id,
                ComicTitle = comic.Title,
                ChapterNumber = (lastChapter?.ChapterNumber ?? 0) + 1
            };

            return View(model);
        }

        // POST: Admin/CreateChapter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChapter(ChapterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var chapter = new Chapter
                {
                    Title = model.Title,
                    ChapterNumber = model.ChapterNumber,
                    ComicId = model.ComicId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Chapters.Add(chapter);
                await _context.SaveChangesAsync();

                // Handle image uploads
                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "chapters", chapter.Id.ToString());
                    Directory.CreateDirectory(uploadsFolder);

                    int pageNumber = 1;
                    foreach (var imageFile in model.ImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            var uniqueFileName = $"page_{pageNumber:D3}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            var chapterImage = new ChapterImage
                            {
                                ChapterId = chapter.Id,
                                ImageUrl = $"/images/chapters/{chapter.Id}/{uniqueFileName}",
                                PageNumber = pageNumber
                            };

                            _context.ChapterImages.Add(chapterImage);
                            pageNumber++;
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                // Update comic's updated date
                var comic = await _context.Comics.FindAsync(model.ComicId);
                if (comic != null)
                {
                    comic.UpdatedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Thêm chương thành công!";
                return RedirectToAction(nameof(Chapters), new { id = model.ComicId });
            }

            var comicForModel = await _context.Comics.FindAsync(model.ComicId);
            if (comicForModel != null)
            {
                model.ComicTitle = comicForModel.Title;
            }

            return View(model);
        }        // GET: Admin/EditChapter/5
        public async Task<IActionResult> EditChapter(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters
                .Include(ch => ch.Comic)
                .Include(ch => ch.ChapterImages)
                .FirstOrDefaultAsync(ch => ch.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }

            var model = new ChapterViewModel
            {
                Id = chapter.Id,
                Title = chapter.Title,
                ChapterNumber = chapter.ChapterNumber,
                ComicId = chapter.ComicId,
                ComicTitle = chapter.Comic.Title,
                ExistingImages = chapter.ChapterImages.OrderBy(img => img.PageNumber).ToList()
            };

            return View(model);
        }

        // POST: Admin/EditChapter/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChapter(int id, ChapterViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var chapter = await _context.Chapters
                    .Include(ch => ch.ChapterImages)
                    .FirstOrDefaultAsync(ch => ch.Id == id);

                if (chapter == null)
                {
                    return NotFound();
                }

                chapter.Title = model.Title;
                chapter.ChapterNumber = model.ChapterNumber;
                chapter.UpdatedDate = DateTime.Now;

                // Handle new image uploads if provided
                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    // Delete old images
                    if (chapter.ChapterImages != null && chapter.ChapterImages.Any())
                    {
                        foreach (var oldImage in chapter.ChapterImages)
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        _context.ChapterImages.RemoveRange(chapter.ChapterImages);
                    }

                    // Upload new images
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "chapters", chapter.Id.ToString());
                    if (Directory.Exists(uploadsFolder))
                    {
                        Directory.Delete(uploadsFolder, true);
                    }
                    Directory.CreateDirectory(uploadsFolder);

                    int pageNumber = 1;
                    foreach (var imageFile in model.ImageFiles)
                    {
                        if (imageFile.Length > 0)
                        {
                            var uniqueFileName = $"page_{pageNumber:D3}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            var chapterImage = new ChapterImage
                            {
                                ChapterId = chapter.Id,
                                ImageUrl = $"/images/chapters/{chapter.Id}/{uniqueFileName}",
                                PageNumber = pageNumber
                            };

                            _context.ChapterImages.Add(chapterImage);
                            pageNumber++;
                        }
                    }
                }

                // Update comic's updated date
                var comic = await _context.Comics.FindAsync(model.ComicId);
                if (comic != null)
                {
                    comic.UpdatedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật chương thành công!";
                return RedirectToAction(nameof(Chapters), new { id = model.ComicId });
            }

            var comicForModel = await _context.Comics.FindAsync(model.ComicId);
            if (comicForModel != null)
            {
                model.ComicTitle = comicForModel.Title;
            }

            return View(model);
        }        // POST: Admin/DeleteComic/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComic(int id)
        {
            try
            {
                var comic = await _context.Comics
                    .Include(c => c.Chapters)
                        .ThenInclude(ch => ch.ChapterImages)
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (comic != null)
                {
                    // Delete related data first to avoid foreign key constraints
                    foreach (var chapter in comic.Chapters)
                    {
                        // Delete comments for each chapter
                        var comments = await _context.Comments.Where(c => c.ChapterId == chapter.Id).ToListAsync();
                        if (comments.Any())
                        {
                            _context.Comments.RemoveRange(comments);
                        }
                        
                        // Delete reading history for each chapter
                        var readingHistories = await _context.ReadingHistories.Where(rh => rh.ChapterId == chapter.Id).ToListAsync();
                        if (readingHistories.Any())
                        {
                            _context.ReadingHistories.RemoveRange(readingHistories);
                        }
                    }
                    
                    // Delete follows for this comic
                    var follows = await _context.Follows.Where(f => f.ComicId == id).ToListAsync();
                    if (follows.Any())
                    {
                        _context.Follows.RemoveRange(follows);
                    }
                    
                    // Delete reading histories for this comic (those without specific chapters)
                    var comicReadingHistories = await _context.ReadingHistories.Where(rh => rh.ComicId == id).ToListAsync();
                    if (comicReadingHistories.Any())
                    {
                        _context.ReadingHistories.RemoveRange(comicReadingHistories);
                    }
                    
                    // Save changes for related data first
                    await _context.SaveChangesAsync();

                    // Delete cover image if exists
                    if (!string.IsNullOrEmpty(comic.CoverImageUrl))
                    {
                        var coverPath = Path.Combine(_webHostEnvironment.WebRootPath, comic.CoverImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(coverPath))
                            System.IO.File.Delete(coverPath);
                    }

                    // Delete chapter images and folders
                    foreach (var chapter in comic.Chapters)
                    {
                        if (chapter.ChapterImages != null)
                        {
                            foreach (var img in chapter.ChapterImages)
                            {
                                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                                if (System.IO.File.Exists(imgPath))
                                    System.IO.File.Delete(imgPath);
                            }
                            // Delete chapter folder
                            var chapterFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "chapters", chapter.Id.ToString());
                            if (Directory.Exists(chapterFolder))
                                Directory.Delete(chapterFolder, true);
                        }
                    }

                    // Finally delete the comic (this will cascade delete chapters and images)
                    _context.Comics.Remove(comic);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Xóa truyện thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy truyện để xóa!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa truyện: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Comics));
        }// POST: Admin/DeleteChapter/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            try
            {
                var chapter = await _context.Chapters
                    .Include(ch => ch.ChapterImages)
                    .FirstOrDefaultAsync(ch => ch.Id == id);
                
                if (chapter != null)
                {
                    var comicId = chapter.ComicId;
                    
                    // Delete related data first to avoid foreign key constraints
                    // Delete comments for this chapter
                    var comments = await _context.Comments.Where(c => c.ChapterId == id).ToListAsync();
                    if (comments.Any())
                    {
                        _context.Comments.RemoveRange(comments);
                    }
                    
                    // Delete reading history for this chapter
                    var readingHistories = await _context.ReadingHistories.Where(rh => rh.ChapterId == id).ToListAsync();
                    if (readingHistories.Any())
                    {
                        _context.ReadingHistories.RemoveRange(readingHistories);
                    }
                    
                    // Delete chapter images from database (will be cascaded, but explicit is better)
                    if (chapter.ChapterImages != null && chapter.ChapterImages.Any())
                    {
                        _context.ChapterImages.RemoveRange(chapter.ChapterImages);
                    }
                    
                    // Save changes for related data first
                    await _context.SaveChangesAsync();
                    
                    // Now delete physical files
                    if (chapter.ChapterImages != null)
                    {
                        foreach (var img in chapter.ChapterImages)
                        {
                            var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                            if (System.IO.File.Exists(imgPath))
                            {
                                System.IO.File.Delete(imgPath);
                            }
                        }
                        // Delete chapter folder
                        var chapterFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "chapters", chapter.Id.ToString());
                        if (Directory.Exists(chapterFolder))
                        {
                            Directory.Delete(chapterFolder, true);
                        }
                    }
                    
                    // Finally delete the chapter itself
                    _context.Chapters.Remove(chapter);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Xóa chương thành công!";
                    return RedirectToAction(nameof(Chapters), new { id = comicId });
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy chương để xóa!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa chương: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Comics));
        }
    }
}
