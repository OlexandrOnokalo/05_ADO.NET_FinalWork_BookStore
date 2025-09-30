using System;

namespace BookStore.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Книгарня (консольний додаток) ===\n");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Головне меню:");
                Console.WriteLine("1) Реєстрація");
                Console.WriteLine("2) Вхід");
                Console.WriteLine("3) Книги");
                Console.WriteLine("4) Продажі");
                Console.WriteLine("5) Резервування");
                Console.WriteLine("6) Акції");
                Console.WriteLine("7) Звіти");
                Console.WriteLine("0) Вихід");
                Console.Write("Вибір: ");
                var key = Console.ReadLine();

                switch (key)
                {
                    case "1": AuthUI.Register(); break;
                    case "2": AuthUI.Login(); break;
                    case "3": BooksUI.Menu(); break;
                    case "4": SalesUI.Menu(); break;
                    case "5": ReservationsUI.Menu(); break;
                    case "6": PromotionsUI.Menu(); break;
                    case "7": ReportsUI.Menu(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Невідомий варіант\n"); break;
                }

                Console.WriteLine();
            }

            Console.WriteLine("До побачення!");
        }
    }
}
