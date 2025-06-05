using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;
using System.Linq;

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
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
        //Get User by Id
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
        //Post User
        [HttpPost]
        public IActionResult AddUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<string>.FailureResponse(string.Join(", ", errors)));
            }

            try
            {
                var user = _userService.AddUser(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                    ApiResponse<UserDto>.SuccessResponse(user, ErrorMessages.UserCreated));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
        // Update User
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<string>.FailureResponse(string.Join(", ", errors)));
            }

            try
            {
                var user = _userService.UpdateUser(id, userDto);
                return Ok(ApiResponse<UserDto>.SuccessResponse(user, ErrorMessages.UserUpdated));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
        //Delete User
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok(ApiResponse<string>.SuccessResponse(null, ErrorMessages.UserDeleted));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }
    }
}
