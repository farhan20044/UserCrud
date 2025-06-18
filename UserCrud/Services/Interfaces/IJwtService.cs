using UserCrud.Models;

namespace UserCrud.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
} 