﻿namespace EmployeeDirectory.Tests.Features.Account
{
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Account;
    using EmployeeDirectory.Infrastructure;
    using Infrastructure;
    using Shouldly;
    using static Testing;

    public class LogOutTests
    {
        public async Task ShouldDeauthenticateUponLogout()
        {
            var employee = await LogIn();

            Scoped<ILoginService>(loginService =>
            {
                ((StubLoginService)loginService)
                    .AuthenticatedEmail.ShouldBe(employee.Email);
            });

            await Send(new LogOut.Command());

            Scoped<ILoginService>(loginService =>
            {
                ((StubLoginService)loginService)
                    .AuthenticatedEmail.ShouldBeNull();
            });
        }
    }
}