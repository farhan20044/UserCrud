using UserCrud.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserCrud.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(string id);
        Task<UserDto?> AddUser(CreateUserDto userDto);
        Task<UserDto?> UpdateUser(string id, CreateUserDto userDto);
        Task<bool> DeleteUser(string id);
    }
}
