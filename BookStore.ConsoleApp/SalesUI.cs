using System;
using System.Linq;
using BookStoreDataAccess;
using BookStoreDataAccess.Entities;

namespace BookStore.ConsoleApp
{
    static class SalesUI
    {
        public static void Menu()
        {
            Console.WriteLine("-- Меню продажів --");
            Console.WriteLine("1) Створити продаж");
            Console.WriteLine("2) Список останніх продажів");
            Console.Write("Вибір: ");
            var k = Console.ReadLine();
            switch (k)
            {
                case "1": CreateSale(); break;
                case "2": ListRecentSales(); break;
                default: Console.WriteLine("Невідомий варіант"); break;
            }
        }

        public static void CreateSale()
        {
            Console.WriteLine("-- Створити продаж --");

            using var db = new BookStoreDbContext();

            var customers = db.Customers.OrderBy(c => c.Id).ToList();
            Console.WriteLine("Клієнти (id: username):");
            foreach (var c0 in customers) Console.WriteLine($"{c0.Id}: {c0.Username} - {c0.FirstName} {c0.LastName}");
            Console.Write("CustomerId (або порожньо для гостя): "); var cs = Console.ReadLine(); int? customerId = string.IsNullOrWhiteSpace(cs) ? null : int.Parse(cs);

            Sale sale = null;

            while (true)
            {
                var booksList = db.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var b0 in booksList) Console.WriteLine($"{b0.Id}: {b0.Title}");

                Console.Write("BookId (або 0 для завершення): "); var bs = int.Parse(Console.ReadLine() ?? "0");
                if (bs == 0) break;
                var book = db.Books.FirstOrDefault(b => b.Id == bs && b.IsActive);
                if (book == null) { Console.WriteLine("Книга не знайдена"); continue; }
                Console.Write($"Кількість (доступно {book.Stock}): "); var q = int.Parse(Console.ReadLine() ?? "0");
                if (q <= 0 || q > book.Stock) { Console.WriteLine("Невірна кількість"); continue; }

                var now = DateTime.UtcNow;
                decimal discount = 0m;
                var promo = db.BookPromotions.Where(bp => bp.BookId == book.Id).Select(bp => bp.Promotion).FirstOrDefault(p => p.IsActive && p.StartDate <= now && p.EndDate >= now);
                if (promo != null) discount = promo.DiscountPercent;

                var unitPrice = book.SalePrice;
                var lineTotal = q * unitPrice * (1 - discount / 100m);

                if (sale == null)
                {
                    sale = new Sale { Date = DateTime.UtcNow, CustomerId = customerId, TotalAmount = 0m, Note = "" };
                    db.Sales.Add(sale);
                }

                var item = new SaleItem
                {
                    BookId = book.Id,
                    Quantity = q,
                    UnitPrice = unitPrice,
                    DiscountPercent = discount,
                    LineTotal = lineTotal
                };

                sale.Items.Add(item);

                book.Stock -= q;
                sale.TotalAmount += lineTotal;

                // Save after adding item to ensure DB state updated incrementally
                db.SaveChanges();
            }

            if (sale == null || !sale.Items.Any())
            {
                Console.WriteLine("Немає товарів — продаж скасовано");
                return;
            }

            if (customerId.HasValue)
            {
                var c = db.Customers.FirstOrDefault(x => x.Id == customerId.Value);
                if (c != null)
                {
                    c.TotalSpent += sale.TotalAmount;
                    db.SaveChanges();
                }
            }

            Console.WriteLine($"Продаж збережено. Id={sale.Id}, Сума={sale.TotalAmount}");
        }

        public static void ListRecentSales()
        {
            var from = DateTime.UtcNow.AddDays(-7);
            using var db = new BookStoreDbContext();
            var sales = db.Sales.Where(s => s.Date >= from).OrderByDescending(s => s.Date).ToList();
            foreach (var s in sales)
            {
                Console.WriteLine($"Продаж {s.Id} | {s.Date} | Сума {s.TotalAmount}");
            }
        }
    }
}
