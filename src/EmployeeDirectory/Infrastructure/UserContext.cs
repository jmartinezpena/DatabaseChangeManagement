namespace EmployeeDirectory.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;

    public class UserContext
    {
        public Employee User { get; set; }
        public bool IsAuthenticated => User != null;
        public IReadOnlyList<Permission> Permissions { get; set; }
        public bool Has(Permission permission)
            => Permissions.Contains(permission);
    }
}