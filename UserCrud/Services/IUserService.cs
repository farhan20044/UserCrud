using UserCrud.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserCrud.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(int id);
        Task<UserDto?> AddUser(CreateUserDto userDto);
        Task<UserDto?> UpdateUser(int id, CreateUserDto userDto);
        Task<bool> DeleteUser(int id);
    }
}
