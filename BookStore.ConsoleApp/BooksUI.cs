using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookStoreDataAccess;
using BookStoreDataAccess.Entities;

namespace BookStore.ConsoleApp
{
    static class BooksUI
    {
        public static void Menu()
        {
            Console.WriteLine("-- Меню книг --");
            Console.WriteLine("1) Додати книгу");
            Console.WriteLine("2) Редагувати книгу");
            Console.WriteLine("3) Видалити книгу");
            Console.WriteLine("4) Пошук книг");
            Console.WriteLine("5) Список новинок (30 днів)");
            Console.WriteLine("6) Показати список книг");
            Console.Write("Вибір: ");
            var key = Console.ReadLine();
            switch (key)
            {
                case "1": AddBook(); break;
                case "2": EditBook(); break;
                case "3": DeleteBook(); break;
                case "4": SearchBooks(); break;
                case "5": ListNovelties(); break;
                case "6": ViewAllBooks(); break;
                default: Console.WriteLine("Невідомий варіант"); break;
            }
        }

        public static void AddBook()
        {
            Console.WriteLine("-- Додати книгу --");

            using (var db = new BookStoreDbContext())
            {
                var authors = db.Authors.OrderBy(a => a.Id).ToList();
                Console.WriteLine("Автори:");
                foreach (var a in authors) Console.WriteLine($"{a.Id}: {a.FullName}");
            }
            Console.Write("AuthorId: "); int.TryParse(Console.ReadLine() ?? "0", out var authorId);

            using (var db = new BookStoreDbContext())
            {
                var pubs = db.Publishers.OrderBy(p => p.Id).ToList();
                Console.WriteLine("Видавництва:");
                foreach (var p in pubs) Console.WriteLine($"{p.Id}: {p.Name} ({p.Country})");
            }
            Console.Write("PublisherId (або порожньо): "); var pubS = Console.ReadLine(); int? pubId = string.IsNullOrWhiteSpace(pubS) ? null : int.Parse(pubS);

            using (var db = new BookStoreDbContext())
            {
                var genres = db.Genres.OrderBy(g => g.Id).ToList();
                Console.WriteLine("Жанри:");
                foreach (var g in genres) Console.WriteLine($"{g.Id}: {g.Name}");
            }
            Console.Write("GenreId: "); int.TryParse(Console.ReadLine() ?? "0", out var genreId);

            Console.Write("Title: "); var title = Console.ReadLine();
            Console.Write("Pages: "); int.TryParse(Console.ReadLine() ?? "0", out var pages);
            Console.Write("Year: "); int.TryParse(Console.ReadLine() ?? "0", out var year);
            Console.Write("CostPrice: "); decimal.TryParse(Console.ReadLine() ?? "0", out var cost);
            Console.Write("SalePrice: "); decimal.TryParse(Console.ReadLine() ?? "0", out var sale);
            Console.Write("Stock: "); int.TryParse(Console.ReadLine() ?? "0", out var stock);

            using (var db = new BookStoreDbContext())
            {
                var books = db.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var b in books) Console.WriteLine($"{b.Id}: {b.Title}");
            }
            Console.Write("ParentBookId (або порожньо): "); var parentS = Console.ReadLine(); int? parentId = string.IsNullOrWhiteSpace(parentS) ? null : int.Parse(parentS);

            var book = new Book
            {
                Title = title,
                AuthorId = authorId,
                PublisherId = pubId,
                GenreId = genreId,
                Pages = pages,
                Year = year,
                CostPrice = cost,
                SalePrice = sale,
                Stock = stock,
                ParentBookId = parentId,
                AddedDate = DateTime.UtcNow,
                IsActive = true
            };

            using var db2 = new BookStoreDbContext();
            db2.Books.Add(book);
            db2.SaveChanges();
            Console.WriteLine($"Книга додана з id={book.Id}");
        }

        public static void EditBook()
        {
            using (var dbList = new BookStoreDbContext())
            {
                var booksList = dbList.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var bL in booksList) Console.WriteLine($"{bL.Id}: {bL.Title}");
            }
            Console.Write("Id книги для редагування: "); int.TryParse(Console.ReadLine() ?? "0", out var id);

            using var db = new BookStoreDbContext();
            var book = db.Books.FirstOrDefault(b => b.Id == id && b.IsActive);
            if (book == null) { Console.WriteLine("Книга не знайдена"); return; }

            Console.Write($"Title ({book.Title}): "); var t = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(t)) book.Title = t;

            using (var dbList2 = new BookStoreDbContext())
            {
                var booksList = dbList2.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var bL in booksList) Console.WriteLine($"{bL.Id}: {bL.Title}");
            }
            Console.Write($"ParentBookId ({(book.ParentBookId.HasValue ? book.ParentBookId.ToString() : "none")}): "); var p = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(p)) book.ParentBookId = int.Parse(p);

            Console.Write($"SalePrice ({book.SalePrice}): "); var sp = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(sp)) book.SalePrice = decimal.Parse(sp);
            Console.Write($"Stock ({book.Stock}): "); var st = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(st)) book.Stock = int.Parse(st);

            db.SaveChanges();
            Console.WriteLine("Книга оновлена");
        }

        public static void DeleteBook()
        {
            using (var dbList = new BookStoreDbContext())
            {
                var booksList = dbList.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var bL in booksList) Console.WriteLine($"{bL.Id}: {bL.Title}");
            }
            Console.Write("Id книги для видалення: "); int.TryParse(Console.ReadLine() ?? "0", out var id);
            using var db = new BookStoreDbContext();
            var book = db.Books.FirstOrDefault(b => b.Id == id && b.IsActive);
            if (book == null) { Console.WriteLine("Книга не знайдена"); return; }
            book.IsActive = false;
            db.SaveChanges();
            Console.WriteLine("Книга успішно видалена");
        }

        public static void SearchBooks()
        {
            Console.Write("Термін пошуку (назва/автор/жанр): "); var q = Console.ReadLine();
            using var db = new BookStoreDbContext();
            var results = db.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Where(b => b.IsActive && (b.Title.Contains(q) || b.Author.FullName.Contains(q) || b.Genre.Name.Contains(q)))
                .OrderBy(b => b.Title)
                .ToList();

            foreach (var b in results)
            {
                Console.WriteLine($"{b.Id}: {b.Title} | {b.Author?.FullName} | {b.Genre?.Name} | Ціна {b.SalePrice} | На складі {b.Stock}");
            }
        }

        public static void ListNovelties()
        {
            var from = DateTime.UtcNow.AddDays(-30);
            using var db = new BookStoreDbContext();
            var novelties = db.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Where(b => b.IsActive && b.AddedDate >= from)
                .OrderByDescending(b => b.AddedDate)
                .ToList();

            foreach (var b in novelties)
                Console.WriteLine($"{b.Id}: {b.Title} ({b.AddedDate:d}) - {b.Author?.FullName} - {b.Genre?.Name}");
        }

        public static void ViewAllBooks()
        {
            using var db = new BookStoreDbContext();
            var books = db.Books.Where(b => b.IsActive).Include(b => b.Author).Include(b => b.Genre).OrderBy(b => b.Id).ToList();
            Console.WriteLine("Список книг (id: назва | автор | жанр | ціна | на складі):");
            foreach (var b in books)
            {
                Console.WriteLine($"{b.Id}: {b.Title} | {b.Author?.FullName} | {b.Genre?.Name} | Ціна {b.SalePrice} | На складі {b.Stock}");
            }
        }
    }
}
