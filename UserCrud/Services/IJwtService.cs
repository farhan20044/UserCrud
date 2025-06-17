using UserCrud.Models;

namespace UserCrud.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
} 