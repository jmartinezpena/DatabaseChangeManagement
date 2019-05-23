namespace EmployeeDirectory.Features.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class Permissions
    {
        public class Query : IRequest<IReadOnlyList<Permission>>
        {
            public Guid EmployeeId { get; set; }
        }

        public class QueryHandler : RequestHandler<Query, IReadOnlyList<Permission>>
        {
            private readonly DirectoryContext _directory;

            public QueryHandler(DirectoryContext directory)
            {
                _directory = directory;
            }

            protected override IReadOnlyList<Permission> HandleCore(Query message)
            {
                return _directory.PermissionView.FromSql(
                        @"SELECT [EmployeeRole].[EmployeeId], [RolePermission].[Permission]
                          FROM [Role]
                          INNER JOIN [RolePermission] ON [RolePermission].[RoleId] = [Role].[Id]
                          INNER JOIN [EmployeeRole] ON [EmployeeRole].[RoleId] = [Role].[Id]")
                    .Where(x => x.EmployeeId == message.EmployeeId)
                    .OrderBy(x => x.Permission)
                    .Select(x => x.Permission)
                    .Distinct()
                    .ToArray();
            }
        }
    }
}