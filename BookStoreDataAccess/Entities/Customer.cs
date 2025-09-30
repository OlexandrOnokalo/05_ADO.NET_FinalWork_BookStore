using System.Collections.Generic;

namespace BookStoreDataAccess.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal TotalSpent { get; set; } = 0m;
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
