using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebTruyenHay.Models;

namespace WebTruyenHay.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterImage> ChapterImages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ComicGenre> ComicGenres { get; set; }        public DbSet<Comment> Comments { get; set; } // Thêm DbSet<Comment> vào ApplicationDbContext để quản lý bình luận
        public DbSet<Follow> Follows { get; set; } // Thêm DbSet<Follow> để quản lý theo dõi truyện
        public DbSet<ReadingHistory> ReadingHistories { get; set; } // Thêm DbSet<ReadingHistory> để quản lý lịch sử đọc
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure many-to-many relationship between Comic and Genre
            modelBuilder.Entity<ComicGenre>()
                .HasKey(cg => new { cg.ComicId, cg.GenreId });
                
            modelBuilder.Entity<ComicGenre>()
                .HasOne(cg => cg.Comic)
                .WithMany(c => c.ComicGenres)
                .HasForeignKey(cg => cg.ComicId);
                
            modelBuilder.Entity<ComicGenre>()
                .HasOne(cg => cg.Genre)
                .WithMany(g => g.ComicGenres)
                .HasForeignKey(cg => cg.GenreId);
                
            // Configure one-to-many relationship between Comic and Chapter
            modelBuilder.Entity<Chapter>()
                .HasOne(ch => ch.Comic)
                .WithMany(c => c.Chapters)
                .HasForeignKey(ch => ch.ComicId);
                
            // Configure one-to-many relationship between Chapter and ChapterImage
            modelBuilder.Entity<ChapterImage>()
                .HasOne(ci => ci.Chapter)
                .WithMany(ch => ch.ChapterImages)
                .HasForeignKey(ci => ci.ChapterId);
                
            // Configure Follow relationship
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Comic)
                .WithMany()
                .HasForeignKey(f => f.ComicId)
                .OnDelete(DeleteBehavior.Cascade);
                  // Create unique index for UserId and ComicId to prevent duplicate follows
            modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.UserId, f.ComicId })
                .IsUnique();
                  // Configure ReadingHistory relationship
            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Comic)
                .WithMany()
                .HasForeignKey(rh => rh.ComicId)
                .OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<ReadingHistory>()
                .HasOne(rh => rh.Chapter)
                .WithMany()
                .HasForeignKey(rh => rh.ChapterId)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Create unique index for UserId and ComicId to track only latest reading position per comic
            modelBuilder.Entity<ReadingHistory>()
                .HasIndex(rh => new { rh.UserId, rh.ComicId })
                .IsUnique();
                
            // Seed data for genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Hành Động", Description = "Thể loại hành động" },
                new Genre { Id = 2, Name = "Phiêu Lưu", Description = "Thể loại phiêu lưu" },
                new Genre { Id = 3, Name = "Hài Hước", Description = "Thể loại hài hước" },
                new Genre { Id = 4, Name = "Lãng Mạn", Description = "Thể loại lãng mạn" },
                new Genre { Id = 5, Name = "Học Đường", Description = "Thể loại học đường" },
                new Genre { Id = 6, Name = "Siêu Nhiên", Description = "Thể loại siêu nhiên" },
                new Genre { Id = 7, Name = "Kinh Dị", Description = "Thể loại kinh dị" },
                new Genre { Id = 8, Name = "Drama", Description = "Thể loại drama" }
            );
        }
    }
}
