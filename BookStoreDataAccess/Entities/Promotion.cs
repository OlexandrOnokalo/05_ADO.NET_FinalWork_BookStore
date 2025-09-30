using System;
using System.Collections.Generic;

namespace BookStoreDataAccess.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? GenreId { get; set; }
        public Genre Genre { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<BookPromotion> BookPromotions { get; set; } = new List<BookPromotion>();
    }
}
