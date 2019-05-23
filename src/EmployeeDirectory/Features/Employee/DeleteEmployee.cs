namespace EmployeeDirectory.Features.Employee
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using Infrastructure;
    using MediatR;
    using Model;
    using Security;

    public class DeleteEmployee
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IMediator _mediator;

            public Validator(IMediator mediator, UserContext context)
            {
                _mediator = mediator;
                RuleFor(x => x.Id)
                    .NotEqual(context.User.Id)
                    .WithMessage("Employees cannot delete themselves.");

                RuleFor(x => x)
                    .MustAsync(NotHaveManageSecurityPermission)
                    .WithMessage(
                        "You cannot delete an employee who has permission to " +
                        "manage security. Please coordinate with your system " +
                        "administrators first.");
            }

            private async Task<bool> NotHaveManageSecurityPermission(Command command, CancellationToken token)
            {
                var permissionsForEmployeeToDelete =
                    await _mediator.Send(new Permissions.Query { EmployeeId = command.Id }, token);

                return !permissionsForEmployeeToDelete.Contains(Permission.ManageSecurity);
            }
        }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly DirectoryContext _database;

            public CommandHandler(DirectoryContext database)
            {
                _database = database;
            }

            protected override void Handle(Command message)
            {
                var employee = _database.Employee.Find(message.Id);

                var roleAssignments =
                    _database.EmployeeRole
                        .Where(x => x.Employee.Id == message.Id)
                        .ToArray();

                foreach (var roleAssignment in roleAssignments)
                    _database.EmployeeRole.Remove(roleAssignment);

                _database.Employee.Remove(employee);
            }
        }
    }
}