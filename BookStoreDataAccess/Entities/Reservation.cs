using System;

namespace BookStoreDataAccess.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}
