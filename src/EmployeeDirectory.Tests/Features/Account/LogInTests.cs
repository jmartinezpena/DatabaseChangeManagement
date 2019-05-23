namespace EmployeeDirectory.Tests.Features.Account
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Account;
    using EmployeeDirectory.Infrastructure;
    using Infrastructure;
    using Should;
    using static Testing;

    public class LogInTests
    {
        private const string LoginErrorMessage =
            "The email address or password was invalid. Please try again.";

        public void ShouldRequireEmailAndPassword()
        {
            new LogIn.Command { Email = null, Password = null }
                .ShouldNotValidate(LoginErrorMessage);

            new LogIn.Command { Email = null, Password = "password" }
                .ShouldNotValidate(LoginErrorMessage);

            new LogIn.Command { Email = SampleEmail(), Password = null }
                .ShouldNotValidate(LoginErrorMessage);
        }

        public void ShouldRequireEmailExists()
        {
            new LogIn.Command { Email = SampleEmail(), Password = "password" }
                .ShouldNotValidate(LoginErrorMessage);
        }

        public async Task ShouldRequirePasswordVerificationAgainstHashedPassword()
        {
            var email = SampleEmail();
            var password = SamplePassword();
            var invalidPassword = password + "typeo";

            await Register(x =>
            {
                x.Email = email;
                x.Password = password;
                x.ConfirmPassword = password;
            });

            new LogIn.Command { Email = email, Password = invalidPassword }
                .ShouldNotValidate(LoginErrorMessage);

            new LogIn.Command { Email = email, Password = password }
                .ShouldValidate();
        }

        public async Task ShouldAuthenticateUponValidLogin()
        {
            var email = SampleEmail();
            var password = SamplePassword();

            await Register(x =>
            {
                x.Email = email;
                x.Password = password;
                x.ConfirmPassword = password;
            });

            await Send(new LogIn.Command { Email = email, Password = password });

            Scoped<ILoginService>(loginService =>
            {
                ((StubLoginService)loginService)
                    .AuthenticatedEmail.ShouldEqual(email);
            });
        }
    }
}