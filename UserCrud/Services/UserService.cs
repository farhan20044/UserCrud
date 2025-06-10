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

        private bool IsEmailDuplicate(string email, string? currentEmail = null)
        {
            return users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                (currentEmail == null || !u.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase)));

        }

        //Get list of All Users
        public List<UserDto> GetAllUsers()
        {
            try
            {
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get users by Id
        public UserDto? GetUserById(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Add new User
        public UserDto? AddUser(CreateUserDto userDto)
        {
            try
            {
                if (IsEmailDuplicate(userDto.Email))
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                var user = _mapper.Map<User>(userDto);
                user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
                users.Add(user);
                return _mapper.Map<UserDto>(user);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update User
        public UserDto? UpdateUser(int id, CreateUserDto userDto)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                if (IsEmailDuplicate(userDto.Email, user.Email))
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                _mapper.Map(userDto, user);
                return _mapper.Map<UserDto>(user);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }
                users.Remove(user);
                return true;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}