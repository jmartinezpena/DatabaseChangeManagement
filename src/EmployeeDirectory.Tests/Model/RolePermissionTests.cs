namespace EmployeeDirectory.Tests.Model
{
    using EmployeeDirectory.Model;
    using static Testing;

    public class RolePermissionTests
    {
        public void ShouldPersist()
        {
            var rolePermission = new RolePermission
            {
                Role = SampleRole(),
                Permission = Sample<Permission>()
            };

            rolePermission.ShouldPersist();
        }
    }
}