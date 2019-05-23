namespace EmployeeDirectory.Tests.Features.Employee
{
    using System.Linq;
    using System.Threading.Tasks;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Employee;
    using EmployeeDirectory.Features.Role;
    using Shouldly;
    using RoleSelection = EmployeeDirectory.Features.Role.RoleAssignment.Command.RoleSelection;
    using static Testing;

    public class DeleteEmployeeTests
    {
        public async Task ShouldDeleteEmployeeById()
        {
            var employeeToDelete = await Register();
            var employeeToPreserve = await Register();
            await LogIn();

            var countBefore = Count<Employee>();

            await Send(new DeleteEmployee.Command
            {
                Id = employeeToDelete.Id
            });

            var countAfter = Count<Employee>();
            countAfter.ShouldBe(countBefore - 1);

            var deletedEmployee = Query<Employee>(employeeToDelete.Id);
            deletedEmployee.ShouldBeNull();
            
            var remainingEmployee = Query<Employee>(employeeToPreserve.Id);
            remainingEmployee.ShouldMatch(employeeToPreserve);
        }

        public async Task ShouldNotAllowDeletingSelf()
        {
            var anotherEmployee = await Register();
            var self = await LogIn();

            new DeleteEmployee.Command { Id = anotherEmployee.Id }.ShouldValidate();
            new DeleteEmployee.Command { Id = self.Id }.ShouldNotValidate("Employees cannot delete themselves.");
        }

        public async Task ShouldNotAllowDeletingEmployeesWhoCanManageSecurity()
        {
            await LogIn();

            var adminRole = await CreateRole();
            var adminEmployee = await Register();
            var anotherEmployee = await Register();

            await AssignRoles(adminEmployee, adminRole);
            await AssignPermissions(adminRole, Permission.ManageSecurity);

            new DeleteEmployee.Command { Id = anotherEmployee.Id }
                .ShouldValidate();

            new DeleteEmployee.Command { Id = adminEmployee.Id }
                .ShouldNotValidate("You cannot delete an employee who has permission to " +
                                   "manage security. Please coordinate with your system " +
                                   "administrators first.");
        }

        public async Task ShouldDeleteEmployeeAndTheirRoleAssignments()
        {
            var employeeToDelete = await Register();
            var employeeToPreserve = await Register();
            await LogIn();

            var role = await CreateRole();
            await AssignRoles(employeeToDelete, role);
            await AssignRoles(employeeToPreserve, role);

            await Send(new DeleteEmployee.Command
            {
                Id = employeeToDelete.Id
            });

            var deletedEmployee = Query<Employee>(employeeToDelete.Id);
            deletedEmployee.ShouldBeNull();

            var remainingEmployee = Query<Employee>(employeeToPreserve.Id);
            remainingEmployee.ShouldMatch(employeeToPreserve);

            var remainingEmployeeRoleAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = remainingEmployee.Id });

            remainingEmployeeRoleAssignments
                .Roles
                .Single(x => x.Selected)
                .RoleId.ShouldBe(role.Id);
        }
    }
}