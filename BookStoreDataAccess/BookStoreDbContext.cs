using System;
using Microsoft.EntityFrameworkCore;
using BookStoreDataAccess.Entities;

namespace BookStoreDataAccess
{
    public class BookStoreDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<BookPromotion> BookPromotions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<WriteOff> WriteOffs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=PULSE\SQLEXPRESS;
                                        Initial Catalog = BookStore;
                                        Integrated Security=True;
                                        Connect Timeout=5;
                                        Encrypt=False;Trust Server Certificate=False;
                                        Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(eb =>
            {
                eb.HasKey(a => a.Id);
                eb.Property(a => a.FullName).HasMaxLength(250).IsRequired();
                eb.HasIndex(a => a.FullName);
                eb.HasMany(a => a.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Publisher>(eb =>
            {
                eb.HasKey(p => p.Id);
                eb.Property(p => p.Name).HasMaxLength(200).IsRequired();
                eb.HasIndex(p => p.Name);
                eb.HasMany(p => p.Books).WithOne(b => b.Publisher).HasForeignKey(b => b.PublisherId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Genre>(eb =>
            {
                eb.HasKey(g => g.Id);
                eb.Property(g => g.Name).HasMaxLength(100).IsRequired();
                eb.HasIndex(g => g.Name);
                eb.HasMany(g => g.Books).WithOne(b => b.Genre).HasForeignKey(b => b.GenreId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Book>(eb =>
            {
                eb.HasKey(b => b.Id);
                eb.Property(b => b.Title).HasMaxLength(250).IsRequired();
                eb.Property(b => b.CostPrice).HasColumnType("decimal(18,2)").IsRequired();
                eb.Property(b => b.SalePrice).HasColumnType("decimal(18,2)").IsRequired();
                eb.Property(b => b.Stock).HasDefaultValue(0);
                eb.Property(b => b.AddedDate).HasDefaultValueSql("GETUTCDATE()");
                eb.Property(b => b.IsActive).HasDefaultValue(true);
                eb.HasIndex(b => b.Title);

                eb.HasOne(b => b.ParentBook).WithMany(b => b.Continuations).HasForeignKey(b => b.ParentBookId).OnDelete(DeleteBehavior.Restrict);
                eb.HasMany(b => b.SaleItems).WithOne(si => si.Book).HasForeignKey(si => si.BookId).OnDelete(DeleteBehavior.Restrict);
                eb.HasMany(b => b.Reservations).WithOne(r => r.Book).HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Cascade);
                eb.HasMany(b => b.WriteOffs).WithOne(w => w.Book).HasForeignKey(w => w.BookId).OnDelete(DeleteBehavior.Cascade);
                eb.HasMany(b => b.BookPromotions).WithOne(bp => bp.Book).HasForeignKey(bp => bp.BookId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Promotion>(eb =>
            {
                eb.HasKey(p => p.Id);
                eb.Property(p => p.Name).HasMaxLength(200).IsRequired();
                eb.Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)").IsRequired();
                eb.Property(p => p.IsActive).HasDefaultValue(true);
                eb.HasMany(p => p.BookPromotions).WithOne(bp => bp.Promotion).HasForeignKey(bp => bp.PromotionId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookPromotion>(eb =>
            {
                eb.HasKey(bp => new { bp.BookId, bp.PromotionId });
                eb.HasOne(bp => bp.Book).WithMany(b => b.BookPromotions).HasForeignKey(bp => bp.BookId);
                eb.HasOne(bp => bp.Promotion).WithMany(p => p.BookPromotions).HasForeignKey(bp => bp.PromotionId);
            });

            modelBuilder.Entity<Customer>(eb =>
            {
                eb.HasKey(c => c.Id);
                eb.Property(c => c.Username).HasMaxLength(100);
                eb.HasIndex(c => c.Username);
                eb.Property(c => c.TotalSpent).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
                eb.HasMany(c => c.Sales).WithOne(s => s.Customer).HasForeignKey(s => s.CustomerId).OnDelete(DeleteBehavior.SetNull);
                eb.HasMany(c => c.Reservations).WithOne(r => r.Customer).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Sale>(eb =>
            {
                eb.HasKey(s => s.Id);
                eb.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                eb.Property(s => s.Note).HasMaxLength(500).HasDefaultValue("");
                eb.Property(s => s.Date).HasDefaultValueSql("GETUTCDATE()");
                eb.HasMany(s => s.Items).WithOne(i => i.Sale).HasForeignKey(i => i.SaleId).OnDelete(DeleteBehavior.Cascade);
                eb.HasIndex(s => s.Date);
            });

            modelBuilder.Entity<SaleItem>(eb =>
            {
                eb.HasKey(si => si.Id);
                eb.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                eb.Property(si => si.DiscountPercent).HasColumnType("decimal(5,2)");
                eb.Property(si => si.LineTotal).HasColumnType("decimal(18,2)").IsRequired();
            });

            modelBuilder.Entity<Reservation>(eb =>
            {
                eb.HasKey(r => r.Id);
                eb.Property(r => r.Status).HasMaxLength(50).IsRequired().HasDefaultValue("Reserved");
                eb.Property(r => r.ReservedAt).HasDefaultValueSql("GETUTCDATE()");
                eb.Property(r => r.Note).HasMaxLength(500).HasDefaultValue("");
            });

            modelBuilder.Entity<WriteOff>(eb =>
            {
                eb.HasKey(w => w.Id);
                eb.Property(w => w.Reason).HasMaxLength(500);
                eb.Property(w => w.Date).HasDefaultValueSql("GETUTCDATE()");
            });

            SeedGenres(modelBuilder);
            SeedAuthors(modelBuilder);
            SeedPublishers(modelBuilder);
            SeedBooks(modelBuilder);
        }

        private void SeedGenres(ModelBuilder modelBuilder)
        {
            var genres = new[]
            {
                new Genre { Id = 1, Name = "Fiction" },
                new Genre { Id = 2, Name = "Non-Fiction" },
                new Genre { Id = 3, Name = "Sci-Fi" },
                new Genre { Id = 4, Name = "Fantasy" },
                new Genre { Id = 5, Name = "Romance" },
                new Genre { Id = 6, Name = "Thriller" },
                new Genre { Id = 7, Name = "Children" },
                new Genre { Id = 8, Name = "Education" }
            };

            modelBuilder.Entity<Genre>().HasData(genres);
        }

        private void SeedAuthors(ModelBuilder modelBuilder)
        {
            var authors = new[]
            {
                new Author { Id = 1, FullName = "George R. R. Martin" },
                new Author { Id = 2, FullName = "J. K. Rowling" },
                new Author { Id = 3, FullName = "Isaac Asimov" },
                new Author { Id = 4, FullName = "Stephen King" },
                new Author { Id = 5, FullName = "Agatha Christie" },
                new Author { Id = 6, FullName = "Ernest Hemingway" }
            };

            modelBuilder.Entity<Author>().HasData(authors);
        }

        private void SeedPublishers(ModelBuilder modelBuilder)
        {
            var pubs = new[]
            {
                new Publisher { Id = 1, Name = "Penguin Random House", Country = "USA" },
                new Publisher { Id = 2, Name = "HarperCollins", Country = "USA" },
                new Publisher { Id = 3, Name = "Macmillan Publishers", Country = "UK" },
                new Publisher { Id = 4, Name = "Hachette Livre", Country = "France" }
            };

            modelBuilder.Entity<Publisher>().HasData(pubs);
        }

        private void SeedBooks(ModelBuilder modelBuilder)
        {
            var books = new[]
            {
                new Book { Id = 1, Title = "A Game of Thrones", AuthorId = 1, PublisherId = 1, GenreId = 4, Pages = 694, Year = 1996, CostPrice = 8.50m, SalePrice = 19.99m, Stock = 10, AddedDate = new DateTime(2024,1,5), IsActive = true },
                new Book { Id = 2, Title = "Harry Potter and the Sorcerer's Stone", AuthorId = 2, PublisherId = 2, GenreId = 4, Pages = 309, Year = 1997, CostPrice = 5.00m, SalePrice = 12.99m, Stock = 15, AddedDate = new DateTime(2024,2,10), IsActive = true },
                new Book { Id = 3, Title = "Foundation", AuthorId = 3, PublisherId = 3, GenreId = 3, Pages = 255, Year = 1951, CostPrice = 4.00m, SalePrice = 9.99m, Stock = 8, AddedDate = new DateTime(2024,3,1), IsActive = true },
                new Book { Id = 4, Title = "The Shining", AuthorId = 4, PublisherId = 4, GenreId = 6, Pages = 447, Year = 1977, CostPrice = 6.00m, SalePrice = 14.99m, Stock = 5, AddedDate = new DateTime(2024,4,20), IsActive = true },
                new Book { Id = 5, Title = "Murder on the Orient Express", AuthorId = 5, PublisherId = 1, GenreId = 6, Pages = 256, Year = 1934, CostPrice = 3.50m, SalePrice = 8.99m, Stock = 7, AddedDate = new DateTime(2024,5,12), IsActive = true },
                new Book { Id = 6, Title = "The Old Man and the Sea", AuthorId = 6, PublisherId = 2, GenreId = 1, Pages = 127, Year = 1952, CostPrice = 2.00m, SalePrice = 6.99m, Stock = 4, AddedDate = new DateTime(2024,6,7), IsActive = true }
            };

            modelBuilder.Entity<Book>().HasData(books);
        }
    }
}
