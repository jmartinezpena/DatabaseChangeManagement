namespace EmployeeDirectory.Infrastructure
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Model;

    public class UnitOfWork : IActionFilter
    {
        private readonly DirectoryContext _database;

        public UnitOfWork(DirectoryContext database)
        {
            _database = database;
        }

        public void OnActionExecuting(ActionExecutingContext context)
            => _database.BeginTransaction();

        public void OnActionExecuted(ActionExecutedContext context)
            => _database.CloseTransaction(context.Exception);
    }
}