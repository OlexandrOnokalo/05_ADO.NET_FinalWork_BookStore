using System;
using System.Collections.Generic;

namespace BookStoreDataAccess.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public string Note { get; set; }
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
