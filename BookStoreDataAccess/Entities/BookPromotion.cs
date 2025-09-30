namespace BookStoreDataAccess.Entities
{
    public class BookPromotion
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; }
    }
}
