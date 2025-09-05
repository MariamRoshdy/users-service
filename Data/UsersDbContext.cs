using UsersService.Models;
using Microsoft.EntityFrameworkCore;

namespace UsersService.Data
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        
    }
}