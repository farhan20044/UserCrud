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
        private readonly IEmailService _emailService;
        private readonly UserdbContext _context;

        public UserService(UserdbContext context, IMapper mapper, IRepository<ApplicationUser> userRepository, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _emailService = emailService;
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
        public async Task<UserCreationResultDto?> AddUser(CreateUserDto userDto)
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

                // Generate email confirmation token and send email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                return new UserCreationResultDto
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = token
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update User
        public async Task<UserCreationResultDto?> UpdateUser(string id, CreateUserDto userDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                var existingUserWithEmail = await _userManager.FindByEmailAsync(userDto.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                string? token = null;
                bool emailChanged = user.Email != userDto.Email;

                if (emailChanged)
                {
                    user.Email = userDto.Email;
                    user.UserName = userDto.Email;
                    user.EmailConfirmed = false;
                }
                
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.PhoneNumber = userDto.PhoneNumber;

                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, passwordToken, userDto.Password);
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                if (emailChanged)
                {
                    token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                }

                await _context.SaveChangesAsync();

                return new UserCreationResultDto
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = token
                };
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
                var result = await _userRepository.DeleteAsync(id);
                if (result)
                {
                    await _context.SaveChangesAsync();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResult<UserDto>> GetUsersPaged(int pageNumber, int pageSize, string? filter, string? sort)
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(pageNumber, pageSize, filter, sort);
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