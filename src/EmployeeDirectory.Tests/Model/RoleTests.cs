namespace EmployeeDirectory.Tests.Model
{
    using EmployeeDirectory.Model;
    using static Testing;

    public class RoleTests
    {
        public void ShouldPersist()
        {
            var role = SampleRole();

            role.ShouldPersist();
        }
    }
}