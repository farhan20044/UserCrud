using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using UserCrud.Helpers;
using System.Text.RegularExpressions;

namespace UserCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly List<User> users = new();

        // Error Messages
        private static class ErrorMessages
        {
            public const string UserNotFound = "User not found";
            public const string DuplicateEmail = "Email already exists";
            public const string UserDeleted = "User deleted successfully";
            public const string InvalidEmailFormat = "Invalid email format";
            public const string InvalidEmailDomain = "Email domain must be one of the following: .com, .net, .org, .co, .pk";
            public const string NoAlphanumericCharacters = "Email must contain at least one alphanumeric character before @";
            public const string ServerError = "An unexpected error occurred";
        }

        private static readonly string[] AllowedDomains = { ".com", ".net", ".org", ".co", ".pk" };

        //Pass error message to ModelState
        private void AddModelError(string errorMessage)
        {
            ModelState.AddModelError("errors", errorMessage);
            
        }
        //Validate Email Address
        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                AddModelError(ErrorMessages.InvalidEmailFormat);
                return false;
            }

            try
            {
                if (!new EmailAddressAttribute().IsValid(email))
                {
                    AddModelError(ErrorMessages.InvalidEmailFormat);
                    return false;
                }

                string localPart = email.Split('@')[0];
                if (!Regex.IsMatch(localPart, @"[a-zA-Z0-9]"))
                {
                    AddModelError(ErrorMessages.NoAlphanumericCharacters);
                    return false;
                }

                var domain = email.Substring(email.LastIndexOf('@')).ToLower();
                if (!AllowedDomains.Any(allowed => domain.EndsWith(allowed)))
                {
                    AddModelError(ErrorMessages.InvalidEmailDomain);
                    return false;
                }

                return true;
            }
            catch
            {
                AddModelError(ErrorMessages.InvalidEmailFormat);
                return false;
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
                }

                return Ok(ApiResponse<User>.SuccessResponse(user));
            }
            catch
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.ServerError));
            }
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(ApiResponse<List<User>>.SuccessResponse(users));
            }
            catch
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.ServerError));
            }
        }

        [HttpPost]
        public IActionResult AddUser(User newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid data"));

                if (!ValidateEmail(newUser.Email))
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid email"));

                if (users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return Conflict(ApiResponse<string>.FailureResponse(ErrorMessages.DuplicateEmail));
                }

                newUser.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
                users.Add(newUser);

                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, ApiResponse<User>.SuccessResponse(newUser));
            }
            catch
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.ServerError));
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid data"));

                if (!ValidateEmail(updatedUser.Email))
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalid email"));

                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
                }

                if (!user.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) &&
                    users.Any(u => u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    return Conflict(ApiResponse<string>.FailureResponse(ErrorMessages.DuplicateEmail));
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;

                return Ok(ApiResponse<User>.SuccessResponse(user));
            }
            catch
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.ServerError));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
                }

                users.Remove(user);

                // Renumber IDs
                for (int i = 0; i < users.Count; i++)
                    users[i].Id = i + 1;

                return Ok(ApiResponse<string>.SuccessResponse(null, ErrorMessages.UserDeleted));
            }
            catch
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.ServerError));
            }
        }
    }
}
