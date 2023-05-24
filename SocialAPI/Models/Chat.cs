using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public virtual ICollection<ApplicationUser> Utenti { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Messaggio> Messaggi { get; set; } = new List<Messaggio>();
    }
}
