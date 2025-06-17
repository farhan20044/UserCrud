using System.ComponentModel.DataAnnotations;
using UserCrud.Helpers;
namespace UserCrud.Models.Dto
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = ErrorMessages.NameRequired)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ErrorMessages.NameLength)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = ErrorMessages.NameRequired)]
        [StringLength(50, MinimumLength = 2, ErrorMessage = ErrorMessages.NameLength)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = ErrorMessages.EmailRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.InvalidEmailFormat)]
        public required string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.PhoneRequired)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = ErrorMessages.InvalidPhoneFormat)]
        public required string PhoneNumber { get; set; }
    }
}
