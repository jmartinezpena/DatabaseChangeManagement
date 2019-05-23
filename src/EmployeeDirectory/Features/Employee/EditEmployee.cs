namespace EmployeeDirectory.Features.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using Infrastructure;
    using MediatR;
    using Model;

    public class EditEmployee
    {
        public class Query : IRequest<Command>
        {
            public Guid Id { get; set; }
        }

        public class QueryHandler : RequestHandler<Query, Command>
        {
            private readonly DirectoryContext _database;
            private readonly IMapper _mapper;

            public QueryHandler(DirectoryContext database, IMapper mapper)
            {
                _database = database;
                _mapper = mapper;
            }

            protected override Command HandleCore(Query message)
            {
                var employee = _database.Employee.Find(message.Id);

                return _mapper.Map<Command>(employee);
            }
        }

        public class Command : IRequest
        {
            public Guid Id { get; set; }

            public string Email { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public string Title { get; set; }

            public Office? Office { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DirectoryContext _directory;

            public Validator(DirectoryContext directory)
            {
                _directory = directory;

                RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
                RuleFor(x => x.FirstName).NotEmpty().Length(1, 100);
                RuleFor(x => x.LastName).NotEmpty().Length(1, 100);
                RuleFor(x => x.Title).NotEmpty().Length(1, 100);
                RuleFor(x => x.Office).NotEmpty();
                RuleFor(x => x.PhoneNumber).Length(1, 50);

                RuleFor(x => x.Email)
                    .Must(BeUniqueEmail)
                    .When(x => x.Email != null)
                    .WithMessage("Another employee already uses '{PropertyValue}'. Please enter a unique email address.");
            }

            private bool BeUniqueEmail(Command command, string email)
            {
                var existingEmployee = _directory.Employee.SingleOrDefault(x => x.Email == email);

                return existingEmployee == null || existingEmployee.Id == command.Id;
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly DirectoryContext _database;
            private readonly IMapper _mapper;
            private readonly UserContext _userContext;
            private readonly ILoginService _loginService;

            public CommandHandler(
                DirectoryContext database,
                IMapper mapper,
                UserContext userContext,
                ILoginService loginService)
            {
                _database = database;
                _mapper = mapper;
                _userContext = userContext;
                _loginService = loginService;
            }

            protected override async Task HandleCore(Command message)
            {
                var employee = _database.Employee.Find(message.Id);

                _mapper.Map(message, employee);

                if (_userContext.User.Id == message.Id)
                    await _loginService.LogIn(message.Email);
            }
        }
    }
}