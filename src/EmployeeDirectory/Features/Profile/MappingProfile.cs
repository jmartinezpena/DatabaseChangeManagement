namespace EmployeeDirectory.Features.Profile
{
    using AutoMapper;
    using Model;

    public class ProfileMappingProfile : Profile
    {
        public ProfileMappingProfile()
        {
            CreateMap<Employee, EditProfile.Command>();
            CreateMap<EditProfile.Command, Employee>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.HashedPassword, opt => opt.Ignore());
        }
    }
}