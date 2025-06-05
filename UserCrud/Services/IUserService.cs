using UserCrud.Models.Dto;
using System.Collections.Generic;

namespace UserCrud.Services
{
    public interface IUserService
    {
        List<UserDto> GetAllUsers();
        UserDto? GetUserById(int id);
        UserDto? AddUser(CreateUserDto userDto);
        UserDto? UpdateUser(int id, UpdateUserDto userDto);
        bool DeleteUser(int id);
    }
}
