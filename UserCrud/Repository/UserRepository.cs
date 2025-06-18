using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Repository.Interfaces;

namespace UserCrud.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserdbContext context, UserManager<ApplicationUser> userManager) 
            : base(context)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null || (excludeUserId != null && user.Id == excludeUserId);
        }

        public override async Task<ApplicationUser> AddAsync(ApplicationUser entity)
        {
            var result = await _userManager.CreateAsync(entity);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return entity;
        }

        public override async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
        {
            var result = await _userManager.UpdateAsync(entity);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return entity;
        }

        public override async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
} 