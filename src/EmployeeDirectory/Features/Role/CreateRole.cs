namespace EmployeeDirectory.Features.Role
{
    using System;
    using System.Linq;
    using FluentValidation;
    using MediatR;
    using Model;

    public class CreateRole
    {
        public class Command : IRequest<Response>
        {
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DirectoryContext _directory;

            public Validator(DirectoryContext directory)
            {
                _directory = directory;

                RuleFor(x => x.Name).NotEmpty().Length(1, 100);

                RuleFor(x => x.Name)
                    .Must(BeUniqueName)
                    .When(x => x.Name != null)
                    .WithMessage("There is already a role named '{PropertyValue}'. Please enter a unique role name.");
            }

            private bool BeUniqueName(string name)
                => _directory.Role.Count(x => x.Name == name) == 0;
        }

        public class Response
        {
            public Guid RoleId { get; set; }
        }

        public class CommandHandler : RequestHandler<Command, Response>
        {
            private readonly DirectoryContext _database;

            public CommandHandler(DirectoryContext database)
            {
                _database = database;
            }

            protected override Response HandleCore(Command message)
            {
                var role = new Role { Name = message.Name };

                _database.Role.Add(role);

                return new Response { RoleId = role.Id };
            }
        }
    }
}