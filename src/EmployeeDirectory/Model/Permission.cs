namespace EmployeeDirectory.Model
{
    using System;

    public enum Permission
    {
        ManageSecurity = 1,
        RegisterEmployees = 2,
        DeleteEmployees = 3,
        EditEmployees = 4
    }

    public class PermissionView
    {
        public Guid EmployeeId { get; set; }
        public Permission Permission { get; set; }
    }
}