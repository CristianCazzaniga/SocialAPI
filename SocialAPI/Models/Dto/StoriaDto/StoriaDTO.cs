using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class StoriaDTO
    {
        public int Id { get; set; }
        public string Media { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
    }
}
