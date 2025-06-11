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

namespace UserCrud.Services
{
    public class UserService : IUserService
    {
        //private static readonly List<User> users = new();
        private readonly IMapper _mapper;
        private readonly UserdbContext _userdb;

        public UserService(IMapper mapper, UserdbContext userdb)
        {
            _mapper = mapper;
            _userdb = userdb;
        }

        private async Task<bool> IsEmailDuplicate(string email, string? currentEmail = null)
        {
            var existingUser = await _userdb.Users
                .FirstOrDefaultAsync(u => 
                    u.Email.ToLower() == email.ToLower() &&
                    (currentEmail == null || u.Email.ToLower() != currentEmail.ToLower()));

            return existingUser != null;

        }

        //Get list of All Users
        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _userdb.Users.ToListAsync();
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get users by Id
        public async Task<UserDto?> GetUserById(int id)
        {
            try
            {
                var user = await _userdb.Users.FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<UserDto?> AddUser(CreateUserDto userDto)
        {
            try
            {
                if (await IsEmailDuplicate(userDto.Email))
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                var user = _mapper.Map<User>(userDto);
                
                await _userdb.Users.AddAsync(user);
                await _userdb.SaveChangesAsync();
                
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
        public async Task<UserDto?> UpdateUser(int id, CreateUserDto userDto)
        {
            try
            {
                var user = await _userdb.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                if (await IsEmailDuplicate(userDto.Email, user.Email))
                {
                    throw new InvalidOperationException(ErrorMessages.DuplicateEmail);
                }

                _mapper.Map(userDto, user);
                await _userdb.SaveChangesAsync();

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

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await _userdb.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException(ErrorMessages.UserNotFound);
                }

                _userdb.Users.Remove(user);
                await _userdb.SaveChangesAsync();

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