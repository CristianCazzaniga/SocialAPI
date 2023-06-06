namespace SocialAPI.Models.Dto
{
    public class ChatViewDTO
    {
        public string MioUsername { get; set; } = null!;
        public List<UsernameAndImageDTO> listaChat { get; set; } = null!;
    }
}