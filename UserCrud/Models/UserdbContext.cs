using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserCrud.Models.Dto;

namespace UserCrud.Models
{
    public class UserdbContext : IdentityDbContext<ApplicationUser>
    {
        public UserdbContext() { }

        public UserdbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
