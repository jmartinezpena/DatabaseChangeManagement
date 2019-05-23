namespace EmployeeDirectory.Features.Account
{
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;

    public class LogOut
    {
        public class Command : IRequest
        {
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ILoginService _loginService;

            public CommandHandler(ILoginService loginService)
            {
                _loginService = loginService;
            }

            protected override async Task Handle(Command message, CancellationToken cancellationToken)
                => await _loginService.LogOut();
        }
    }
}