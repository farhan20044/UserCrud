using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;

namespace UserCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
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
                var response = ApiResponse<List<UserDto>>.SuccessResponse(users);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
            }
        }
        //Get User
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                if (user == null)
                    return NotFound(ApiResponse<UserDto>.FailureResponse(ErrorMessages.UserNotFound));

                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
            }
        }
        //Post User
        [HttpPost]
        public IActionResult AddUser([FromBody] CreateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.NameRequired));
                //Adding User to list 
                var user = _userService.AddUser(userDto, out string? error);

                if (user == null)
                    return Conflict(ApiResponse<string>.FailureResponse(error ?? ErrorMessages.DuplicateEmail));

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<UserDto>.SuccessResponse(user, ErrorMessages.UserCreated));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.DuplicateEmail));
            }
        }
        // Update User
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.NameRequired));

                var user = _userService.UpdateUser(id, userDto, out string? error);
                if (user == null)
                    return Conflict(ApiResponse<string>.FailureResponse(error ?? ErrorMessages.DuplicateEmail));

                return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.DuplicateEmail));
            }
        }
        //Delete User
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var deleted = _userService.DeleteUser(id, out string? error);
                if (!deleted)
                    return NotFound(ApiResponse<string>.FailureResponse(error ?? ErrorMessages.UserNotFound));

                return Ok(ApiResponse<string>.SuccessResponse(null, ErrorMessages.UserDeleted));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ErrorMessages.UserNotFound));
            }
        }
    }
}
