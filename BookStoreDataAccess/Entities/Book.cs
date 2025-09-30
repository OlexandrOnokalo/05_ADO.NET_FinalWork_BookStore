using System;
using System.Collections.Generic;

namespace BookStoreDataAccess.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int? PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int Pages { get; set; }
        public int Year { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int? ParentBookId { get; set; }
        public Book ParentBook { get; set; }
        public ICollection<Book> Continuations { get; set; } = new List<Book>();
        public int Stock { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<BookPromotion> BookPromotions { get; set; } = new List<BookPromotion>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<WriteOff> WriteOffs { get; set; } = new List<WriteOff>();
    }
}
