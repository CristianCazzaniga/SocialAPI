using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class MessaggioDTO
    {
        public int Id { get; set; }
        public string Contenuto { get; set; } = null!;
        public bool Mittente { get; set; }
    
    }
}
