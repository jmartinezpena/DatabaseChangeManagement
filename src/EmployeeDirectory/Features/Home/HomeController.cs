namespace EmployeeDirectory.Features.Home
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Employee");

        public IActionResult Error() => View();
    }
}