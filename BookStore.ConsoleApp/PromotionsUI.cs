using System;
using System.Linq;
using BookStoreDataAccess;
using BookStoreDataAccess.Entities;

namespace BookStore.ConsoleApp
{
    static class PromotionsUI
    {
        public static void Menu()
        {
            Console.WriteLine("-- Меню акцій --");
            Console.WriteLine("1) Додати акцію");
            Console.WriteLine("2) Прив'язати акцію до книги");
            Console.WriteLine("3) Список активних акцій");
            Console.Write("Вибір: ");
            var k = Console.ReadLine();
            switch (k)
            {
                case "1": CreatePromotion(); break;
                case "2": AssignPromotionToBook(); break;
                case "3": ListActivePromotions(); break;
                default: Console.WriteLine("Невідомий варіант"); break;
            }
        }

        public static void CreatePromotion()
        {
            Console.Write("Назва: "); var name = Console.ReadLine();
            Console.Write("Відсоток знижки: "); var dp = decimal.Parse(Console.ReadLine() ?? "0");
            Console.Write("Дата початку (yyyy-MM-dd або yyyyMMdd): "); var sdRaw = Console.ReadLine();
            Console.Write("Дата закінчення (yyyy-MM-dd або yyyyMMdd): "); var edRaw = Console.ReadLine();

            DateTime sd, ed;
            string[] formats = new[] { "yyyy-MM-dd", "yyyyMMdd", "yyyy.MM.dd", "dd.MM.yyyy" };
            if (!DateTime.TryParseExact(sdRaw ?? string.Empty, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out sd))
            {
                if (!DateTime.TryParse(sdRaw ?? string.Empty, out sd)) sd = DateTime.UtcNow;
            }
            if (!DateTime.TryParseExact(edRaw ?? string.Empty, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ed))
            {
                if (!DateTime.TryParse(edRaw ?? string.Empty, out ed)) ed = DateTime.UtcNow.AddDays(7);
            }

            var p = new Promotion { Name = name, DiscountPercent = dp, StartDate = sd, EndDate = ed, IsActive = true };
            using var db = new BookStoreDbContext();
            db.Promotions.Add(p);
            db.SaveChanges();
            Console.WriteLine($"Акція додана id={p.Id}");
        }

        public static void AssignPromotionToBook()
        {
            using (var db = new BookStoreDbContext())
            {
                var promos = db.Promotions.OrderBy(p => p.Id).ToList();
                Console.WriteLine("Акції:");
                foreach (var p in promos) Console.WriteLine($"{p.Id}: {p.Name} ({p.DiscountPercent}%) {p.StartDate:d}-{p.EndDate:d}");
            }
            using (var db2 = new BookStoreDbContext())
            {
                var books = db2.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var b in books) Console.WriteLine($"{b.Id}: {b.Title}");
            }

            Console.Write("PromotionId: "); var pid = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("BookId: "); var bid = int.Parse(Console.ReadLine() ?? "0");
            using var db3 = new BookStoreDbContext();
            var exist = db3.BookPromotions.Any(bp => bp.BookId == bid && bp.PromotionId == pid);
            if (exist) { Console.WriteLine("Вже прив'язано"); return; }
            db3.BookPromotions.Add(new BookPromotion { BookId = bid, PromotionId = pid });
            db3.SaveChanges();
            Console.WriteLine("Акція прив'язана до книги");
        }

        public static void ListActivePromotions()
        {
            var now = DateTime.UtcNow;
            using var db = new BookStoreDbContext();
            var list = db.Promotions.Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now).ToList();
            foreach (var p in list) Console.WriteLine($"{p.Id}: {p.Name} ({p.DiscountPercent}%) {p.StartDate:d} - {p.EndDate:d}");
        }
    }
}
