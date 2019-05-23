namespace EmployeeDirectory.Tests.Infrastructure
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Infrastructure;

    public class StubLoginService : ILoginService
    {
        public string AuthenticatedEmail { get; private set; }

        public Task LogIn(string email)
        {
            AuthenticatedEmail = email;
            return Task.CompletedTask;
        }

        public Task LogOut()
        {
            AuthenticatedEmail = null;
            return Task.CompletedTask;
        }

        public void Reset()
        {
            AuthenticatedEmail = null;
        }
    }
}