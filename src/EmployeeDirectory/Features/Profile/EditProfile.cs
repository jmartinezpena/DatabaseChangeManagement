namespace EmployeeDirectory.Features.Profile
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using Infrastructure;
    using MediatR;
    using Model;

    public class EditProfile
    {
        public class Query : IRequest<Command>
        {
        }

        public class QueryHandler : RequestHandler<Query, Command>
        {
            private readonly UserContext _userContext;
            private readonly IMapper _mapper;

            public QueryHandler(UserContext userContext, IMapper mapper)
            {
                _userContext = userContext;
                _mapper = mapper;
            }

            protected override Command Handle(Query message)
                => _mapper.Map<Command>(_userContext.User);
        }

        public class Command : IRequest
        {
            public string Email { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Sport Team")]
            public string SportTeam { get; set; }

            public string Title { get; set; }

            public Office? Office { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DirectoryContext _directory;
            private readonly UserContext _userContext;

            public Validator(DirectoryContext directory, UserContext userContext)
            {
                _directory = directory;
                _userContext = userContext;

                RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
                RuleFor(x => x.FirstName).NotEmpty().Length(1, 100);
                RuleFor(x => x.LastName).NotEmpty().Length(1, 100);
                RuleFor(x => x.SportTeam).NotEmpty().Length(1, 100);
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

                return existingEmployee == null || existingEmployee.Id == _userContext.User.Id;
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly IMapper _mapper;
            private readonly UserContext _userContext;
            private readonly ILoginService _loginService;

            public CommandHandler(IMapper mapper, UserContext userContext, ILoginService loginService)
            {
                _mapper = mapper;
                _userContext = userContext;
                _loginService = loginService;
            }

            protected override async Task Handle(Command message, CancellationToken cancellationToken)
            {
                _mapper.Map(message, _userContext.User);

                await _loginService.LogIn(message.Email);
            }
        }
    }
}