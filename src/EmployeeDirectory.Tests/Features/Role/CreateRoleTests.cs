namespace EmployeeDirectory.Tests.Features.Role
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Role;
    using static Testing;

    public class CreateRoleTests
    {
        public void ShouldRequireMinimumFields()
        {
            new CreateRole.Command()
                .ShouldNotValidate("'Name' should not be empty.");
        }

        public async Task ShouldRequireUniqueName()
        {
            var preexistingRole = await CreateRole();

            var command = new CreateRole.Command
            {
                Name = SampleRole().Name
            };

            command.ShouldValidate();

            command.Name = preexistingRole.Name;

            command.ShouldNotValidate(
                $"There is already a role named '{command.Name}'. " +
                "Please enter a unique role name.");
        }

        public async Task ShouldCreateNewRole()
        {
            var name = SampleRole().Name;

            var response = await Send(new CreateRole.Command
            {
                Name = name
            });

            var actual = Query<Role>(response.RoleId);

            actual.ShouldMatch(new Role
            {
                Id = response.RoleId,
                Name = name
            });
        }
    }
}