using UserCrud.Models.Dto;
using System.Collections.Generic;

namespace UserCrud.Services
{
    public interface IUserService
    {
        List<UserDto> GetAllUsers();
        UserDto? GetUserById(int id);
        UserDto? AddUser(CreateUserDto userDto, out string? error);
        UserDto? UpdateUser(int id, UpdateUserDto userDto, out string? error);
        bool DeleteUser(int id, out string? error);
    }
}
