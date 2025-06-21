using System.ComponentModel.DataAnnotations;

namespace UserCrud.Models.Dto
{
    public class ConfirmEmailDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
} 