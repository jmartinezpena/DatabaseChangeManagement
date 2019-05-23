namespace EmployeeDirectory.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;
    using Model;

    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(Permission permission)
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}