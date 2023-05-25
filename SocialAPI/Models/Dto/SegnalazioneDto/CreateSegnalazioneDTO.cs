using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class CreateSegnalazioneDTO
    {
        public string Motivazione { get; set; } = null!;
    }
}
