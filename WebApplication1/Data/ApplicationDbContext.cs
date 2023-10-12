using WebApplication1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubMenu> SubMenu { get; set; }
        public DbSet<NotiWeb> NotiWeb { get; set; }
        public DbSet<MainView> MainView { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<ConfigLink> ConfigLink { get; set; }
        public DbSet<UserConfig> UserConfig { get; set; }
        public DbSet<BankConfig> BankConfig { get; set; }
        public DbSet<History> History { get; set; }




    }
}
