using AutoMapper;
using inzBackend.Models;
using inzBackend.Models.CourseModels;
using inzBackend.Models.ProfileModels;
using inzBackend.Models.ProgramModels;
using inzBackend.Models.UserModels;

namespace inzBackend.Profiles
{
    public class GmitrzakEnglishAppMappingProfile : Profile
    {
        public GmitrzakEnglishAppMappingProfile()
        {
            CreateMap<AppUser, AppUserDto>();

            CreateMap<Entities.Profile, ProfileDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role.ToString()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
                .ForMember(dest => dest.EnglishLevel, opt => opt.MapFrom(src => src.EnglishLevel.ToString()));

            CreateMap<Course, CourseDto>();
            CreateMap<Models.Program, ProgramDto>()
                .ForMember(dest => dest.CourseDtos, opt => opt.MapFrom(src =>
                    src.ProgramCourses.Select(pc => pc.Course)));
        }
    }
}
