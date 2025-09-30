using System;

namespace BookStoreDataAccess.Entities
{
    public class WriteOff
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}
