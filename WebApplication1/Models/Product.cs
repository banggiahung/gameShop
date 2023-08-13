using MessagePack;

namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? MainImg { get; set; }
        public string? LinkDown { get; set; }
        public string? LinkDownDrop { get; set; }
        public string? LinkDownMedia { get; set; }
        public string? DetailsGame { get; set; }
        public string? DesShort { get; set; }
        public string? RAM { get; set; }
        public string? GB { get; set; }
        public string? Language { get; set; }
        public string? CPU { get; set; }
        public string? Part { get; set; }
        public int? CateId { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
