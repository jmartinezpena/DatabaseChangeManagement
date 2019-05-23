namespace EmployeeDirectory.Tests.Features.Role
{
    using System.Linq;
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Role;
    using EmployeeDirectory.Model;
    using Shouldly;
    using static EmployeeDirectory.Model.Permission;
    using static Testing;

    public class PermissionAssignmentTests
    {
        public async Task ShouldAssignPermissionsPerRole()
        {
            var roleA = await CreateRole();
            var roleB = await CreateRole();
            var roleC = await CreateRole();

            //Role A has no assigned permissions.

            //Role B can Delete and Edit Employees.

            await Send(new PermissionAssignment.Command
            {
                RoleId = roleB.Id,
                Permissions = new[]
                {
                    DeleteEmployees,
                    EditEmployees
                }
            });

            //Role C can Register, Delete, and Edit Employees.

            await Send(new PermissionAssignment.Command
            {
                RoleId = roleC.Id,
                Permissions = new[]
                {
                    RegisterEmployees,
                    DeleteEmployees,
                    EditEmployees
                }
            });

            //Dramatically modify Role C's assignments. Only these should remain.

            await Send(new PermissionAssignment.Command
            {
                RoleId = roleC.Id,
                Permissions = new[]
                {
                    ManageSecurity,
                    RegisterEmployees
                }
            });

            ActualPermissions(roleA)
                .ShouldBeEmpty();

            ActualPermissions(roleB)
                .ShouldMatch(DeleteEmployees, EditEmployees);

            ActualPermissions(roleC)
                .ShouldMatch(ManageSecurity, RegisterEmployees);
        }

        private static Permission[] ActualPermissions(Role role)
            => Query(database => database
                .RolePermission
                .Where(x => x.Role.Id == role.Id)
                .OrderBy(x => x.Permission)
                .Select(x => x.Permission)
                .ToArray());
    }
}