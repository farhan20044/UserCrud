using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services.Interfaces;
using UserCrud.Repository.Interfaces;
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
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IMapper mapper, IRepository<ApplicationUser> userRepository, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        //Get list of All Users
        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
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
                var user = await _userRepository.GetByIdAsync(id);
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

                var result = await _userManager.CreateAsync(user, userDto.Password);
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
                var user = await _userRepository.GetByIdAsync(id);
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

                var updatedUser = await _userRepository.UpdateAsync(user);
                return _mapper.Map<UserDto>(updatedUser);
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
                return await _userRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResult<UserDto>> GetUsersPaged(int pageNumber, int pageSize, string? search, string? sortBy, string? sortOrder)
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(pageNumber, pageSize, search, sortBy, sortOrder);
            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(users),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}