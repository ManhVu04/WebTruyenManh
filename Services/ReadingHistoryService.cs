using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Data;
using WebTruyenHay.Models;

namespace WebTruyenHay.Services
{
    public interface IReadingHistoryService
    {
        Task UpdateReadingHistoryAsync(string userId, int comicId, int chapterId, int chapterNumber);
        Task<ReadingHistory?> GetReadingHistoryAsync(string userId, int comicId);
        Task<List<ReadingHistory>> GetUserReadingHistoriesAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task DeleteReadingHistoryAsync(string userId, int comicId);
    }

    public class ReadingHistoryService : IReadingHistoryService
    {
        private readonly ApplicationDbContext _context;

        public ReadingHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateReadingHistoryAsync(string userId, int comicId, int chapterId, int chapterNumber)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            var existingHistory = await _context.ReadingHistories
                .FirstOrDefaultAsync(rh => rh.UserId == userId && rh.ComicId == comicId);

            if (existingHistory != null)
            {
                // Update existing record
                existingHistory.ChapterId = chapterId;
                existingHistory.ChapterNumber = chapterNumber;
                existingHistory.LastReadDate = DateTime.Now;
                _context.ReadingHistories.Update(existingHistory);
            }
            else
            {
                // Create new record
                var newHistory = new ReadingHistory
                {
                    UserId = userId,
                    ComicId = comicId,
                    ChapterId = chapterId,
                    ChapterNumber = chapterNumber,
                    LastReadDate = DateTime.Now
                };
                _context.ReadingHistories.Add(newHistory);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ReadingHistory?> GetReadingHistoryAsync(string userId, int comicId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _context.ReadingHistories
                .Include(rh => rh.Comic)
                .Include(rh => rh.Chapter)
                .FirstOrDefaultAsync(rh => rh.UserId == userId && rh.ComicId == comicId);
        }

        public async Task<List<ReadingHistory>> GetUserReadingHistoriesAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<ReadingHistory>();

            return await _context.ReadingHistories
                .Include(rh => rh.Comic)
                .Include(rh => rh.Chapter)
                .Where(rh => rh.UserId == userId)
                .OrderByDescending(rh => rh.LastReadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task DeleteReadingHistoryAsync(string userId, int comicId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            var history = await _context.ReadingHistories
                .FirstOrDefaultAsync(rh => rh.UserId == userId && rh.ComicId == comicId);

            if (history != null)
            {
                _context.ReadingHistories.Remove(history);
                await _context.SaveChangesAsync();
            }
        }
    }
}
