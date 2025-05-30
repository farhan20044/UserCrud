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
    }
}
