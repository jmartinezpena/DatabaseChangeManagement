namespace EmployeeDirectory.Features.Profile
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    public class ProfileController : BaseController
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> Edit()
        {
            var model = await _mediator.Send(new EditProfile.Query());

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditProfile.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                SuccessMessage("Your profile has been updated.");

                return RedirectToAction("Index", "Home");
            }

            return View(command);
        }
    }
}