using UserCrud.Models.Dto;

namespace UserCrud.Models.Dto
{
    public class UserCreationResultDto
    {
        public UserDto? User { get; set; }
        public string? Token { get; set; }
    }
} 