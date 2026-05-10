using AutoMapper;
using inzBackend.Models;
using inzBackend.Models.ProfileModels;
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
        }
    }
}
