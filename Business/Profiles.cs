using AutoMapper;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;

namespace Business
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<LeadCreateDto, Lead>();
            CreateMap<Lead, LeadListItemDto>()
                .ForMember(dest => dest.AssignedUserName, option => option.MapFrom(source => source.AssignedUser != null ? $"{source.AssignedUser.FirstName}  {source.AssignedUser.LastName}" : null ));
        }
    }
}
