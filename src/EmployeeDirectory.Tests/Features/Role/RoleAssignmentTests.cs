namespace EmployeeDirectory.Tests.Features.Role
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Account;
    using EmployeeDirectory.Model;
    using EmployeeDirectory.Features.Role;
    using RoleSelection = EmployeeDirectory.Features.Role.RoleAssignment.Command.RoleSelection;
    using static Testing;

    public class RoleAssignmentTests
    {
        private Employee _intern, _manager, _admin;
        private Role _roleA, _roleB, _roleC;

        public async Task SetUp()
        {
            ResetRolePermissionMatrix();

            await LogIn();

            _roleA = await CreateRole(x => x.Name = "Role A");
            _roleB = await CreateRole(x => x.Name = "Role B");
            _roleC = await CreateRole(x => x.Name = "Role C");

            //Intern has no assigned roles.

            _intern = await Register(x =>
            {
                x.FirstName = "Joe";
                x.LastName = "Intern";
            });

            //Manager has roles A, C.

            _manager = await Register(x =>
            {
                x.FirstName = "Bill";
                x.LastName = "Manager";
            });

            await Send(new RoleAssignment.Command
            {
                EmployeeId = _manager.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Selected = true },
                    new RoleSelection { RoleId = _roleB.Id, Selected = false },
                    new RoleSelection { RoleId = _roleC.Id, Selected = true }
                }
            });

            //Admin has roles A, B, C.

            _admin = await Register(x =>
            {
                x.FirstName = "Sally";
                x.LastName = "Admin";
            });

            await Send(new RoleAssignment.Command
            {
                EmployeeId = _admin.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Selected = true },
                    new RoleSelection { RoleId = _roleB.Id, Selected = true },
                    new RoleSelection { RoleId = _roleC.Id, Selected = true }
                }
            });
        }

        public async Task ShouldGetCurrentRoleAssignmentsByEmployee()
        {
            var internAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = _intern.Id });

            internAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeId = _intern.Id,
                EmployeeName = "Joe Intern",
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = false },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = false },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = false }
                }
            });

            var managerAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = _manager.Id });

            managerAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeName = "Bill Manager",
                EmployeeId = _manager.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = true },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = false },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = true }
                }
            });

            var adminAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = _admin.Id });

            adminAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeName = "Sally Admin",
                EmployeeId = _admin.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = true },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = true },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = true }
                }
            });
        }

        public async Task ShouldSaveNewRoleSelectionsPerEmployee()
        {
            await Send(new RoleAssignment.Command
            {
                EmployeeId = _manager.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Selected = false },
                    new RoleSelection { RoleId = _roleB.Id, Selected = true },
                    new RoleSelection { RoleId = _roleC.Id, Selected = false }
                }
            });

            var internAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = _intern.Id });

            internAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeId = _intern.Id,
                EmployeeName = "Joe Intern",
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = false },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = false },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = false }
                }
            });

            var managerAssignments =
                await Send(new RoleAssignment.Query { EmployeeId = _manager.Id });

            managerAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeName = "Bill Manager",
                EmployeeId = _manager.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = false },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = true },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = false }
                }
            });

            var adminAssignments = await Send(new RoleAssignment.Query { EmployeeId = _admin.Id });

            adminAssignments.ShouldMatch(new RoleAssignment.Command
            {
                EmployeeName = "Sally Admin",
                EmployeeId = _admin.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = _roleA.Id, Name = "Role A", Selected = true },
                    new RoleSelection { RoleId = _roleB.Id, Name = "Role B", Selected = true },
                    new RoleSelection { RoleId = _roleC.Id, Name = "Role C", Selected = true }
                }
            });
        }

        public async Task ShouldNotAllowEmployeesToModifyTheirOwnRoleAssignments()
        {
            var email = SampleEmail();
            var password = SamplePassword();

            var self = await Register(x =>
            {
                x.Email = email;
                x.Password = password;
                x.ConfirmPassword = password;
            });
            var anotherEmployee = await Register();

            var adminRole = await CreateRole();
            var anotherRole = await CreateRole();
            await AssignRoles(self, adminRole);

            await Send(new LogIn.Command { Email = email, Password = password });

            new RoleAssignment.Command
            {
                EmployeeId = self.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = adminRole.Id, Selected = true },
                    new RoleSelection { RoleId = anotherRole.Id, Selected = true }
                }
            }.ShouldNotValidate("Employees cannot modify their own role assignments.");

            new RoleAssignment.Command
            {
                EmployeeId = anotherEmployee.Id,
                Roles = new[]
                {
                    new RoleSelection { RoleId = adminRole.Id, Selected = true },
                    new RoleSelection { RoleId = anotherRole.Id, Selected = true }
                }
            }.ShouldValidate();
        }
    }
}