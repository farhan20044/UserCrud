using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UserCrud.Models;
using UserCrud.Helpers;
using UserCrud.Services;

namespace UserCrud.Services
{
    public class UserService : IUserService
    {
        private static readonly List<User> users = new();
        //error message
        private static class ErrorMessages
        {
            public const string UserNotFound = "User not found";
            public const string DuplicateEmail = "Email already exists";
            public const string UserDeleted = "User deleted successfully";
            public const string InvalidEmailFormat = "Invalid email format";
            public const string InvalidEmailDomain = "Email domain must be one of the following: .com, .net, .org, .co, .pk";
            public const string NoAlphanumericCharacters = "Email must contain at least one alphanumeric character before @";
        }
        //domians
        private static readonly string[] AllowedDomains = { ".com", ".net", ".org", ".co", ".pk" };
        //validate email 
        private bool ValidateEmail(string email, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(email))
            {
                error = ErrorMessages.InvalidEmailFormat;
                return false;
            }

            if (!new EmailAddressAttribute().IsValid(email))
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

        public ApiResponse<List<User>> GetAllUsers()
        {
            return ApiResponse<List<User>>.SuccessResponse(users);
        }

        public ApiResponse<User> GetUserById(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            return user == null
                ? ApiResponse<User>.FailureResponse(ErrorMessages.UserNotFound)
                : ApiResponse<User>.SuccessResponse(user);
        }

        public ApiResponse<User> AddUser(User user)
        {
            if (!ValidateEmail(user.Email, out string emailError))
                return ApiResponse<User>.FailureResponse(emailError);

            if (users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<User>.FailureResponse(ErrorMessages.DuplicateEmail);

            user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
            users.Add(user);
            return ApiResponse<User>.SuccessResponse(user);
        }

        public ApiResponse<User> UpdateUser(int id, User updatedUser)
        {
            if (!ValidateEmail(updatedUser.Email, out string emailError))
                return ApiResponse<User>.FailureResponse(emailError);

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return ApiResponse<User>.FailureResponse(ErrorMessages.UserNotFound);

            if (!user.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) &&
                users.Any(u => u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return ApiResponse<User>.FailureResponse(ErrorMessages.DuplicateEmail);
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            return ApiResponse<User>.SuccessResponse(user);
        }

        public ApiResponse<string> DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound);

            users.Remove(user);

            for (int i = 0; i < users.Count; i++)
                users[i].Id = i + 1;

            return ApiResponse<string>.SuccessResponse(null, ErrorMessages.UserDeleted);
        }
    }
}
