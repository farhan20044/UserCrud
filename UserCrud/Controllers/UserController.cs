using Microsoft.AspNetCore.Mvc;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;

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

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = _userService.GetAllUsers();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var result = _userService.GetUserById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid data"));

            var result = _userService.AddUser(userDto);
            return result.Success ? CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result) : Conflict(result);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid data"));

            var result = _userService.UpdateUser(id, userDto);
            return result.Success ? Ok(result) : Conflict(result);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var result = _userService.DeleteUser(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
