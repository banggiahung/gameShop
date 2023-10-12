namespace WebApplication1.Models
{
    public class History
    {
        public int ID { get; set; }
        public int? PriceUser { get; set; }
        public string? UserId { get; set; }
        public int? BankId { get; set; }
        public string? ContentTransit { get; set; }
        public bool? isDone { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
