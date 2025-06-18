using Microsoft.AspNetCore.Identity;
using UserCrud.Models;

namespace UserCrud.Repository.Interfaces
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null);
    }
} 