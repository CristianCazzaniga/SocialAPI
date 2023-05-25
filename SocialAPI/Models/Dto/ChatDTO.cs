using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public string UtenteA { get; set; } = null!;
        public string UtenteB { get; set; } = null!;
    }
}
