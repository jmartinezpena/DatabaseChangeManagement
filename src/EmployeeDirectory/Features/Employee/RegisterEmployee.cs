namespace EmployeeDirectory.Features.Employee
{
    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using FluentValidation;
    using MediatR;
    using Model;

    public class RegisterEmployee
    {
        public class Command : IRequest<Response>
        {
            public string Email { get; set; }

            [Display(Name = "Initial Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Confirm Initial Password")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public string Title { get; set; }

            public Office? Office { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DirectoryContext _directory;

            public Validator(DirectoryContext directory)
            {
                _directory = directory;

                RuleFor(x => x.Email).NotEmpty().EmailAddress().Length(1, 255);
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.ConfirmPassword).NotEmpty();
                RuleFor(x => x.FirstName).NotEmpty().Length(1, 100);
                RuleFor(x => x.LastName).NotEmpty().Length(1, 100);
                RuleFor(x => x.Title).NotEmpty().Length(1, 100);
                RuleFor(x => x.Office).NotEmpty();
                RuleFor(x => x.PhoneNumber).Length(1, 50);

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.Password)
                    .When(x => x.ConfirmPassword != null && x.Password != null)
                    .WithMessage("These passwords do not match. Be sure to enter the same password twice.");

                RuleFor(x => x.Email)
                    .Must(BeUniqueEmail)
                    .When(x => x.Email != null)
                    .WithMessage("Another employee already uses '{PropertyValue}'. Please enter a unique email address.");
            }

            private bool BeUniqueEmail(string email)
                => _directory.Employee.Count(x => x.Email == email) == 0;
        }

        public class Response
        {
            public Guid EmployeeId { get; set; }
        }

        public class CommandHandler : RequestHandler<Command, Response>
        {
            private readonly DirectoryContext _database;
            private readonly IMapper _mapper;

            public CommandHandler(DirectoryContext database, IMapper mapper)
            {
                _database = database;
                _mapper = mapper;
            }

            protected override Response HandleCore(Command message)
            {
                var employee = _mapper.Map<Employee>(message);

                _database.Employee.Add(employee);

                return new Response
                {
                    EmployeeId = employee.Id
                };
            }
        }
    }
}