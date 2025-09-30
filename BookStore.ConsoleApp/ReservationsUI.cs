using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookStoreDataAccess;
using BookStoreDataAccess.Entities;

namespace BookStore.ConsoleApp
{
    static class ReservationsUI
    {
        public static void Menu()
        {
            Console.WriteLine("-- Меню резервувань --");
            Console.WriteLine("1) Створити резерв");
            Console.WriteLine("2) Отримати резерв (забрати)");
            Console.WriteLine("3) Скасувати резерв");
            Console.WriteLine("4) Показати список резервацій");
            Console.Write("Вибір: ");
            var k = Console.ReadLine();
            switch (k)
            {
                case "1": CreateReservation(); break;
                case "2": CollectReservation(); break;
                case "3": CancelReservation(); break;
                case "4": ListReservations(); break;
                default: Console.WriteLine("Невідомий варіант"); break;
            }
        }

        public static void CreateReservation()
        {
            using (var db = new BookStoreDbContext())
            {
                var books = db.Books.Where(b => b.IsActive).OrderBy(b => b.Id).ToList();
                Console.WriteLine("Книги (id: назва):");
                foreach (var b in books) Console.WriteLine($"{b.Id}: {b.Title}");
            }
            Console.Write("BookId: "); var bId = int.Parse(Console.ReadLine() ?? "0");

            using (var db2 = new BookStoreDbContext())
            {
                var customers = db2.Customers.OrderBy(c => c.Id).ToList();
                Console.WriteLine("Клієнти (id: username):");
                foreach (var c in customers) Console.WriteLine($"{c.Id}: {c.Username} - {c.FirstName} {c.LastName}");
            }
            Console.Write("CustomerId (або порожньо): "); var cs = Console.ReadLine(); int? customerId = string.IsNullOrWhiteSpace(cs) ? null : int.Parse(cs);
            Console.Write("Кількість: "); var qty = int.Parse(Console.ReadLine() ?? "0");

            using var db3 = new BookStoreDbContext();
            var book = db3.Books.FirstOrDefault(b => b.Id == bId && b.IsActive);
            if (book == null) { Console.WriteLine("Книга не знайдена"); return; }
            if (qty <= 0 || qty > book.Stock) { Console.WriteLine("Невірна кількість"); return; }

            var r = new Reservation { BookId = bId, CustomerId = customerId, Quantity = qty, ReservedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddDays(7), Status = "Reserved", Note = "" };
            db3.Reservations.Add(r);
            db3.SaveChanges();
            Console.WriteLine($"Резерв створено id={r.Id}");
        }

        public static void CollectReservation()
        {
            using (var dbList = new BookStoreDbContext())
            {
                var rs = dbList.Reservations.OrderBy(r => r.Id).ToList();
                Console.WriteLine("Резервації (id: BookId | customerId | qty | status):");
                foreach (var rr in rs) Console.WriteLine($"{rr.Id}: {rr.BookId} | {rr.CustomerId} | {rr.Quantity} | {rr.Status}");
            }
            Console.Write("Id резерву для отримання: "); var id = int.Parse(Console.ReadLine() ?? "0");
            using var db = new BookStoreDbContext();
            var r = db.Reservations.FirstOrDefault(x => x.Id == id && x.Status == "Reserved");
            if (r == null) { Console.WriteLine("Резерв не знайдено або вже оброблено"); return; }
            var book = db.Books.FirstOrDefault(b => b.Id == r.BookId);
            if (book == null || book.Stock < r.Quantity) { Console.WriteLine("Недостатньо на складі для отримання"); return; }
            book.Stock -= r.Quantity;
            r.Status = "Collected";
            db.SaveChanges();
            Console.WriteLine("Резерв отримано і запас оновлено");
        }

        public static void CancelReservation()
        {
            using (var dbList = new BookStoreDbContext())
            {
                var rs = dbList.Reservations.OrderBy(r => r.Id).ToList();
                Console.WriteLine("Резервації (id: BookId | customerId | qty | status):");
                foreach (var rr in rs) Console.WriteLine($"{rr.Id}: {rr.BookId} | {rr.CustomerId} | {rr.Quantity} | {rr.Status}");
            }
            Console.Write("Id резерву для скасування: "); var id = int.Parse(Console.ReadLine() ?? "0");
            using var db = new BookStoreDbContext();
            var r = db.Reservations.FirstOrDefault(x => x.Id == id && x.Status == "Reserved");
            if (r == null) { Console.WriteLine("Резерв не знайдено або не в стані 'Reserved'"); return; }
            r.Status = "Cancelled";
            db.SaveChanges();
            Console.WriteLine("Резерв скасовано");
        }

        public static void ListReservations()
        {
            using var db = new BookStoreDbContext();
            var rs = db.Reservations.OrderBy(r => r.Id).ToList();
            Console.WriteLine("Список резервацій (id: BookId | customerId | qty | status):");
            foreach (var r in rs)
            {
                Console.WriteLine($"{r.Id}: {r.BookId} | {r.CustomerId} | {r.Quantity} | {r.Status}");
            }
        }
    }
}
