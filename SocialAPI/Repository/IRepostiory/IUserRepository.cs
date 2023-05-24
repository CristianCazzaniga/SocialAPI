using SocialAPI.Models;
using SocialAPI.Models.Dto;

namespace SocialAPI.Repository.IRepostiory
{
    public interface IUserRepository: IRepository<ApplicationUser>
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
