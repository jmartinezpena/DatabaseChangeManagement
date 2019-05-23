namespace EmployeeDirectory.Tests.Model
{
    using System;
    using System.Linq;
    using EmployeeDirectory.Model;
    using Should;
    using static Testing;

    public class DirectoryContextTests
    {
        public void ShouldRollBackOnFailure()
        {
            var employee = SampleEmployee();

            var countBefore = Count<Employee>();

            Action failingTransaction = () =>
            {
                Transaction(database =>
                {
                    database.Employee.Add(employee);
                    database.SaveChanges();

                    var intermediateCount = database.Employee.Count();
                    intermediateCount.ShouldEqual(countBefore + 1);

                    throw new Exception("This is expected to cause a rollback.");
                });
            };

            failingTransaction.Throws<Exception>()
                .Message.ShouldEqual("This is expected to cause a rollback.");

            var countAfter = Count<Employee>();

            countAfter.ShouldEqual(countBefore);

            var loaded = Query<Employee>(employee.Id);
            loaded.ShouldBeNull();
        }
    }
}