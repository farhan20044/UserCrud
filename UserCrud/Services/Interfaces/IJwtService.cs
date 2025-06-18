using UserCrud.Models.Dto;

namespace UserCrud.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
} 