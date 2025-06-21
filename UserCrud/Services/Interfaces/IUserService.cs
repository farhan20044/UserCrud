using UserCrud.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserCrud.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto?> GetUserById(string id);
        Task<UserCreationResultDto?> AddUser(CreateUserDto userDto);
        Task<UserCreationResultDto?> UpdateUser(string id, CreateUserDto userDto);
        Task<bool> DeleteUser(string id);
        Task<PagedResult<UserDto>> GetUsersPaged(int pageNumber, int pageSize, string? filter, string? sort);
    }
} 