namespace BrainThrust.src.Services
{
    public static class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            
            // Ensure at least 8 characters, one uppercase, one lowercase, one digit, and one special character
            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));
            var isLongEnough = password.Length >= 8;

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && isLongEnough;
        }
    }

}