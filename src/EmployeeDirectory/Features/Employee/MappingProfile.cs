namespace EmployeeDirectory.Features.Employee
{
    using AutoMapper;
    using Model;
    using static Infrastructure.PasswordService;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeIndex.ViewModel>();

            CreateMap<Employee, EditEmployee.Command>();
            CreateMap<EditEmployee.Command, Employee>()
                .ForMember(x => x.HashedPassword, options => options.Ignore());

            CreateMap<RegisterEmployee.Command, Employee>()
                .ForMember(x => x.Id, options => options.Ignore())
                .ForMember(x => x.HashedPassword, options => options.MapFrom(x => HashPassword(x.Password)));
        }
    }
}