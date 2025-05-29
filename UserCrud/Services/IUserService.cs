using UserCrud.Models;
using UserCrud.Helpers;

namespace UserCrud.Services
{
    public interface IUserService
    {
        ApiResponse<List<User>> GetAllUsers();
        ApiResponse<User> GetUserById(int id);
        ApiResponse<User> AddUser(User user);
        ApiResponse<User> UpdateUser(int id, User updatedUser);
        ApiResponse<string> DeleteUser(int id);
    }
}
