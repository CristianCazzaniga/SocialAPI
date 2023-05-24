using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class PostUpdateDTO
    {
        [Required]
        public string Contenuto { get; set; } = null!;
      
    }
}
