namespace EmployeeDirectory.Infrastructure
{
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Model;

    public class RequirePermissionFilter : IAuthorizationFilter
    {
        private readonly Permission _permission;
        private readonly UserContext _userContext;

        public RequirePermissionFilter(Permission permission, UserContext userContext)
        {
            _permission = permission;
            _userContext = userContext;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_userContext.Has(_permission))
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}