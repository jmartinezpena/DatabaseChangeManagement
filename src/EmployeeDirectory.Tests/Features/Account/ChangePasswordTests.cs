namespace EmployeeDirectory.Tests.Features.Account
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Account;
    using EmployeeDirectory.Infrastructure;
    using Should;
    using static Testing;
    using Employee = EmployeeDirectory.Model.Employee;

    public class ChangePasswordTests
    {
        public void ShouldRequireAllInputFields()
        {
            new ChangePassword.Command()
                .ShouldNotValidate(
                    "'Current Password' should not be empty.",
                    "'New Password' should not be empty.",
                    "'Confirm Password' should not be empty.");
        }

        public async Task ShouldRequireValidCurrentPasswordToAuthorizePasswordChange()
        {
            var currentPassword = SamplePassword();
            await LogIn(currentPassword);

            var typeo = new ChangePassword.Command
            {
                CurrentPassword = "current password typo",
                NewPassword = "abc123",
                ConfirmPassword = "abc123"
            };

            var valid = new ChangePassword.Command
            {
                CurrentPassword = currentPassword,
                NewPassword = "abc123",
                ConfirmPassword = "abc123"
            };

            typeo.ShouldNotValidate("'Current Password' was invalid. Please enter your current password and try again.");
            valid.ShouldValidate();
        }

        public async Task ShouldRequirePasswordConfirmationMatchesNewPassword()
        {
            var currentPassword = SamplePassword();
            await LogIn(currentPassword);

            var command = new ChangePassword.Command
            {
                CurrentPassword = currentPassword
            };

            command.NewPassword = null;
            command.ConfirmPassword = "abc123";
            command.ShouldNotValidate("'New Password' should not be empty.");

            command.NewPassword = "abc123";
            command.ConfirmPassword = null;
            command.ShouldNotValidate("'Confirm Password' should not be empty.");

            command.NewPassword = "abc123";
            command.ConfirmPassword = "abc123";
            command.ShouldValidate();

            command.NewPassword = "abc123";
            command.ConfirmPassword = "abc123 typo";
            command.ShouldNotValidate("These passwords do not match. Be sure to enter the same password twice.");
        }

        public async Task ShouldUpdateHashedPasswordForTheCurrentlyLoggedInUser()
        {
            var currentPassword = SamplePassword();
            var employee = await LogIn(currentPassword);

            await Send(new ChangePassword.Command
            {
                CurrentPassword = currentPassword,
                NewPassword = "abc123",
                ConfirmPassword = "abc123"
            });

            var actual = Query<Employee>(employee.Id);

            PasswordService.Verify("abc123", actual.HashedPassword).ShouldBeTrue();
        }

        private static async Task<Employee> LogIn(string currentPassword)
        {
            var email = SampleEmail();

            var employee = await Register(x =>
            {
                x.Email = email;
                x.Password = currentPassword;
                x.ConfirmPassword = currentPassword;
            });

            await Send(new LogIn.Command { Email = email, Password = currentPassword });

            return employee;
        }
    }
}