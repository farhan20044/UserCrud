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

        //Email Validation 
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
        //Phone numeber Validation 
        private bool ValidatePhoneNumber(string phoneNumber, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit))
            {
                error = ErrorMessages.InvalidPhoneFormat;
                return false;
            }
            return true;
        }

        //Get list of All Users
        public List<UserDto> GetAllUsers()
        {
            return _mapper.Map<List<UserDto>>(users);
        }
        //Get users by Id
        public UserDto? GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        //Add new User
        public UserDto? AddUser(CreateUserDto userDto, out string? error)
        {
            error = null;
            //First Validate Email
            if (!ValidateEmail(userDto.Email, out string emailError))
            {   
                error = emailError;
                return null;
            }
            //Check if Email is Duplicated 
            if (users.Any(u => u.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase)))
            {
                error = ErrorMessages.DuplicateEmail;
                return null;
            }

            if (!ValidatePhoneNumber(userDto.PhoneNumber, out string phoneError))
            {
                error = phoneError;
                return null;
            }
            if (users.Any(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                error = ErrorMessages.DuplicatedPhoneNumber;
                return null;
            }

            //Map to UserDto
            var user = _mapper.Map<User>(userDto);
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            return _mapper.Map<UserDto>(user);
        }
        //Update User
        public UserDto? UpdateUser(int id, UpdateUserDto userDto, out string? error)
        {
            error = null;
            //Validate Email
            if (!ValidateEmail(userDto.Email, out string emailError))
            {
                error = emailError;
                return null;
            }
            if (!ValidatePhoneNumber(userDto.PhoneNumber, out string phoneError))
            {
                error = phoneError;
                return null;
            }

            //Find user by Id to update 
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                error = ErrorMessages.UserNotFound;
                return null;
            }
            //Check if Email is Duplicated 
            if (!user.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase) &&
                users.Any(u => u.Email.Equals(userDto.Email, System.StringComparison.OrdinalIgnoreCase)))
            {
                error = ErrorMessages.DuplicateEmail;
                return null;
            }
            if (!user.PhoneNumber.Equals(userDto.PhoneNumber) && users.Any(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                error = ErrorMessages.DuplicatedPhoneNumber;
                return null;
            }

            _mapper.Map(userDto, user);
            return _mapper.Map<UserDto>(user);
        }

        public bool DeleteUser(int id, out string? error)
        {
            error = null;
            //Find by id 
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                error = ErrorMessages.UserNotFound;
                return false;
            }
            //Remove from Users List 
            users.Remove(user);
            return true;
        }
    }
}
