namespace EmployeeDirectory.Tests.Infrastructure
{
    using EmployeeDirectory.Infrastructure;
    using Shouldly;
    using static Testing;

    public class PasswordServiceTests
    {
        public void ShouldHashAndVerifyPasswords()
        {
            var password = SamplePassword();
            var invalidPassword = SamplePassword();

            var hashedPassword = PasswordService.HashPassword(password);

            hashedPassword.ShouldNotBe(password);
            PasswordService.Verify(password, hashedPassword).ShouldBeTrue();
            PasswordService.Verify(invalidPassword, hashedPassword).ShouldBeFalse();
        }
    }
}
