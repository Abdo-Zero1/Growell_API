using AutoMapper;
using Growell_API.DTOs;
using Models;

namespace Growell_API.Profiles
{
    public class ApplicationUserProfiles:Profile
    {
        public ApplicationUserProfiles()
        {
         CreateMap<ApplicationUserDTO, ApplicationUser>()
        .ForMember(dest => dest.UserName, option => option.MapFrom(src => $"{src.FristName}_{src.LastName}"))
        .ForMember(dest => dest.Adderss, option => option.MapFrom(src => src.Address)); 
        }
    }
}
