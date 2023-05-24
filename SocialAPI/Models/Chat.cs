using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string UtenteA { get; set; } = null!;
        public string UtenteB { get; set; } = null!;
        [ForeignKey(nameof(UtenteA))]
        public ApplicationUser UserUtenteA { get; set; } = null!;
        [ForeignKey(nameof(UtenteB))]
        public ApplicationUser UserUtenteB { get; set; } = null!;
        public virtual ICollection<Messaggio> Messaggi { get; set; } = new List<Messaggio>();
    }
}
