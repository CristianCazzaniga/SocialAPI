using AutoMapper;
using SocialAPI.Models;
using SocialAPI.Models.Dto;

namespace SocialAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Post, PostDTO>();
            CreateMap<PostDTO, Post>();

            CreateMap<Post, PostCreateDTO>().ReverseMap();
            CreateMap<Post, PostUpdateDTO>().ReverseMap();

            CreateMap<Storia, StoriaDTO>();
            CreateMap<StoriaDTO, Storia>();

            CreateMap<Storia, StoriaCreateDTO>().ReverseMap();

            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}
