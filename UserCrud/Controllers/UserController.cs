using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserCrud.Models;
using System.Threading.Tasks;
using UserCrud.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace UserCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserService userService, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _emailService = emailService;
            _userManager = userManager;
        }

        //Get All Users with Pagination, Search, and Sorting
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            int pageNumber = 1,
            int pageSize = 10,
            string? filter = null,
            string? sort = null)
        {
            try
            {
                var pagedResult = await _userService.GetUsersPaged(pageNumber, pageSize, filter, sort);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //Get User by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Post User
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.AddUser(userDto);
                if (result?.User == null || result.Token == null)
                {
                    return BadRequest(ErrorMessages.UserCreationError);
                }
                var confirmationLink = EmailConfirmationHelper.GenerateEmailConfirmationLink(this, result.User, result.Token);
                await _emailService.SendEmailConfirmationAsync(result.User.Email, confirmationLink);
                return Ok(ErrorMessages.UserCreatedWithConfirmationLink);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // Update User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.UpdateUser(id, userDto);
                if (result?.User == null)
                {
                    return BadRequest(new Exception(ErrorMessages.UserUpdateFailed));
                }

                if (!string.IsNullOrEmpty(result.Token))
                {
                    var confirmationLink = EmailConfirmationHelper.GenerateEmailConfirmationLink(this, result.User, result.Token);
                    await _emailService.SendEmailConfirmationAsync(result.User.Email, confirmationLink);
                    return Ok(result.User, ErrorMessages.UserCreatedWithConfirmationLink);
                }
                
                return Ok(result.User, ErrorMessages.UserUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return Ok(ErrorMessages.UserDeleted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
 
    }
}
