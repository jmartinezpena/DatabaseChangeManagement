namespace EmployeeDirectory.Tests.Features.Security
{
    using System.Linq;
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Security;
    using static Testing;
    using static EmployeeDirectory.Model.Permission;

    public class PermissionsTests
    {
        private Role _systemAdministratorRole;
        private Role _humanResourcesRole;
        private Role _managerRole;
        private Employee _ceo;
        private Employee _systemAdministrator;
        private Employee _directorOfHumanResources;
        private Employee _salesManager;
        private Employee _employee;

        public async Task SetUp()
        {
            await LogIn();

            _systemAdministratorRole = await CreateRole();
            _humanResourcesRole = await CreateRole();
            _managerRole = await CreateRole();

            _ceo = await Register();
            _systemAdministrator = await Register();
            _directorOfHumanResources = await Register();
            _salesManager = await Register();
            _employee = await Register();

            await AssignRoles(_ceo, _systemAdministratorRole, _humanResourcesRole, _managerRole);
            await AssignRoles(_systemAdministrator, _systemAdministratorRole);
            await AssignRoles(_directorOfHumanResources, _humanResourcesRole);
            await AssignRoles(_salesManager, _managerRole);

            await AssignPermissions(_systemAdministratorRole, ManageSecurity);
            await AssignPermissions(_humanResourcesRole, RegisterEmployees, DeleteEmployees, EditEmployees);
            await AssignPermissions(_managerRole, EditEmployees);
        }

        public async Task ShouldGetDistinctPermissionsByEmployee()
        {
            await AssertPermissions(_ceo,
                ManageSecurity, RegisterEmployees, DeleteEmployees, EditEmployees);

            await AssertPermissions(_systemAdministrator, ManageSecurity);

            await AssertPermissions(_directorOfHumanResources,
                RegisterEmployees, DeleteEmployees, EditEmployees);

            await AssertPermissions(_salesManager, EditEmployees);

            await AssertPermissions(_employee, new Permission[] {});
        }

        private static async Task AssertPermissions(Employee employee, params Permission[] expectedPermissions)
        {
            var actualPermissions = await Send(new Permissions.Query
            {
                EmployeeId = employee.Id
            });

            actualPermissions
                .ToArray()
                .ShouldMatch(expectedPermissions);
        }
    }
}