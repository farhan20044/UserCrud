using UserCrud.Models;
using UserCrud.Helpers;
using UserCrud.Models.Dto;

namespace UserCrud.Services
{
    public interface IUserService
    {
        ApiResponse<List<UserDto>> GetAllUsers();
        ApiResponse<UserDto> GetUserById(int id);
        ApiResponse<UserDto> AddUser(CreateUserDto userDto);
        ApiResponse<UserDto> UpdateUser(int id, UpdateUserDto userDto);
        ApiResponse<string> DeleteUser(int id);
    }
}
