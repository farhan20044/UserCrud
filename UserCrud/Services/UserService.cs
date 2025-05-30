using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UserCrud.Models;
using UserCrud.Helpers;
using UserCrud.Models.Dto;
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


        private static readonly string[] AllowedDomains = { ".com", ".net", ".org", ".co", ".pk" };

        private bool ValidateEmail(string email, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                error = ErrorMessages.InvalidEmailFormat;
                return false;
            }

            string localPart = email.Split('@')[0];
            if (!Regex.IsMatch(localPart, @"[a-zA-Z0-9]"))
            {
                error = ErrorMessages.NoAlphanumericCharacters;
                return false;
            }

            var domain = email.Substring(email.LastIndexOf('@')).ToLower();
            if (!AllowedDomains.Any(d => domain.EndsWith(d)))
            {
                error = ErrorMessages.InvalidEmailDomain;
                return false;
            }

            return true;
        }

        public ApiResponse<List<UserDto>> GetAllUsers()
        {
            var dtoList = _mapper.Map<List<UserDto>>(users);
            return ApiResponse<List<UserDto>>.SuccessResponse(dtoList);
        }

        public ApiResponse<UserDto> GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            return user == null
                ? ApiResponse<UserDto>.FailureResponse(ErrorMessages.UserNotFound)
                : ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user));
        }

        public ApiResponse<UserDto> AddUser(CreateUserDto userDto)
        {
            if (!ValidateEmail(userDto.Email, out string emailError))
                return ApiResponse<UserDto>.FailureResponse(emailError);

            if (users.Any(u => u.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<UserDto>.FailureResponse(ErrorMessages.DuplicateEmail);

            var user = _mapper.Map<User>(userDto);
            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);

            return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user));
        }

        public ApiResponse<UserDto> UpdateUser(int id, UpdateUserDto userDto)
        {
            if (!ValidateEmail(userDto.Email, out string emailError))
                return ApiResponse<UserDto>.FailureResponse(emailError);

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse(ErrorMessages.UserNotFound);

            if (!user.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase) &&
                users.Any(u => u.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return ApiResponse<UserDto>.FailureResponse(ErrorMessages.DuplicateEmail);
            }

            _mapper.Map(userDto, user);
            return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user));
        }

        public ApiResponse<string> DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound);

            users.Remove(user);
            for (int i = 0; i < users.Count; i++) users[i].Id = i + 1;

            return ApiResponse<string>.SuccessResponse(null, ErrorMessages.UserDeleted);
        }
    }
}
