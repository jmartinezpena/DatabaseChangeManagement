namespace EmployeeDirectory.Tests.Model
{
    using static Testing;

    public class EmployeeTests
    {
        public void ShouldPersist()
        {
            var employee = SampleEmployee();

            employee.ShouldPersist();
        }
    }
}