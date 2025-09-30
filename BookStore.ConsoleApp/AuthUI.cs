using System;
using System.Linq;
using BookStoreDataAccess;
using BookStoreDataAccess.Entities;

namespace BookStore.ConsoleApp
{
    static class AuthUI
    {
        public static void Register()
        {
            Console.WriteLine("-- Реєстрація нового покупця --");
            Console.Write("Ім'я користувача (username): "); var username = Console.ReadLine();
            Console.Write("Пароль: "); var password = Console.ReadLine();
            Console.Write("Ім'я: "); var first = Console.ReadLine();
            Console.Write("Прізвище: "); var last = Console.ReadLine();
            Console.Write("Email: "); var email = Console.ReadLine();
            Console.Write("Телефон: "); var phone = Console.ReadLine();

            using var db = new BookStoreDbContext();
            if (db.Customers.Any(c => c.Username == username))
            {
                Console.WriteLine("Користувач з таким іменем вже існує.");
                return;
            }

            var c = new Customer { Username = username, Password = password, FirstName = first, LastName = last, Email = email, Phone = phone };
            db.Customers.Add(c);
            db.SaveChanges();
            Console.WriteLine($"Зареєстровано користувача з id={c.Id}");
        }

        public static Customer Login()
        {
            Console.WriteLine("-- Вхід --");
            Console.Write("Ім'я користувача: "); var username = Console.ReadLine();
            Console.Write("Пароль: "); var password = Console.ReadLine();

            using var db = new BookStoreDbContext();
            var user = db.Customers.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                Console.WriteLine("Невірні облікові дані");
                return null;
            }

            Console.WriteLine($"Ласкаво просимо, {user.FirstName} {user.LastName} (id={user.Id})");
            return user;
        }
    }
}
