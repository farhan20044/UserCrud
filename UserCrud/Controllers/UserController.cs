using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserCrud.Models;
using System.Threading.Tasks;

namespace UserCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //Get All Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //Get User by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
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
                return BadRequest(ex);
            }
        }
        //Post User
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                // var errors = ModelState.Values
                //     .SelectMany(v => v.Errors)
                //     .Select(e => e.ErrorMessage)
                //     .ToList();
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userService.AddUser(userDto);
                return Ok(user, ErrorMessages.UserCreated);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // Update User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                // var errors = ModelState.Values
                //     .SelectMany(v => v.Errors)
                //     .Select(e => e.ErrorMessage)
                //     .ToList();
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.UpdateUser(id, userDto);
                return Ok(user, ErrorMessages.UserUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
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
