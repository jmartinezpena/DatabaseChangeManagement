namespace EmployeeDirectory.Features.Role
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using static Model.Permission;

    public class RoleController : BaseController
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [RequirePermission(ManageSecurity)]
        public async Task<ActionResult> Assign(RoleAssignment.Query query)
        {
            var model = await _mediator.Send(query);

            return View(model);
        }

        [HttpPost]
        [RequirePermission(ManageSecurity)]
        public async Task<ActionResult> Assign(RoleAssignment.Command command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);

                return RedirectToAction("Index", "Employee");
            }

            DisplayGeneralValidationErrors();

            return RedirectToAction("Assign", new RoleAssignment.Query { EmployeeId = command.EmployeeId });
        }
    }
}