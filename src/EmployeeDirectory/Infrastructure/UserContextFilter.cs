namespace EmployeeDirectory.Infrastructure
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Model;
    using Features.Security;
    using MediatR;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class UserContextFilter : IAsyncAuthorizationFilter
    {
        private readonly DirectoryContext _database;
        private readonly UserContext _userContext;
        private readonly ILoginService _loginService;
        private readonly IMediator _mediator;

        public UserContextFilter(
            DirectoryContext database,
            UserContext userContext,
            ILoginService loginService,
            IMediator mediator)
        {
            _database = database;
            _userContext = userContext;
            _loginService = loginService;
            _mediator = mediator;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var email = context.HttpContext.User.Identity.Name;
            var user = _database.Employee.SingleOrDefault(x => x.Email == email);

            if (email != null && user == null)
            {
                // The user is still authenticated under a previous email address,
                // but the email address in the database has since been updated.
                // Force them to log in again.
                await _loginService.LogOut();
                context.Result = new UnauthorizedResult();
            }
            else
            {
                _userContext.User = user;
                _userContext.Permissions = new Permission[] { };

                if (user != null)
                {
                    var query = new Permissions.Query
                    {
                        EmployeeId = user.Id
                    };
                    _userContext.Permissions = await _mediator.Send(query);
                }
            }
        }
    }
}