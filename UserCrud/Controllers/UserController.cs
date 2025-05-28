using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserCrud.Models;
using System.Text.RegularExpressions;
using System.Linq;

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

        // Valid email domains
        private static readonly string[] AllowedDomains = { ".com", ".net", ".org", ".co", ".pk" };
        private void AddModelError(string errorMessage)
        {
            ModelState.AddModelError("errors", errorMessage);
        }

        // Email validation method
        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                AddModelError(ErrorMessages.InvalidEmailFormat);
                return false;
            }

            try
            {
                // Check basic email format
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

                // Check for allowed domains
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
                    AddModelError(ErrorMessages.UserNotFound);
                    return NotFound(ModelState);
                }
                return Ok(user);
            }
            catch (Exception)
            {
                AddModelError(ErrorMessages.ServerError);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(users);
            }
            catch (Exception)
            {
                AddModelError(ErrorMessages.ServerError);
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public IActionResult AddUser(User newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!ValidateEmail(newUser.Email))
                    return BadRequest(ModelState);

                if (users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    AddModelError(ErrorMessages.DuplicateEmail);
                    return Conflict(ModelState);
                }

                newUser.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
                users.Add(newUser);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
            }
            catch (Exception)
            {
                AddModelError(ErrorMessages.ServerError);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!ValidateEmail(updatedUser.Email))
                    return BadRequest(ModelState);

                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    AddModelError(ErrorMessages.UserNotFound);
                    return NotFound(ModelState);
                }

                if (!user.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) &&
                    users.Any(u => u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    AddModelError(ErrorMessages.DuplicateEmail);
                    return Conflict(ModelState);
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                return Ok(user);
            }
            catch (Exception)
            {
                AddModelError(ErrorMessages.ServerError);
                return BadRequest(ModelState);
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
                    AddModelError(ErrorMessages.UserNotFound);
                    return NotFound(ModelState);
                }

                users.Remove(user);
                // renumbering Id
                for (int i = 0; i < users.Count; i++)
                    users[i].Id = i + 1;

                return Ok(ErrorMessages.UserDeleted);
            }
            catch (Exception)
            {
                AddModelError(ErrorMessages.ServerError);
                return BadRequest(ModelState);
            }
        }
    }
}