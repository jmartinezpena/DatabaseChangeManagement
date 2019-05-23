namespace EmployeeDirectory.Model
{
    using System;
    using System.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Serilog;

    public class DirectoryContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public DirectoryContext(DbContextOptions<DirectoryContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<EmployeeRole> EmployeeRole { get; set; }
        public DbQuery<PermissionView> PermissionView { get; set; }

        public void BeginTransaction()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CloseTransaction()
        {
            CloseTransaction(exception: null);
        }

        public void CloseTransaction(Exception exception)
        {
            try
            {
                if (exception != null)
                {
                    _currentTransaction?.Rollback();
                    return;
                }

                SaveChanges();

                _currentTransaction?.Commit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception thrown while attempting to close a transaction.");
                _currentTransaction?.Rollback();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }
    }
}