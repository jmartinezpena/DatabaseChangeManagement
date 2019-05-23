namespace EmployeeDirectory.Features.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using FluentValidation;
    using Infrastructure;
    using MediatR;

    public class LogIn
    {
        public class Command : IRequest
        {
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DirectoryContext _database;

            public Validator(DirectoryContext database)
            {
                _database = database;

                RuleFor(x => x)
                    .Must(PassVerification)
                    .WithMessage("The email address or password was invalid. Please try again.");
            }

            private bool PassVerification(Command command)
            {
                if (string.IsNullOrEmpty(command.Email) || string.IsNullOrEmpty(command.Password))
                    return false;

                var employee = _database.Employee.SingleOrDefault(x => x.Email == command.Email);

                if (employee == null)
                    return false;

                if (!PasswordService.Verify(command.Password, employee.HashedPassword))
                    return false;

                return true;
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ILoginService _loginService;

            public CommandHandler(ILoginService loginService)
            {
                _loginService = loginService;
            }

            protected override async Task Handle(Command message, CancellationToken cancellationToken)
                => await _loginService.LogIn(message.Email);
        }
    }
}