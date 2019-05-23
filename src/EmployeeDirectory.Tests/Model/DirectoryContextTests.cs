namespace EmployeeDirectory.Tests.Model
{
    using System;
    using System.Linq;
    using EmployeeDirectory.Model;
    using Shouldly;
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
                    intermediateCount.ShouldBe(countBefore + 1);

                    throw new Exception("This is expected to cause a rollback.");
                });
            };

            failingTransaction.ShouldThrow<Exception>().Message.ShouldBe("This is expected to cause a rollback.");

            var countAfter = Count<Employee>();

            countAfter.ShouldBe(countBefore);

            var loaded = Query<Employee>(employee.Id);
            loaded.ShouldBeNull();
        }
    }
}