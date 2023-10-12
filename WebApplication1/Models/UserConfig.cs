namespace WebApplication1.Models
{
    public class UserConfig
    {
        public int ID { get; set; }
        public string? UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? Total { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsAcitive { get; set; }


    }
}
