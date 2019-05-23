namespace EmployeeDirectory.Features.Account
{
    using System.ComponentModel.DataAnnotations;
    using FluentValidation;
    using Infrastructure;
    using MediatR;

    public class ChangePassword
    {
        public class Command : IRequest
        {
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(UserContext context)
            {
                RuleFor(x => x.CurrentPassword).NotEmpty();
                RuleFor(x => x.NewPassword).NotEmpty();
                RuleFor(x => x.ConfirmPassword).NotEmpty();

                RuleFor(x => x.CurrentPassword)
                    .Must(currentPassword => PasswordService.Verify(currentPassword, context.User.HashedPassword))
                    .When(x => x.CurrentPassword != null)
                    .WithMessage("'{PropertyName}' was invalid. Please enter your current password and try again.");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword)
                    .When(x => x.ConfirmPassword != null && x.NewPassword != null)
                    .WithMessage("These passwords do not match. Be sure to enter the same password twice.");
            }
        }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly UserContext _context;

            public CommandHandler(UserContext context)
            {
                _context = context;
            }

            protected override void HandleCore(Command message)
                => _context.User.HashedPassword = PasswordService.HashPassword(message.NewPassword);
        }
    }
}