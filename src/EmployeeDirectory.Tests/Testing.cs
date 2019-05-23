namespace EmployeeDirectory.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using EmployeeDirectory.Features.Account;
    using EmployeeDirectory.Features.Employee;
    using EmployeeDirectory.Features.Role;
    using EmployeeDirectory.Infrastructure;
    using EmployeeDirectory.Model;
    using FluentValidation;
    using FluentValidation.Results;
    using Infrastructure;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using RoleSelection = EmployeeDirectory.Features.Role.RoleAssignment.Command.RoleSelection;
    using static EmployeeDirectory.Infrastructure.PasswordService;

    public static class Testing
    {
        private static readonly Random Random = new Random();
        private static readonly IServiceScopeFactory ScopeFactory;

        public static IConfigurationRoot Configuration { get; }

        static Testing()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables(Program.ApplicationName + ":")
                .Build();

            var startup = new Startup(Configuration);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            services.AddSingleton<ILoginService, StubLoginService>();

            var rootContainer = services.BuildServiceProvider();
            ScopeFactory = rootContainer.GetService<IServiceScopeFactory>();
        }

        public static string Json(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static T DeepCopy<T>(T value)
        {
            return JsonConvert.DeserializeObject<T>(Json(value));
        }

        public static void Scoped<TService>(Action<TService> useService)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var service = serviceProvider.GetService<TService>();

                useService(service);
            }
        }

        public static async Task Send(IRequest message)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var database = serviceProvider.GetService<DirectoryContext>();

                try
                {
                    database.BeginTransaction();
                    EmulateUserContextFilter(serviceProvider, database);
                    Validator(serviceProvider, message)?.Validate(message).ShouldBeSuccessful();
                    await serviceProvider.GetService<IMediator>().Send(message);
                    database.CloseTransaction();
                }
                catch (Exception exception)
                {
                    database.CloseTransaction(exception);
                    throw;
                }
            }
        }

        public static async Task<TResponse> Send<TResponse>(IRequest<TResponse> message)
        {
            TResponse response;

            using (var scope = ScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var database = serviceProvider.GetService<DirectoryContext>();

                try
                {
                    database.BeginTransaction();
                    EmulateUserContextFilter(serviceProvider, database);
                    Validator(serviceProvider, message)?.Validate(message).ShouldBeSuccessful();
                    response = await serviceProvider.GetService<IMediator>().Send(message);
                    database.CloseTransaction();
                }
                catch (Exception exception)
                {
                    database.CloseTransaction(exception);
                    throw;
                }
            }

            return response;
        }

        private static void EmulateUserContextFilter(IServiceProvider serviceProvider, DirectoryContext database)
        {
            var loginService = (StubLoginService)serviceProvider.GetService<ILoginService>();

            if (loginService.AuthenticatedEmail != null)
            {
                var userContext = serviceProvider.GetService<UserContext>();
                var user = database.Employee.SingleOrDefault(x => x.Email == loginService.AuthenticatedEmail);
                userContext.User = user;
            }
        }

        public static void Transaction(Action<DirectoryContext> action)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var database = scope.ServiceProvider.GetService<DirectoryContext>();

                try
                {
                    database.BeginTransaction();
                    action(database);
                    database.CloseTransaction();
                }
                catch (Exception exception)
                {
                    database.CloseTransaction(exception);
                    throw;
                }
            }
        }

        public static TResult Query<TResult>(Func<DirectoryContext, TResult> query)
        {
            var result = default(TResult);

            Transaction(database =>
            {
                result = query(database);
            });

            return result;
        }

        public static TEntity Query<TEntity>(Guid id) where TEntity : Entity
        {
            return Query(database => database.Set<TEntity>().Find(id));
        }

        public static int Count<TEntity>() where TEntity : class
        {
            return Query(database => database.Set<TEntity>().Count());
        }

        public static ValidationResult Validation<TResult>(IRequest<TResult> message)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var database = serviceProvider.GetService<DirectoryContext>();

                try
                {
                    database.BeginTransaction();
                    EmulateUserContextFilter(serviceProvider, database);


                    var validator = Validator(serviceProvider, message);

                    if (validator == null)
                        throw new Exception($"There is no validator for {message.GetType()} messages.");

                    var validationResult = validator.Validate(message);

                    database.CloseTransaction();

                    return validationResult;
                }
                catch (Exception exception)
                {
                    database.CloseTransaction(exception);
                    throw;
                }
            }
        }

        private static IValidator Validator<TResult>(IServiceProvider serviceProvider, IRequest<TResult> message)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(message.GetType());
            return serviceProvider.GetService(validatorType) as IValidator;
        }

        public static string SampleEmail() => SampleString() + "@example.com";
        public static string SamplePassword() => SampleString();
        public static string SampleFirstName() => SampleString();
        public static string SampleLastName() => SampleString();
        public static string SampleTitle() => SampleString();

        public static Role SampleRole()
        {
            return new Role
            {
                Name = SampleString()
            };
        }

        public static Employee SampleEmployee()
        {
            return new Employee
            {
                Email = SampleEmail(),
                HashedPassword = HashPassword(SamplePassword()),
                FirstName = SampleFirstName(),
                LastName = SampleLastName(),
                Title = SampleTitle(),
                Office = Sample<Office>(),
                PhoneNumber = SamplePhoneNumber()
            };
        }

        private static string SampleString([CallerMemberName]string caller = null)
            => caller.Replace("Sample", "") + "-" + Guid.NewGuid();

        public static TEnum Sample<TEnum>() where TEnum : struct
        {
            var values = Enum.GetValues(typeof(TEnum));
            return (TEnum)values.GetValue(Random.Next(values.Length));
        }

        public static string SamplePhoneNumber()
            => $"({Random.Next(100, 1000)}) {Random.Next(100, 1000)}-{Random.Next(1000, 10000)}";

        public static void ResetRolePermissionMatrix()
        {
            Transaction(directory =>
            {
                directory.Database.ExecuteSqlCommand("DELETE FROM [EmployeeRole]");
                directory.Database.ExecuteSqlCommand("DELETE FROM [RolePermission]");
                directory.Database.ExecuteSqlCommand("DELETE FROM [Role]");
            });
        }

        public static async Task<Role> CreateRole(Action<CreateRole.Command> customize = null)
        {
            var command = new CreateRole.Command
            {
                Name = SampleRole().Name
            };

            customize?.Invoke(command);

            var roleId = (await Send(command)).RoleId;

            return Query<Role>(roleId);
        }

        public static async Task<Employee> Register(Action<RegisterEmployee.Command> customize = null)
        {
            var password = SamplePassword();

            var command = new RegisterEmployee.Command
            {
                Email = SampleEmail(),
                Password = password,
                ConfirmPassword = password,
                FirstName = SampleFirstName(),
                LastName = SampleLastName(),
                Title = SampleTitle(),
                Office = Sample<Office>(),
                PhoneNumber = SamplePhoneNumber()
            };

            customize?.Invoke(command);

            var employeeId = (await Send(command)).EmployeeId;

            return Query<Employee>(employeeId);
        }

        public static async Task<Employee> LogIn()
        {
            var email = SampleEmail();
            var password = SamplePassword();

            var employee = await Register(x =>
            {
                x.Email = email;
                x.Password = password;
                x.ConfirmPassword = password;
            });

            await Send(new LogIn.Command { Email = email, Password = password });

            return employee;
        }

        public static async Task AssignRoles(Employee employee, params Role[] roles)
        {
            await Send(new RoleAssignment.Command
            {
                EmployeeId = employee.Id,
                Roles = roles.Select(x => new RoleSelection
                {
                    RoleId = x.Id,
                    Selected = true
                }).ToArray()
            });
        }

        public static async Task AssignPermissions(Role role, params Permission[] permissions)
        {
            await Send(new PermissionAssignment.Command
            {
                RoleId = role.Id,
                Permissions = permissions
            });
        }
    }
}
