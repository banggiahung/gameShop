namespace WebApplication1.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? NameCustomer { get; set; }
        public string? CommentText { get; set; }
        public string? CommentByAdmin { get; set; }
        public int? productId { get; set; }
        public bool? isHeartAdmin { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? IpAddress { get; set; }

    }
}
