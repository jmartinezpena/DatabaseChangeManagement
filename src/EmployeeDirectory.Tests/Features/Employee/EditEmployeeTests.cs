namespace EmployeeDirectory.Tests.Features.Employee
{
    using System;
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Employee;
    using EmployeeDirectory.Infrastructure;
    using Infrastructure;
    using Shouldly;
    using static Testing;

    public class EditEmployeeTests
    {
        public async Task ShouldGetCurrentEmployeeDataById()
        {
            var email = SampleEmail();
            var selectedEmployee = await Register(x =>
            {
                x.Email = email;
                x.FirstName = "John";
                x.LastName = "Smith";
                x.SportTeam = "Cruz Azul";
                x.Title = "Senior Consultant";
                x.Office = Office.Austin;
                x.PhoneNumber = "555-123-0001";
            });

            var anotherEmployee = await Register();

            var result = await Send(new EditEmployee.Query { Id = selectedEmployee.Id });

            result.ShouldMatch(new EditEmployee.Command
            {
                Id = selectedEmployee.Id,
                Email = email,
                FirstName = "John",
                LastName = "Smith",
                SportTeam = "Cruz Azul",
                Title = "Senior Consultant",
                Office = Office.Austin,
                PhoneNumber = "555-123-0001"
            });
        }

        public void ShouldRequireMinimumFields()
        {
            new EditEmployee.Command()
                .ShouldNotValidate(
                    "'Email' must not be empty.",
                    "'First Name' must not be empty.",
                    "'Last Name' must not be empty.",
                    "'Sport Team' must not be empty.",
                    "'Title' must not be empty.",
                    "'Office' must not be empty.");
        }

        public void ShouldRequireValidEmailAddress()
        {
            var command = new EditEmployee.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "Patrick",
                LastName = "Jones",
                SportTeam = "Leon",
                Title = "Principal Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-0002"
            };

            command.ShouldNotValidate("'Email' must not be empty.");

            command.Email = SampleEmail();
            command.ShouldValidate();

            command.Email = "test at example dot com";
            command.ShouldNotValidate("'Email' is not a valid email address.");
        }

        public async Task ShouldRequireUniqueEmail()
        {
            var employeeToEdit = await Register();
            var preexistingEmployee = await Register();

            var command = await Send(new EditEmployee.Query { Id = employeeToEdit.Id });
            command.ShouldValidate();

            command.Email = SampleEmail();
            command.ShouldValidate();

            command.Email = preexistingEmployee.Email;
            command.ShouldNotValidate(
                $"Another employee already uses '{command.Email}'. " +
                "Please enter a unique email address.");
        }

        public async Task ShouldSaveChangesToEmployeeData()
        {
            var selectedEmployee = await Register();
            var anotherEmployee = await Register();
            await LogIn();

            var originalHashedPassword = selectedEmployee.HashedPassword;

            var email = SampleEmail();
            await Send(new EditEmployee.Command
            {
                Id = selectedEmployee.Id,
                Email = email,
                FirstName = "Patrick",
                LastName = "Jones",
                SportTeam = "Leon",
                Title = "Principal Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-0002"
            });

            var actual = Query<Employee>(selectedEmployee.Id);

            actual.ShouldMatch(new Employee
            {
                Id = selectedEmployee.Id,
                HashedPassword = originalHashedPassword,
                Email = email,
                FirstName = "Patrick",
                LastName = "Jones",
                SportTeam = "Leon",
                Title = "Principal Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-0002"
            });
        }

        public async Task ShouldAllowUserToEditTheirOwnEmailByCorrectingAuthenticationCookie()
        {
            var self = await LogIn();

            var originalHashedPassword = self.HashedPassword;

            var email = SampleEmail();
            await Send(new EditEmployee.Command
            {
                Id = self.Id,
                Email = email,
                FirstName = self.FirstName,
                LastName = self.LastName,
                SportTeam = self.SportTeam,
                Title = self.Title,
                Office = self.Office,
                PhoneNumber = self.PhoneNumber
            });

            var actual = Query<Employee>(self.Id);

            actual.ShouldMatch(new Employee
            {
                Id = self.Id,
                HashedPassword = originalHashedPassword,
                Email = email,
                FirstName = self.FirstName,
                LastName = self.LastName,
                SportTeam = self.SportTeam,
                Title = self.Title,
                Office = self.Office,
                PhoneNumber = self.PhoneNumber
            });

            Scoped<ILoginService>(loginService =>
            {
                ((StubLoginService)loginService)
                    .AuthenticatedEmail.ShouldBe(email);
            });
        }
    }
}