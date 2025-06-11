using Microsoft.EntityFrameworkCore;
using UserCrud.Models.Dto;

namespace UserCrud.Models
{
    public class UserdbContext : DbContext
    {
        public UserdbContext() { }

        public UserdbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
