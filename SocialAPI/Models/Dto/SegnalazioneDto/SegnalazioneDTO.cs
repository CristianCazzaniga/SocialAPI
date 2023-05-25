using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class SegnalazioneDTO
    {
        public int Id { get; set; }
        public string Motivazione { get; set; } = null!;
        public string utenteRichiedente { get; set; } = null!;
        public string utenteSegnalato { get; set; } = null!;
    }
}
