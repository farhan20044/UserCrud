using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserCrud.Services
{
    public class UserService : IUserService
    {
        //private static readonly List<User> users = new();
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        //Get list of All Users
        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get users by Id
        public async Task<UserDto?> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Add new User
        public async Task<UserDto?> AddUser(CreateUserDto userDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                var user = new ApplicationUser
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PhoneNumber = userDto.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update User
        public async Task<UserDto?> UpdateUser(string id, CreateUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                user.Email = userDto.Email;
                user.UserName = userDto.Email;
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.PhoneNumber = userDto.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}