using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using AutoMapper;
using System;

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

        //Domain Validation 
        private void ValidateEmailDomain(string email)
        {
            var domain = email.Substring(email.LastIndexOf('@')).ToLower();
            if (!Domains.AllowedDomains.Any(d => domain.EndsWith(d)))
            {
                throw new ArgumentException(ErrorMessages.InvalidEmailDomain);
            }
        }

        private bool IsEmailDuplicate(string email, string? currentEmail = null)
        {
            return users.Any(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                (currentEmail == null || !u.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase)));
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
            if (user == null)
            {
                throw new KeyNotFoundException(ErrorMessages.UserNotFound);
            }
            return _mapper.Map<UserDto>(user);
        }

        //Add new User
        public UserDto? AddUser(CreateUserDto userDto)
        {
            //Validate Email Domain
            ValidateEmailDomain(userDto.Email);

            //Check if Email is Duplicated 
            if (IsEmailDuplicate(userDto.Email))
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
            }

            //Map to UserDto
            var user = _mapper.Map<User>(userDto);
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            return _mapper.Map<UserDto>(user);
        }

        //Update User
        public UserDto? UpdateUser(int id, UpdateUserDto userDto)
        {
            //Validate Email Domain
            ValidateEmailDomain(userDto.Email);

            //Find user by Id to update 
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException(ErrorMessages.UserNotFound);
            }

            //Check if Email is Duplicated 
            if (IsEmailDuplicate(userDto.Email, user.Email))
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
            }

            if (!user.PhoneNumber.Equals(userDto.PhoneNumber) && users.Any(u => u.PhoneNumber == userDto.PhoneNumber))
            {
                throw new InvalidOperationException(ErrorMessages.DuplicatedPhoneNumber);
            }

            _mapper.Map(userDto, user);
            return _mapper.Map<UserDto>(user);
        }

        public bool DeleteUser(int id)
        {
            //Find by id 
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException(ErrorMessages.UserNotFound);
            }
            //Remove from Users List 
            users.Remove(user);
            return true;
        }
    }
}
