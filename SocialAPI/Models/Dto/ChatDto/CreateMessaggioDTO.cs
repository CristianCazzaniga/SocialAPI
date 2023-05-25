using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class CreateMessaggioDTO
    {
        public string Contenuto { get; set; } = null!;
    
    }
}
