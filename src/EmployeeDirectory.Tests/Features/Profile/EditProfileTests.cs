namespace EmployeeDirectory.Tests.Features.Profile
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Profile;
    using EmployeeDirectory.Infrastructure;
    using Infrastructure;
    using Shouldly;
    using static Testing;

    public class EditProfileTests
    {
        public async Task ShouldGetEmployeeDataForCurrentlyLoggedInEmployee()
        {
            var anotherEmployee = await Register();
            var loggedInEmployee = await LogIn();

            var result = await Send(new EditProfile.Query());

            result.ShouldMatch(new EditProfile.Command
            {
                FirstName = loggedInEmployee.FirstName,
                LastName = loggedInEmployee.LastName,
                SportTeam = loggedInEmployee.SportTeam,
                Title = loggedInEmployee.Title,
                Office = loggedInEmployee.Office,
                Email = loggedInEmployee.Email,
                PhoneNumber = loggedInEmployee.PhoneNumber
            });
        }

        public void ShouldRequireMinimumFields()
        {
            new EditProfile.Command()
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
            var command = new EditProfile.Command
            {
                FirstName = SampleFirstName(),
                LastName = SampleLastName(),
                SportTeam = SampleSportTeam(),
                Title = SampleTitle(),
                Office = Sample<Office>(),
                PhoneNumber = SamplePhoneNumber()
            };

            command.ShouldNotValidate("'Email' must not be empty.");

            command.Email = SampleEmail();
            command.ShouldValidate();

            command.Email = "test at example dot com";
            command.ShouldNotValidate("'Email' is not a valid email address.");
        }

        public async Task ShouldRequireUniqueEmail()
        {
            var employeeToEdit = await LogIn();
            var preexistingEmployee = await Register();

            var command = await Send(new EditProfile.Query());
            command.ShouldValidate();

            command.Email = SampleEmail();
            command.ShouldValidate();

            command.Email = preexistingEmployee.Email;
            command.ShouldNotValidate(
                $"Another employee already uses '{command.Email}'. " +
                "Please enter a unique email address.");
        }

        public async Task ShouldSaveChangesToCurrentlyLoggedInEmployee()
        {
            var anotherEmployee = await Register();
            var loggedInEmployee = await LogIn();

            var originalHashedPassword = loggedInEmployee.HashedPassword;

            var email = SampleEmail();
            await Send(new EditProfile.Command
            {
                Email = email,
                FirstName = "Patrick",
                LastName = "Jones",
                SportTeam = "Pumas",
                Title = "Principal Consultant",
                Office = Office.Houston,
                PhoneNumber = "555-123-0002"
            });

            var actual = Query<Employee>(loggedInEmployee.Id);

            actual.ShouldMatch(new Employee
            {
                Id = loggedInEmployee.Id,
                Email = email,
                HashedPassword = originalHashedPassword,
                FirstName = "Patrick",
                LastName = "Jones",
                SportTeam = "Pumas",
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
            await Send(new EditProfile.Command
            {
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