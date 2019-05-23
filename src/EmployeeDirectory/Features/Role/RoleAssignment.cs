namespace EmployeeDirectory.Features.Role
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure;
    using Model;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class RoleAssignment
    {
        public class Query : IRequest<Command>
        {
            public Guid EmployeeId { get; set; }
        }

        public class Command : IRequest
        {
            public Guid EmployeeId { get; set; }
            public RoleSelection[] Roles { get; set; }
            public string EmployeeName { get; set; }

            public class RoleSelection
            {
                public Guid RoleId { get; set; }
                public string Name { get; set; }
                public bool Selected { get; set; }
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(UserContext context)
            {
                RuleFor(command => command)
                    .Must(command => command.EmployeeId != context.User.Id)
                    .WithMessage("Employees cannot modify their own role assignments.");
            }
        }

        public class QueryHandler : RequestHandler<Query, Command>
        {
            private readonly DirectoryContext _directory;

            public QueryHandler(DirectoryContext directory)
            {
                _directory = directory;
            }

            protected override Command Handle(Query message)
            {
                var selectedRoles = _directory.EmployeeRole
                    .Where(x => x.Employee.Id == message.EmployeeId)
                    .Select(x => x.Role)
                    .ToArray();

                var allRoles = _directory.Role.OrderBy(x => x.Name).ToArray();

                var employee = _directory.Employee.Find(message.EmployeeId);

                return new Command
                {
                    EmployeeId = message.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Roles = allRoles.Select(r => new Command.RoleSelection
                    {
                        RoleId = r.Id,
                        Name = r.Name,
                        Selected = selectedRoles.Contains(r)
                    }).ToArray()
                };
            }
        }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly DirectoryContext _directory;

            public CommandHandler(DirectoryContext directory)
            {
                _directory = directory;
            }

            protected override void Handle(Command message)
            {
                _directory.Database.ExecuteSqlCommand(
                    "DELETE FROM [EmployeeRole] WHERE [EmployeeId] = @EmployeeId",
                    new SqlParameter("EmployeeId", message.EmployeeId));

                foreach (var selection in message.Roles.Where(x => x.Selected))
                {
                    _directory.Database.ExecuteSqlCommand(
                        "INSERT INTO [EmployeeRole] ([EmployeeId], [RoleId]) VALUES (@EmployeeId, @RoleId)",
                        new SqlParameter("EmployeeId", message.EmployeeId),
                        new SqlParameter("RoleId", selection.RoleId));
                }
            }
        }
    }
}