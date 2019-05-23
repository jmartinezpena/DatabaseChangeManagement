namespace EmployeeDirectory.Infrastructure
{
    public static class PasswordService
    {
        public static string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public static bool Verify(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}