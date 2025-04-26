using Microsoft.EntityFrameworkCore;
using huan.Models;
namespace huan.Data
{
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Topic> Topics { get; set; }
    
}
}