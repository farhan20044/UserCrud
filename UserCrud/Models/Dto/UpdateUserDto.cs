using System.ComponentModel.DataAnnotations;
using UserCrud.Helpers;
namespace UserCrud.Models.Dto
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = ErrorMessages.NameRequired)]
        [StringLength(12, MinimumLength = 8, ErrorMessage = ErrorMessages.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.EmailRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailFormat)]
        public string Email { get; set; } = string.Empty;
    }
}
