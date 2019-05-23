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
                    "'Email' should not be empty.",
                    "'Initial Password' should not be empty.",
                    "'Confirm Initial Password' should not be empty.",
                    "'First Name' should not be empty.",
                    "'Last Name' should not be empty.",
                    "'Title' should not be empty.",
                    "'Office' should not be empty.");
        }

        public void ShouldRequirePasswordConfirmationMatchesInitialPassword()
        {
            var command = new RegisterEmployee.Command
            {
                Email = SampleEmail(),
                FirstName = SampleFirstName(),
                LastName = SampleLastName(),
                Title = SampleTitle(),
                Office = Sample<Office>(),
                PhoneNumber = SamplePhoneNumber()
            };

            command.Password = null;
            command.ConfirmPassword = "abc123";
            command.ShouldNotValidate("'Initial Password' should not be empty.");

            command.Password = "abc123";
            command.ConfirmPassword = null;
            command.ShouldNotValidate("'Confirm Initial Password' should not be empty.");

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
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            };

            command.ShouldNotValidate("'Email' should not be empty.");

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
                Title = "Junior Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-9999"
            });
        }
    }
}