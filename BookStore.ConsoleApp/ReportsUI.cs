using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookStoreDataAccess;

namespace BookStore.ConsoleApp
{
    static class ReportsUI
    {
        public static void Menu()
        {
            Console.WriteLine("-- Меню звітів --");
            Console.WriteLine("1) Топ книг (за період, дні)");
            Console.WriteLine("2) Топ авторів (за період, дні)");
            Console.Write("Вибір: ");
            var k = Console.ReadLine();
            switch (k)
            {
                case "1": TopBooks(); break;
                case "2": TopAuthors(); break;
                default: Console.WriteLine("Невідомий варіант"); break;
            }
        }

        public static void TopBooks()
        {
            Console.Write("Період у днях: "); var d = int.Parse(Console.ReadLine() ?? "7");
            var from = DateTime.UtcNow.AddDays(-d);
            using var db = new BookStoreDbContext();
            var top = db.SaleItems
                .Where(si => si.Sale.Date >= from)
                .GroupBy(si => si.BookId)
                .Select(g => new { BookId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Qty)
                .Take(10)
                .ToList();

            foreach (var t in top)
            {
                var book = db.Books.Find(t.BookId);
                Console.WriteLine($"{book?.Title} | Продано: {t.Qty}");
            }
        }

        public static void TopAuthors()
        {
            Console.Write("Період у днях: "); var d = int.Parse(Console.ReadLine() ?? "30");
            var from = DateTime.UtcNow.AddDays(-d);
            using var db = new BookStoreDbContext();
            var top = db.SaleItems
                .Where(si => si.Sale.Date >= from)
                .Include(si => si.Book)
                .GroupBy(si => si.Book.AuthorId)
                .Select(g => new { AuthorId = g.Key, Qty = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.Qty)
                .Take(10)
                .ToList();

            foreach (var t in top)
            {
                var author = db.Authors.Find(t.AuthorId);
                Console.WriteLine($"{author?.FullName} | Продано: {t.Qty}");
            }
        }
    }
}
