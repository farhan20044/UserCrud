namespace UserCrud.Helpers
{
    public static class ErrorMessages
    {
        public const string UserNotFound = "User not found";
        public const string DuplicateEmail = "Email already exists";
        public const string UserDeleted = "User deleted successfully";
        public const string InvalidEmailFormat = "Invalid email format";
        public const string InvalidEmailDomain = "Email domain must be one of the following: .com, .net, .org, .co, .pk";
        public const string NoAlphanumericCharacters = "Email must contain at least one alphanumeric character before @";
        public const string NameRequired = "Name is required";
        public const string NameLength = "Name must be between 8 and 12 characters";
        public const string EmailRequired = "Email is required";
        public const string EmailFormat = "Invalid email format";
        public const string UserCreated = "User Created Successfully";
        public const string UserRegistered = "User Registered Successfully";
        public const string UserUpdated = "User Updated Successfully";
        public const string PhoneRequired = "Phone number is required";
        public const string InvalidPhoneFormat = "Phone number must be exactly 11 digits";
        public const string DuplicatedPhoneNumber = "Phone number Already Exists";
        public const string JwtConfigurationMessage = "JWT settings are not configured properly";
        public const string InvalidEmailPass = "Invalid Username or Password"; 
        public const string EmailConfirmed = "Email confirmed and password set. You can now log in."; 
        public const string FailedinChangingPass = "Failed to remove old password."; 
        public const string InvalidUsers = "Invalid Users.";
        public const string ConfirmEmail = "Please confirm your email before logging in..";  
        public const string RegistrationSuccessfull = "Registration successful. Please check your email to confirm your account.";  
        public const string ExpiredToken = "Invalid or expired token.";  


    }
}
