namespace EmployeeDirectory.Tests.Features.Employee
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Employee;
    using Shouldly;
    using static EmployeeDirectory.Infrastructure.PasswordService;
    using static Testing;

    public class RegisterEmployeeTests
    {
        public void ShouldRequireMinimumFields()
        {
            new RegisterEmployee.Command()
                .ShouldNotValidate(
                    "'Email' must not be empty.",
                    "'Initial Password' must not be empty.",
                    "'Confirm Initial Password' must not be empty.",
                    "'First Name' must not be empty.",
                    "'Last Name' must not be empty.",
                    "'Sport Team' must not be empty.",
                    "'Title' must not be empty.",
                    "'Office' must not be empty.");
        }

        public void ShouldRequirePasswordConfirmationMatchesInitialPassword()
        {
            var command = new RegisterEmployee.Command
            {
                Email = SampleEmail(),
                FirstName = SampleFirstName(),
                LastName = SampleLastName(),
                SportTeam = SampleSportTeam(),
                Title = SampleTitle(),
                Office = Sample<Office>(),
                PhoneNumber = SamplePhoneNumber()
            };

            command.Password = null;
            command.ConfirmPassword = "abc123";
            command.ShouldNotValidate("'Initial Password' must not be empty.");

            command.Password = "abc123";
            command.ConfirmPassword = null;
            command.ShouldNotValidate("'Confirm Initial Password' must not be empty.");

            command.Password = "abc123";
            command.ConfirmPassword = "abc123";
            command.ShouldValidate();

            command.Password = "abc123";
            command.ConfirmPassword = "abc123 typo";
            command.ShouldNotValidate("These passwords do not match. Be sure to enter the same password twice.");
        }

        public void ShouldRequireValidEmailAddress()
        {
            var password = SamplePassword();
            var command = new RegisterEmployee.Command
            {
                Password = password,
                ConfirmPassword = password,
                FirstName = "John",
                LastName = "Smith",
                SportTeam = "Monterrey",
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            };

            command.ShouldNotValidate("'Email' must not be empty.");

            command.Email = SampleEmail();
            command.ShouldValidate();

            command.Email = "test at example dot com";
            command.ShouldNotValidate("'Email' is not a valid email address.");
        }

        public async Task ShouldRequireUniqueEmail()
        {
            var preexistingEmployee = await Register();

            var password = SamplePassword();
            var command = new RegisterEmployee.Command
            {
                Email = SampleEmail(),
                Password = password,
                ConfirmPassword = password,
                FirstName = "John",
                LastName = "Smith",
                SportTeam = "Tigres",
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            };

            command.ShouldValidate();

            command.Email = preexistingEmployee.Email;

            command.ShouldNotValidate(
                $"Another employee already uses '{command.Email}'. " +
                "Please enter a unique email address.");
        }

        public async Task ShouldRegisterNewEmployee()
        {
            var email = SampleEmail();
            var password = SamplePassword();
            var response = await Send(new RegisterEmployee.Command
            {
                Email = email,
                Password = password,
                ConfirmPassword = password,
                FirstName = "John",
                LastName = "Smith",
                SportTeam = "Monterrey",
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            });

            var actual = Query<Employee>(response.EmployeeId);
            Verify(password, actual.HashedPassword).ShouldBeTrue();

            actual.ShouldMatch(new Employee
            {
                Id = response.EmployeeId,
                HashedPassword = actual.HashedPassword,
                Email = email,
                FirstName = "John",
                LastName = "Smith",
                SportTeam = "Monterrey",
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            });
        }
    }
}