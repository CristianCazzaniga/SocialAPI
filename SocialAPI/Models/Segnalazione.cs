using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Segnalazione
    {
        public int Id { get; set; }
        public string Motivazione { get; set; } = null!;
        public string fk_UtenteRichiedente { get; set; } = null!;
        [ForeignKey(nameof(fk_UtenteRichiedente))]
        public virtual ApplicationUser UtenteRichiedente { get; set; } = null!;
        public string fk_UtenteSegnalato { get; set; }=null!;
        [ForeignKey(nameof(fk_UtenteSegnalato))]
        public virtual ApplicationUser UtenteSegnalato { get; set; } = null!;
    }
}
