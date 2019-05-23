namespace EmployeeDirectory.Features.Role
{
    using System;
    using System.Data.SqlClient;
    using Model;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class PermissionAssignment
    {
        public class Command : IRequest
        {
            public Guid RoleId { get; set; }
            public Permission[] Permissions { get; set; }
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
                    "DELETE FROM [RolePermission] WHERE [RoleId] = @RoleId",
                    new SqlParameter("RoleId", message.RoleId));

                foreach (var permission in message.Permissions)
                {
                    _directory.Database.ExecuteSqlCommand(
                        "INSERT INTO [RolePermission] ([RoleId], [Permission]) VALUES (@RoleId, @Permission)",
                        new SqlParameter("RoleId", message.RoleId),
                        new SqlParameter("Permission", permission));
                }
            }
        }
    }
}
