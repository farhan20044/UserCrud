using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using AutoMapper;

namespace UserCrud.Services
{
    public class UserService : IUserService
    {
        private static readonly List<User> users = new();
        private readonly IMapper _mapper;
        public UserService(IMapper mapper)
        {
            _mapper = mapper;
        }


        private bool ValidateEmail(string email, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                error = ErrorMessages.InvalidEmailFormat;
                return false;
            }
            var domain = email.Substring(email.LastIndexOf('@')).ToLower();
            if (!Domains.AllowedDomains.Any(d => domain.EndsWith(d)))
            {
                error = ErrorMessages.InvalidEmailDomain;
                return false;
            }
            return true;
        }

        public List<UserDto> GetAllUsers()
        {
            return _mapper.Map<List<UserDto>>(users);
        }

        public UserDto? GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public UserDto? AddUser(CreateUserDto userDto, out string? error)
        {
            error = null;
            if (!ValidateEmail(userDto.Email, out string emailError))
            {
                error = emailError;
                return null;
            }
            if (users.Any(u => u.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase)))
            {
                error = ErrorMessages.DuplicateEmail;
                return null;
            }
            var user = _mapper.Map<User>(userDto);
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            return _mapper.Map<UserDto>(user);
        }

        public UserDto? UpdateUser(int id, UpdateUserDto userDto, out string? error)
        {
            error = null;
            if (!ValidateEmail(userDto.Email, out string emailError))
            {
                error = emailError;
                return null;
            }
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                error = ErrorMessages.UserNotFound;
                return null;
            }
            if (!user.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase) &&
                users.Any(u => u.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase)))
            {
                error = ErrorMessages.DuplicateEmail;
                return null;
            }
            _mapper.Map(userDto, user);
            return _mapper.Map<UserDto>(user);
        }

        public bool DeleteUser(int id, out string? error)
        {
            error = null;
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                error = ErrorMessages.UserNotFound;
                return false;
            }
            users.Remove(user);
            return true;
        }
    }
}
