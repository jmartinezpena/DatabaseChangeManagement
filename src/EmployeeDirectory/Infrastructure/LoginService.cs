namespace EmployeeDirectory.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;

    public interface ILoginService
    {
        Task LogIn(string email);
        Task LogOut();
    }

    public class LoginService : ILoginService
    {
        private const string AuthenticationScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogIn(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _httpContextAccessor.HttpContext
                .SignInAsync(AuthenticationScheme, principal, properties);
        }

        public async Task LogOut()
        {
            await _httpContextAccessor.HttpContext
                .SignOutAsync(AuthenticationScheme);
        }
    }
}