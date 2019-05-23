namespace EmployeeDirectory.Tests.Model
{
    using EmployeeDirectory.Model;
    using static Testing;

    public class EmployeeRoleTests
    {
        public void ShouldPersist()
        {
            var employeeRole = new EmployeeRole
            {
                Employee = SampleEmployee(),
                Role = SampleRole()
            };

            employeeRole.ShouldPersist();
        }
    }
}