using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class PostCreateDTO
    {
        public string Media { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
    }
}
