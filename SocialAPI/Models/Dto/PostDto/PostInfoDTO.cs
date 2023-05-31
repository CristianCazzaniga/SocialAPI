
using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class PostInfoDTO
    {
        public int Id { get; set; }
        public UsernameAndImageDTO User { get; set; } = null!;
        public string Media { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
        public DateTime DataModifica { get; set; }
        public List<UsernameAndImageDTO> likes { get; set; } = null!;
        public List<CommentoDTO> commenti { get; set; } = null!;
    }
}
