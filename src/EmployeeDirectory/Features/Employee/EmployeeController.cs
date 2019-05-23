namespace EmployeeDirectory.Features.Employee
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using static Model.Permission;

    public class EmployeeController : BaseController
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> Index(EmployeeIndex.Query query)
        {
            var model = await _mediator.Send(query);

            return View(model);
        }

        [RequirePermission(RegisterEmployees)]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [RequirePermission(RegisterEmployees)]
        public async Task<ActionResult> Register(RegisterEmployee.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                SuccessMessage($"{command.FirstName} {command.LastName} has been registered.");

                return RedirectToAction("Index");
            }

            return View(command);
        }

        [RequirePermission(EditEmployees)]
        public async Task<ActionResult> Edit(EditEmployee.Query query)
        {
            var model = await _mediator.Send(query);

            return View(model);
        }

        [HttpPost]
        [RequirePermission(EditEmployees)]
        public async Task<ActionResult> Edit(EditEmployee.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                SuccessMessage($"{command.FirstName} {command.LastName} has been updated.");

                return RedirectToAction("Index");
            }

            return View(command);
        }

        [HttpPost]
        [RequirePermission(DeleteEmployees)]
        public async Task<ActionResult> Delete(DeleteEmployee.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                SuccessMessage($"{command.FirstName} {command.LastName} has been deleted.");

                return AjaxRedirect(Url.Action("Index"));
            }

            DisplayGeneralValidationErrors();

            return AjaxRedirect(Url.Action("Index"));
        }
    }
}