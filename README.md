# BookStore (консольний додаток) - Final
Проєкт містить два підпроєкти:
- BookStoreDataAccess — Class Library з EF Core DbContext та сутностями (Code First, FluentAPI)
- BookStore.ConsoleApp — консольний інтерфейс

## Інструкція
1. Розпакувати архів.
2. Відкрити `BookStore.sln` у Visual Studio або запустити з `dotnet`.
3. Якщо база `BookStore` відсутня — створити її за допомогою міграцій або дозволити створення (EnsureCreated):
   - `dotnet ef migrations add InitialCreate -p BookStoreDataAccess -s BookStore.ConsoleApp`
   - `dotnet ef database update -p BookStoreDataAccess -s BookStore.ConsoleApp`
4. Рядок підключення знаходиться в `BookStoreDbContext.OnConfiguring`. За замовчуванням вказано інстанс `PULSE\SQLEXPRESS`.

## Зміни у цій версії
- Видалено "(soft)" з виводів меню.
- Перед редагуванням/видаленням книги виводиться список книг для вибору id.
- Повідомлення при видаленні: "Книга успішно видалена".
- Додано пункт "Показати список книг" у меню книг.
- Додано пункт "Показати список резервацій" у меню резервацій.
- При виборі резервації для отримання/скасування спочатку виводиться список резервацій.
