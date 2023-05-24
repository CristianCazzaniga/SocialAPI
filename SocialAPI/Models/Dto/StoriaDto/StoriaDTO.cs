using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class StoriaDTO
    {
        public string Media { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
    }
}
