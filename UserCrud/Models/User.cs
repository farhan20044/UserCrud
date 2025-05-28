using System.ComponentModel.DataAnnotations;

namespace UserCrud.Models
{
    public class User
    {
        public static class ErrorMessages
        {
            public const string NameRequired = "Name is required";
            public const string NameLength = "Name must be between 8 and 12 characters";
            public const string EmailRequired = "Email is required";
            public const string EmailFormat = "Invalid email format";

        }

        public int Id { get; set; }

        [Required(ErrorMessage = ErrorMessages.NameRequired)]
        [StringLength(12, MinimumLength = 8, ErrorMessage = ErrorMessages.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.EmailRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailFormat)]
        public string Email { get; set; } = string.Empty;
    }
}