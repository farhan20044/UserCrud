using System.ComponentModel.DataAnnotations;
using UserCrud.Helpers;

namespace UserCrud.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required(ErrorMessage = ErrorMessages.PhoneRequired)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = ErrorMessages.InvalidPhoneFormat)]
        public required string PhoneNumber { get; set; }
    }
} 