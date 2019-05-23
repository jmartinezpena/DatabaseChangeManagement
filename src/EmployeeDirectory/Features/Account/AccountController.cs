namespace EmployeeDirectory.Features.Account
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        public ActionResult LogIn()
        {
            return View(new LogIn.Command());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogIn(LogIn.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                return RedirectToAction("Index", "Home");
            }

            DisplayGeneralValidationErrors();

            return View(command);
        }

        public async Task<ActionResult> LogOut([FromServices] UserContext userContext)
        {
            if (userContext.IsAuthenticated)
            {
                await _mediator.Send(new LogOut.Command());
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePassword.Command());
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePassword.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                return RedirectToAction("Index", "Home");
            }

            return View(command);
        }
    }
}